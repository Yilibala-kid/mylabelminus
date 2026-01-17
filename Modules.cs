using Newtonsoft.Json;
using SkiaSharp;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

using System;
using System.Windows.Forms;


namespace mylabel.Modules
{
    public static class Modules
    {

        /// <summary>
        /// 核心绘图函数：绘制序号及竖向文字
        /// </summary>
        public static void DrawAnnotations(Graphics g, ImageInfo currentImage, Size imgSize,float scale, ImageLabel selectedLabel, 
                                                     float imageDpi, Func<string, Color> colorPicker, bool onlyShowIndex = false)
        {
            // 如果当前图片对象为空，直接退出
            if (currentImage == null || currentImage.Labels == null) return;
            // 开启抗锯齿，保证缩放后文字依然清晰
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            // 1. 预创建居中格式（用于序号）和竖排格式（用于内容）
            using var centerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var vFormat = new StringFormat { FormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft };
            // 序号字体
            using var indexFont = new Font("Arial", 20 * imageDpi, FontStyle.Bold, GraphicsUnit.Pixel);

            foreach (var label in currentImage.Labels)
            {
                if (label.IsDeleted) continue;

                // 计算基础坐标
                float x = label.Position.X * imgSize.Width;
                float y = label.Position.Y * imgSize.Height;

                bool isSelected = (label == selectedLabel);
                Color groupColor = colorPicker(label.Group);
                Color finalColor = isSelected ? Color.Purple : groupColor;

                using var themeBrush = new SolidBrush(finalColor);

                // --- A. 绘制序号 ---
                string idxStr = label.Index.ToString();

                // 如果没有了圆圈，我们可以给序号加一个极小的阴影或发光，防止在复杂背景下看不清（可选）
                // 这里直接绘制居中文字
                g.DrawString(idxStr, indexFont, themeBrush, x, y, centerFormat);

                // --- B. 绘制内容 ---
                if (onlyShowIndex || string.IsNullOrEmpty(label.Text)) continue;

                float fontSize = (label.FontSize > 0 ? (float)label.FontSize : 12f) * imageDpi;
                Font textFont = GetSafeFont(label.FontFamily, fontSize);

                using (textFont)
                {
                    // 因为没有了圆圈，内容文本的起始位置直接从序号下方偏移一点即可
                    // 25 * imageDpi 大约是序号文字占据的高度空间
                    float textY = y + (15 * imageDpi);
                    g.DrawString(label.Text, textFont, themeBrush, x, textY, vFormat);
                }
            }

        }
        private static Font GetSafeFont(string family, float size)
        {
            try
            {
                // 尝试根据数据库记录的名称创建字体
                return new Font(family, size, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            catch
            {
                // 如果创建失败（比如对方电脑没装这个字体），则强制回退到“微软雅黑”
                // 如果连微软雅黑都没有，Windows 会自动指定一个系统默认字体（如宋体）
                return new Font("Microsoft YaHei", size, FontStyle.Regular, GraphicsUnit.Pixel);
            }
        }

        /// <summary>
        /// 将特定格式的文本解析为 imageLabel 列表
        /// </summary>
        public static Dictionary<string, ImageInfo> ParseTextToLabels(string content)
        {
            var database = new Dictionary<string, ImageInfo>();
            var groupList = new List<string>(); // 存储从文件头读取到的动态分组

            // 使用预编译正则，提高性能
            var imgRegex = new Regex(@">>>>>>>>\[(.*?)\]<<<<<<<<", RegexOptions.Compiled);
            var metaRegex = new Regex(@"----------------\[(\d+)\]----------------\[([\d\.]+),([\d\.]+),(\d+)\]", RegexOptions.Compiled);

            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string currentImgName = null;
            ImageLabel currentLabel = null;
            int hyphenCount = 0; // 用于追踪我们处理到了第几个 "-" 分隔符

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                // 1. 处理文件头分组
                if (line == "-") { hyphenCount++; continue; }
                if (hyphenCount == 1) { groupList.Add(line); continue; }
                // 2. 识别图片
                var imgMatch = imgRegex.Match(line);
                if (imgMatch.Success)
                {
                    // 在切换图片前，确保上一个 Label 的状态被锁定
                    currentLabel?.LoadBaseContent(currentLabel.Text);

                    currentImgName = imgMatch.Groups[1].Value;
                    database[currentImgName] = new ImageInfo { ImageName = currentImgName };
                    currentLabel = null; // 重置当前标签
                    continue;
                }

                // --- 3. 识别标注元数据行 ---
                var metaMatch = metaRegex.Match(line);
                if (metaMatch.Success && currentImgName != null)
                {
                    // 在处理新 Label 前，锁定上一个 Label 的原文
                    currentLabel?.LoadBaseContent(currentLabel.Text);

                    int groupIdx = int.Parse(metaMatch.Groups[4].Value);
                    string groupName = (groupIdx > 0 && groupIdx <= groupList.Count)
                                       ? groupList[groupIdx - 1]
                                       : (groupIdx == 2 ? "框外" : "框内");

                    currentLabel = new ImageLabel
                    {
                        Index = int.Parse(metaMatch.Groups[1].Value),
                        Position = new BoundingBox(float.Parse(metaMatch.Groups[2].Value), float.Parse(metaMatch.Groups[3].Value), 0, 0),
                        Group = groupName,
                        Text = "" // 暂时为空，等待下方读取文本行
                    };
                    database[currentImgName].Labels.Add(currentLabel);
                    continue;
                }

                // --- 4. 识别标注文本内容 ---
                // 排除掉干扰行（如头部信息和分隔符）
                if (currentLabel != null && hyphenCount >= 2)
                {
                    currentLabel.Text = string.IsNullOrEmpty(currentLabel.Text)
                                        ? line
                                        : currentLabel.Text + Environment.NewLine + line;
                }
            }
            foreach (var img in database.Values)
            {
                foreach (var lbl in img.Labels)
                {
                    // 将当前读到的 Text 正式转为 OriginalText，并重置 IsModified
                    lbl.LoadBaseContent(lbl.Text);
                }
            }

            return database;
        }

        /// <summary>
        /// 将列表数据转换回特定的 txt 文本格式
        /// </summary>
        public enum ExportMode { Original, Current, Diff }
        public static string LabelsToText(Dictionary<string, ImageInfo> imageDatabase, ExportMode mode = ExportMode.Current)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // --- 1. 获取分组映射 (保持原逻辑) ---
            var allGroups = imageDatabase.Values
                    .SelectMany(img => img.Labels)
                    .Select(l => l.Group).Distinct()
                    .OrderBy(g => g == "框内" ? 0 : (g == "框外" ? 1 : 2)).ThenBy(g => g).ToList();
            if (allGroups.Count == 0) { allGroups.Add("框内"); allGroups.Add("框外"); }
            var groupToIdMap = allGroups.Select((g, i) => new { g, id = i + 1 }).ToDictionary(x => x.g, x => x.id);
            // 写入头部
            sb.AppendLine("1,0\n-\n" + string.Join("\n", allGroups) + "\n-\nDefault Comment\nYou can edit me\n");

            // --- 2. 遍历图片 ---
            foreach (var imageName in imageDatabase.Keys.OrderBy(name => name))
            {
                var imageInfo = imageDatabase[imageName];

                // 修改点：如果不是 Diff 模式，检查是否存在未删除的标签；如果是 Diff 模式，检查是否有修改
                bool shouldExportImage = mode == ExportMode.Diff
                    ? imageInfo.Labels.Any(l => l.IsModified)
                    : imageInfo.Labels.Any(l => !l.IsDeleted);

                if (!shouldExportImage) continue;

                string pureName = System.IO.Path.GetFileName(imageName);
                sb.AppendLine($">>>>>>>>[{pureName}]<<<<<<<<");

                // --- 3. 遍历标注 ---
                foreach (var label in imageInfo.Labels.OrderBy(l => l.Index))
                {
                    // --- 核心修改：根据模式过滤标签 ---
                    if (mode == ExportMode.Diff)
                    {
                        // Diff 模式：只导出有变动的（包括已删除的和内容修改的）
                        if (!label.IsModified) continue;
                    }
                    else
                    {
                        // 普通模式 (Current/Original)：绝对不导出已删除的标签
                        if (label.IsDeleted) continue;
                    }

                    // 写入坐标和组信息
                    int groupValue = groupToIdMap.ContainsKey(label.Group) ? groupToIdMap[label.Group] : 1;
                    sb.AppendLine($"----------------[{label.Index}]----------------[{label.X:F3},{label.Y:F3},{groupValue}]");

                    // --- 4. 写入文本内容 ---
                    if (mode == ExportMode.Diff)
                    {
                        if (label.IsDeleted)
                        {
                            sb.AppendLine("【状态】：!!! 已删除 !!!");
                            sb.AppendLine($"【原内容】：{label.OriginalText}");
                        }
                        else
                        {
                            sb.AppendLine($"【OLD】\n{label.OriginalText}");
                            sb.AppendLine($"【NEW】\n{label.Text}");
                        }
                    }
                    else
                    {
                        // Current 模式写 Text，Original 模式写 OriginalText
                        sb.AppendLine(mode == ExportMode.Original ? label.OriginalText : label.Text);
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 识别字体函数
        /// </summary>
        public static async Task<string> GetFontRecognitionResult(string imagePath)
        {
            using (var client = new HttpClient())
            {
                // 设置超时时间，防止 Docker 没启动时程序卡死
                client.Timeout = TimeSpan.FromSeconds(10);
                var url = "http://localhost:7860/api/predict";

                byte[] imageBytes = File.ReadAllBytes(imagePath);
                string base64Image = "data:image/png;base64," + Convert.ToBase64String(imageBytes);

                var requestBody = new { data = new[] { base64Image } };
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        dynamic resultObj = JsonConvert.DeserializeObject(jsonResponse);

                        // 根据 demo.py，第一个返回项通常是 Label 概率字典
                        // 我们取概率最高（第一个）的 Key
                        var predictions = resultObj.data[0];
                        string bestMatch = "未知字体";

                        // 这里的解析逻辑取决于 Gradio 具体的返回格式
                        // 通常是一个 Dictionary<string, double>
                        foreach (var prop in predictions)
                        {
                            bestMatch = prop.Name; // 拿到第一个 Key 就跳出
                            break;
                        }
                        return bestMatch;
                    }
                }
                catch (Exception)
                {
                    return "本地 Docker 未响应";
                }
            }
            return "识别未成功";
        }
    }
    public static class UIHelper
    {

        /// <summary>
        /// 1. 递归绑定：点击非输入控件时，强制让目标控件（如PicView）获取焦点
        /// </summary>
        public static void BindFocusTransfer(Control parent, Control targetFocusControl)
        {
            foreach (Control ctrl in parent.Controls)
            {
                // 如果不是输入类控件，则绑定点击事件
                if (!(ctrl is TextBox || ctrl is ComboBox || ctrl is DataGridView || ctrl is Button))
                {
                    ctrl.MouseDown += (s, e) => {
                        if (targetFocusControl.CanFocus) targetFocusControl.Focus();
                    };
                }
                // 递归处理子容器
                if (ctrl.HasChildren) BindFocusTransfer(ctrl, targetFocusControl);
            }
        }

        /// <summary>
        /// 3. 自动根据内容最长项调整 ComboBox 下拉框宽度
        /// </summary>
        public static void AdaptDropDownWidth(ComboBox cb)
        {
            int maxWidth = cb.Width;
            using (Graphics g = cb.CreateGraphics())
            {
                foreach (var item in cb.Items)
                {
                    int itemWidth = (int)g.MeasureString(item.ToString(), cb.Font).Width;
                    if (itemWidth > maxWidth) maxWidth = itemWidth;
                }
            }
            cb.DropDownWidth = maxWidth + 20; // 预留滚动条空间
        }
    }

    public static class ThemeManager
    {
        public static bool IsDarkMode { get; private set; } = false;

        // --- 1. 核心颜色定义 (私有) ---
        // 黑暗模式
        private static readonly Color Dark_Win = Color.FromArgb(25, 25, 25);
        private static readonly Color Dark_Pnl = Color.FromArgb(40, 40, 40);
        private static readonly Color Dark_Img = Color.FromArgb(20, 20, 20);
        private static readonly Color Dark_Txt = Color.FromArgb(220, 220, 220);
        private static readonly Color Dark_Act = Color.FromArgb(45, 90, 48);
        private static readonly Color Dark_Bdr = Color.FromArgb(70, 70, 70);

        // 白天模式
        private static readonly Color Light_Win = SystemColors.Control;
        private static readonly Color Light_Pnl = Color.PeachPuff;
        private static readonly Color Light_Img = Color.SeaShell; // 你的 SeaShell
        private static readonly Color Light_Txt = SystemColors.ControlText;
        private static readonly Color Light_Act = Color.LightGreen;
        private static readonly Color Light_Bdr = Color.FromArgb(210, 205, 195);

        // --- 2. 对外统一接口 (利用三元表达式简化) ---
        public static Color BackColor => IsDarkMode ? Dark_Win : Light_Win;
        public static Color PanelColor => IsDarkMode ? Dark_Pnl : Light_Pnl;
        public static Color PicViewBg => IsDarkMode ? Dark_Img : Light_Img;
        public static Color TextColor => IsDarkMode ? Dark_Txt : Light_Txt;
        public static Color AccentColor => IsDarkMode ? Dark_Act : Light_Act;
        public static Color BorderColor => IsDarkMode ? Dark_Bdr : Light_Bdr;
        // 输入激活状态：黑暗模式用深灰，白天用纯白
        public static Color InputActiveBg => IsDarkMode ? Color.FromArgb(65, 65, 65) : Color.White;

        // 输入闲置状态：黑暗模式用近黑，白天用你喜欢的 AntiqueWhite
        public static Color InputIdleBg => IsDarkMode ? Color.FromArgb(35, 35, 35) : Color.AntiqueWhite;

        // 对应的文字颜色
        public static Color InputActiveText => IsDarkMode ? Color.White : Color.Black;
        public static Color InputIdleText => Color.Gray;
        /// <summary>
        /// 切换主题的主入口
        /// </summary>
        public static void ToggleTheme(Form form) { IsDarkMode = !IsDarkMode; ApplyTheme(form); }

        public static void ApplyTheme(Form form)
        {
            form.BackColor = BackColor;
            form.ForeColor = TextColor;
            ProcessControls(form.Controls);
            SetTitleBarTheme(form.Handle, IsDarkMode);
            form.Invalidate(true);
        }

        private static void ProcessControls(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is RadioButton && ctrl.Parent?.Name == "flowGroups")
                {
                    continue;
                }
                // 使用新版 C# 的模式匹配
                switch (ctrl)
                {
                    case Panel p:
                        if (p.Name == "LabelViewpanel"|| p.Name == "Textboxpanel") 
                        {
                            p.BackColor = Color.Orange;
                        }
                        else if(p.Name == "PicView")
                        {
                            p.BackColor = PicViewBg;
                        }
                        else
                            p.BackColor = PanelColor;
                        break;
                    case Button btn:
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.BackColor = IsDarkMode ? Color.FromArgb(60, 60, 60) : Color.OldLace;
                        btn.FlatAppearance.BorderColor = IsDarkMode ?  BorderColor:Color.Tan; // 使用你定义的 BorderColor
                        btn.FlatAppearance.BorderSize = 2;
                        break;
                    case DataGridView dgv:
                        ApplyDgvTheme(dgv);
                        break;
                    case ToolStrip ts:
                        ts.Renderer = new DarkThemeRenderer();
                        ts.BackColor = BackColor;
                        break;
                    case TextBox tb:
                        tb.BackColor = InputIdleBg;
                        tb.ForeColor = InputIdleText;
                        break;
                    case ComboBox cb:
                        cb.FlatStyle = FlatStyle.Flat;
                        cb.BackColor = IsDarkMode ? Color.FromArgb(50, 50, 50) : Color.White;
                        cb.ForeColor = IsDarkMode ? Color.White : Color.Black;
                        break;
                }

                ctrl.ForeColor = TextColor;
                if (ctrl.HasChildren) ProcessControls(ctrl.Controls);
            }
        }
        public class DarkThemeRenderer : ToolStripProfessionalRenderer
        {
            public DarkThemeRenderer() : base(IsDarkMode ? (ProfessionalColorTable)new DarkColors() : new LightColors()) { }
            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            { e.TextColor = TextColor; base.OnRenderItemText(e); }
        }

