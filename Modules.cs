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
        public static void DrawAnnotations(Graphics g, ImageInfo currentImage, Size imgSize,
                                                     float scale, ImageLabel selectedLabel, float imageDpi, bool onlyShowIndex = false)
        {
            // 如果当前图片对象为空，直接退出
            if (currentImage == null || currentImage.Labels == null) return;
            // 开启抗锯齿，保证缩放后文字依然清晰
            g.TextRenderingHint = TextRenderingHint.AntiAlias;


            // 准备竖排格式
            using (StringFormat vFormat = new StringFormat())
            {
                vFormat.FormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft;
                vFormat.Alignment = StringAlignment.Near;

                foreach (var label in currentImage.Labels)
                {

                    // 计算在图片上的实际像素坐标
                    float x = label.Position.X * imgSize.Width;
                    float y = label.Position.Y * imgSize.Height;

                    // 颜色逻辑
                    bool isSelected = (label == selectedLabel);
                    Brush themeBrush = isSelected ? Brushes.Purple : Brushes.Red;
                    // --- A. 绘制序号 (始终绘制) ---
                    using (Font indexFont = new Font("Arial", 20 * imageDpi, FontStyle.Bold, GraphicsUnit.Pixel))
                    {
                        string idxStr = label.Index.ToString();
                        SizeF size = g.MeasureString(idxStr, indexFont);
                        float centerX = x - (size.Width / 2f);
                        float centerY = y - (size.Height / 2f);

                        g.DrawString(idxStr, indexFont, themeBrush, centerX, centerY);

                        // --- B. 绘制内容 (受 onlyShowIndex 控制) ---
                        // 如果开启了“仅显示序号”，或者文本为空，则跳过以下逻辑
                        if (onlyShowIndex || string.IsNullOrEmpty(label.Text)) continue;

                        // 只有非文校模式才创建这些字体，节省性能
                        float currentOriginFontSize = label.FontSize > 0 ? (float)label.FontSize : 12f;
                        float currentFontSize = currentOriginFontSize * imageDpi;

                        Font textFont = null;
                        try
                        {
                            textFont = new Font(label.FontFamily, currentFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                        }
                        catch
                        {
                            textFont = new Font("Microsoft YaHei", currentFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                        }

                        using (textFont)
                        {
                            float textX = centerX + (currentFontSize * 0.4f);
                            g.DrawString(label.Text, textFont, themeBrush, textX, y, vFormat);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将特定格式的文本解析为 imageLabel 列表
        /// </summary>
        public static Dictionary<string, ImageInfo> ParseTextToLabels(string content)
        {
            var database = new Dictionary<string, ImageInfo>();
            var groupList = new List<string>(); // 存储从文件头读取到的动态分组

            // 使用正则预编译，提高性能
            var imgRegex = new Regex(@">>>>>>>>\[(.*?)\]<<<<<<<<", RegexOptions.Compiled);
            var metaRegex = new Regex(@"----------------\[(\d+)\]----------------\[([\d\.]+),([\d\.]+),(\d+)\]", RegexOptions.Compiled);

            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            string currentImgName = string.Empty;
            ImageLabel currentLabel = null;
            int hyphenCount = 0; // 用于追踪我们处理到了第几个 "-" 分隔符

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // --- 1. 动态解析文件头部的分组信息 ---
                if (line == "-")
                {
                    hyphenCount++;
                    continue;
                }

                // 当处于第一个和第二个 "-" 之间时，认为这些行是分组名称
                if (hyphenCount == 1)
                {
                    groupList.Add(line);
                    continue;
                }

                // --- 2. 识别图片行 ---
                var imgMatch = imgRegex.Match(line);
                if (imgMatch.Success)
                {
                    // 移除后缀（如果你需要的话，可以用 Path.GetFileNameWithoutExtension）
                    currentImgName = imgMatch.Groups[1].Value;

                    if (!database.ContainsKey(currentImgName))
                    {
                        database[currentImgName] = new ImageInfo { ImageName = currentImgName };
                    }
                    continue;
                }

                // --- 3. 识别标注元数据行 ---
                var metaMatch = metaRegex.Match(line);
                if (metaMatch.Success)
                {
                    if (string.IsNullOrEmpty(currentImgName)) continue;

                    int index = int.Parse(metaMatch.Groups[1].Value);
                    float x = float.Parse(metaMatch.Groups[2].Value);
                    float y = float.Parse(metaMatch.Groups[3].Value);
                    int groupIdx = int.Parse(metaMatch.Groups[4].Value);

                    // 根据文件头的索引获取组名 (文件里是从 1 开始的)
                    string groupName = "默认分组";
                    if (groupIdx > 0 && groupIdx <= groupList.Count)
                    {
                        groupName = groupList[groupIdx - 1];
                    }
                    else
                    {
                        // 兼容性逻辑：如果 idx 是 1 或 2 且 groupList 为空
                        if (groupIdx == 1) groupName = "框内";
                        else if (groupIdx == 2) groupName = "框外";
                    }

                    currentLabel = new ImageLabel
                    {
                        Index = index,
                        Position = new BoundingBox { X = x, Y = y, Width = 0f, Height = 0f },
                        Group = groupName,
                        Text = ""
                    };

                    database[currentImgName].Labels.Add(currentLabel);
                    continue;
                }

                // --- 4. 识别标注文本内容 ---
                // 排除掉干扰行（如头部信息和分隔符）
                if (currentLabel != null && hyphenCount >= 2 && !line.StartsWith(">") && !line.StartsWith("-"))
                {
                    if (string.IsNullOrEmpty(currentLabel.Text))
                        currentLabel.Text = line;
                    else
                        currentLabel.Text += Environment.NewLine + line;
                }
            }

            return database;
        }

        /// <summary>
        /// 将列表数据转换回特定的 txt 文本格式
        /// </summary>
        public static string LabelsToText(Dictionary<string, ImageInfo> imageDatabase)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // --- 1. 动态获取所有 Group 并排序 ---
            // 找出所有图片中用到的所有组名，并去重
            var allGroups = imageDatabase.Values
                    .SelectMany(img => img.Labels)
                    .Select(l => l.Group)
                    .Distinct()
                    .OrderBy(g => g == "框内" ? 0 : (g == "框外" ? 1 : 2)) // 强制框内=1, 框外=2
                    .ThenBy(g => g) // 其余按字母排
                    .ToList();

            // 如果没有任何标签，至少给个默认值
            if (allGroups.Count == 0) { allGroups.Add("框内"); allGroups.Add("框外"); }

            // 建立 组名 -> 编号 的映射 (从 1 开始)
            var groupToIdMap = new Dictionary<string, int>();

            // 写入头部
            sb.AppendLine("1,0");
            sb.AppendLine("-");
            for (int i = 0; i < allGroups.Count; i++)
            {
                string gName = allGroups[i];
                groupToIdMap[gName] = i + 1; // 存储映射关系
                sb.AppendLine(gName);       // 写入文件头
            }
            sb.AppendLine("-");
            sb.AppendLine("Default Comment");
            sb.AppendLine("You can edit me");
            sb.AppendLine();

            // 2. 字典的 Keys 已经是唯一的图片名，直接排序后遍历
            var sortedImageNames = imageDatabase.Keys.OrderBy(name => name);

            foreach (var imageName in sortedImageNames)
            {
                var imageInfo = imageDatabase[imageName];

                // 写入图片分隔符（使用 Path.GetFileName 确保格式统一）
                string pureName = System.IO.Path.GetFileName(imageName);
                sb.AppendLine($">>>>>>>>[{pureName}]<<<<<<<<");

                // 3. 遍历该图片下的所有标注（已经由 ImageInfo 管理，直接排序 Index 即可）
                foreach (var label in imageInfo.Labels.OrderBy(l => l.Index))
                {
                    // 从映射表中取值
                    if (!groupToIdMap.TryGetValue(label.Group, out int groupValue))
                    {
                        groupValue = 1;
                    }

                    // 写入标注行
                    sb.AppendLine($"----------------[{label.Index}]----------------[{label.Position.X:F3},{label.Position.Y:F3},{groupValue}]");

                    // 写入文本内容
                    sb.AppendLine(label.Text);
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