        // 定义黑暗模式下的具体颜色表
        public class DarkColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected => Color.FromArgb(80, 80, 80);        // 鼠标滑过背景
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(80, 80, 80);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(80, 80, 80);
            public override Color MenuItemBorder => Color.FromArgb(100, 100, 100);       // 菜单边框
            public override Color MenuBorder => Color.FromArgb(40, 40, 40);             // 弹出菜单外框
            public override Color ToolStripDropDownBackground => Color.FromArgb(45, 45, 45); // 下拉背景
            public override Color ImageMarginGradientBegin => Color.FromArgb(45, 45, 45);   // 左侧图标栏
            public override Color ImageMarginGradientMiddle => Color.FromArgb(45, 45, 45);
            public override Color ImageMarginGradientEnd => Color.FromArgb(45, 45, 45);
        }

        public class LightColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected => Color.FromArgb(230, 225, 215); // 接近 SeaShell 的深色
            public override Color ToolStripDropDownBackground => Color.White;
            public override Color MenuBorder => Color.FromArgb(210, 205, 195);
            public override Color ImageMarginGradientBegin => Color.FromArgb(252, 251, 248); // 极淡的底色
        }
        private static void ApplyDgvTheme(DataGridView dgv)
        {
            dgv.BackgroundColor = BackColor;
            dgv.GridColor = BorderColor;
            dgv.DefaultCellStyle.BackColor = IsDarkMode ? Dark_Pnl:Color.White;
            dgv.DefaultCellStyle.ForeColor = TextColor;
            dgv.DefaultCellStyle.SelectionBackColor = Color.Orange;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = IsDarkMode ? Color.FromArgb(55, 55, 55) : Color.FromArgb(235, 230, 220);
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextColor;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = IsDarkMode ? Color.FromArgb(55, 55, 55) : Color.FromArgb(235, 230, 220);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
        }

        // --- Windows API 标题栏处理 ---
        [DllImport("dwmapi.dll")] private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        private static void SetTitleBarTheme(IntPtr handle, bool dark)
        {
            if (Environment.OSVersion.Version.Major >= 10)
            {
                int v = dark ? 1 : 0;
                DwmSetWindowAttribute(handle, 20, ref v, sizeof(int));
            }
        }
    }

}

