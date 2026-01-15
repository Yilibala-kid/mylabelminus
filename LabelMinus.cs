using LabelMinus;
using LabelMinus.LabelMinus;
using mylabel.Modules;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;
using Label = System.Windows.Forms.Label;
using TextBox = System.Windows.Forms.TextBox;
namespace mylabel
{
    public partial class LabelMinusForm : Form
    {
        private string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".webp" };

        private UndoManager _undoManager = new UndoManager();
        private Dictionary<string, ImageInfo> imageDatabase = new();

        private string currentTranslationPath = string.Empty;
        private string currentFolder = string.Empty;
        private ImageInfo CurrentInfo => PicNameBindingSource.Current as ImageInfo;// 快捷获取当前图片
        private ImageLabel CurrentSelectedLabel => imageLabelBindingSource.Current as ImageLabel;// 快捷获取当前选中的标注

        private Image image;          // 当前显示的图片
        private float scale = 1f;     // 缩放比例
        private PointF offset = new PointF(0, 0); // 平移偏移
        private PointF ScreenToImage(Point screenPoint)
        {
            float x = (screenPoint.X - offset.X) / scale;
            float y = (screenPoint.Y - offset.Y) / scale;
            return new PointF(x, y);
        }
        private PointF ImageToScreen(PointF imagePoint)
        {
            float x = imagePoint.X * scale + offset.X;
            float y = imagePoint.Y * scale + offset.Y;
            return new PointF(x, y);
        }



        public LabelMinusForm()
        {
            InitializeComponent();
            

            Modules.UIHelper.BindFocusTransfer(this, PicView);
        }
        #region 窗口加载关闭要做的
        private void LabelMinus_Load(object sender, EventArgs e)
        {
            this.ActiveControl = PicView;
            // 启动清理
            ClearTempFolders();

            LoadSystemFonts(FontstylecomboBox);
            InitFontSizeComboBox();
            InitOCRComboBox();
            BeautifyUI();
            ThemeManager.ApplyTheme(this);
            SetMode("LabelMode");
        }
        private void LabelMinusForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 如果你的 UndoManager 能判断当前状态是否与初始状态一致：
            if (_undoManager.HasUnsavedChanges)
            {
                // 2. 弹出对话框询问用户
                DialogResult result = MessageBox.Show(
                    "您有未保存的更改，是否在退出前保存？",
                    "保存确认",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // 执行你的保存逻辑
                    SaveTranslation_Click(null, null);
                }
                else if (result == DialogResult.Cancel)
                {
                    // 3. 关键：取消关闭动作，回到程序
                    e.Cancel = true;
                }
                // 如果选 No，则不执行任何操作，窗体正常关闭
            }
        }

        private void LabelMinusForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClearTempFolders();
        }
        #region 加载字体/字号
        private void LoadSystemFonts(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox.ItemHeight = 25;

            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (FontFamily font in fonts.Families)
            {
                // 改为存储字符串名称
                comboBox.Items.Add(font.Name);
            }

            comboBox.DrawItem -= ComboBox_DrawItem;
            comboBox.DrawItem += ComboBox_DrawItem;

            comboBox.Sorted = true;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // 核心绘图逻辑
        private readonly Dictionary<string, Font> _fontCache = new Dictionary<string, Font>();

        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            string fontName = ((ComboBox)sender).Items[e.Index].ToString();
            e.DrawBackground();

            // 优先从缓存获取，没有再创建
            if (!_fontCache.TryGetValue(fontName, out Font displayFont))
            {
                try
                {
                    displayFont = new Font(fontName, 12, FontStyle.Regular);
                }
                catch
                {
                    displayFont = e.Font; // 降级使用默认字体
                }
                _fontCache[fontName] = displayFont;
            }

            Brush brush = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                          ? Brushes.White : Brushes.Black;

            e.Graphics.DrawString(fontName, displayFont, brush, e.Bounds);
            e.DrawFocusRectangle();
        }
        private void InitFontSizeComboBox()
        {
            // 清空现有项
            FontsizecomboBox.Items.Clear();

            // 添加常用字号
            double[] commonSizes = { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            foreach (double size in commonSizes)
            {
                FontsizecomboBox.Items.Add(size.ToString("F0")); // 统一保留一位小数格式
            }

            // 设置默认值
            FontsizecomboBox.Text = "12.0";

            // 重要设置：允许用户既能选也能手动输入
            // DropDownStyle 必须为 DropDown（默认就是这个），不能是 DropDownList
            FontsizecomboBox.DropDownStyle = ComboBoxStyle.DropDown;
        }
        #endregion
        private void InitOCRComboBox()
        {
            OCRComboBox.Items.Clear();
            OCRComboBox.Items.Add("识字体网 (LikeFont)");
            OCRComboBox.Items.Add("AI识别（YuzuMarker.FontDetection）"); // 或者你想加的其他网站
            OCRComboBox.SelectedIndex = 0; // 默认选第一个
        }
        private void ClearTempFolders()
        {
            // 定义需要清理的文件夹列表
            string[] tempFolders = { "OCRtemp", "ScreenShottemp", "Archivetemp" };

            foreach (string folderName in tempFolders)
            {
                try
                {
                    // 获取绝对路径（相对于 .exe 所在目录）
                    string folderPath = Path.Combine(Application.StartupPath, folderName);

                    if (Directory.Exists(folderPath))
                    {
                        // 获取文件夹下所有的文件
                        string[] files = Directory.GetFiles(folderPath);
                        foreach (string file in files)
                        {
                            try
                            {
                                File.Delete(file); // 逐个删除
                            }
                            catch (Exception)
                            {
                                // 如果文件被占用（比如上次运行没关掉），跳过即可
                            }
                        }
                    }
                    else
                    {
                        // 如果文件夹不存在，顺便创建它，省得后面保存图片时报错
                        Directory.CreateDirectory(folderPath);
                    }
                }
                catch (Exception ex)
                {
                    // 记录日志或静默处理
                    Console.WriteLine($"清空文件夹 {folderName} 失败: {ex.Message}");
                }
            }
        }
        #endregion


        #region 图像绘制
        private string prompt =
            "1. 点击“新建翻译”扫描图片文件夹\n" +
            "2. 点击“打开翻译”加载历史进度\n" +
            "3. 在下方列表选择图片开始标注\n" +
            "4. 点击“保存翻译”保存当前进度\n" +
            "\n" +
            "标注模式：左键添加标记，右键删除标记\n" +
            "文校模式：左键悬停选中标记并显示文本内容\n" +
            "识别模式：拉框截图跳转字体识别网站\n" +
            "图校模式：比较两组图片的不同\n" +
            "\n" +
            "没有编辑文本时：\n" +
            "A/D：上一张图片/下一张图片\n" +
            "W/S：上一个标记/下一个标记\n" +
            "R：适应屏幕\n" +
            "ctrl+Z/ctrl+Y  :  撤销/重做对上一个标记的改动\n" +
            "支持右键点击图片中的标记直接删除";
        private void PicView_Paint(object sender, PaintEventArgs e)
        {
            if (image == null)
            {
                DrawEmptyStatePrompt(e.Graphics);
                return;
            }

            // A. 基础环境设置
            e.Graphics.Clear(Modules.ThemeManager.PicViewBg);
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // B. 应用坐标变换
            e.Graphics.TranslateTransform(offset.X, offset.Y);
            e.Graphics.ScaleTransform(scale, scale);

            // C. 画图片
            e.Graphics.DrawImage(image,
                    new RectangleF(0, 0, image.Width, image.Height),
                    new RectangleF(0, 0, image.Width, image.Height),
                    GraphicsUnit.Pixel);

            // D. 画标注点
            // 获取图片的实际 DPI（若图片没设置，默认 96）
            // PS 标准换算：1 point = (DPI / 72) pixels
            bool onlyShowIndex = (_currentDownAction == DoTextReviewMouseDown);
            float dpiScale = image.HorizontalResolution / 72f;

            // 2. 只有都不为空时才绘制
            if (CurrentInfo != null && image != null)
            {
                mylabel.Modules.Modules.DrawAnnotations(
                    e.Graphics,
                    CurrentInfo,       // 替换 this.currentImageInfo
                    image.Size,
                    scale,
                    CurrentSelectedLabel, // 替换 currentSelected 或直接传入
                    dpiScale,
                    onlyShowIndex
                );
            }
            // --- OCR 模式独有层 ---
            if (_currentDownAction == DoOCRMouseDown)
            {
                e.Graphics.ResetTransform();
                // 在左上角画个小标识，提醒用户正在文校模式
                e.Graphics.DrawString("● 正在截图！！", this.Font, Brushes.Green, 10, 10);

            }
            if (_currentDownAction == DoOCRMouseDown && isSelectingRect)
            {
                // 1. 暂时重置坐标系到屏幕坐标
                e.Graphics.ResetTransform();

                // 2. 创建遮罩层（挖洞）
                using (Region maskRegion = new Region(PicView.ClientRectangle))
                {
                    // 关键点：从 Region 中减去当前的截图矩形，形成一个“带洞的蒙版”
                    maskRegion.Exclude(ocrRect);

                    using (Brush overlay = new SolidBrush(Color.FromArgb(120, 0, 0, 0))) // 稍微加深蒙版
                    {
                        e.Graphics.FillRegion(overlay, maskRegion);
                    }
                }

                // 3. 画截图框的虚线边框
                using (Pen p = new Pen(Color.Cyan, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                {
                    e.Graphics.DrawRectangle(p, ocrRect);
                }

                // 4. 画尺寸信息（增加背景，防止在深色图片上看不清）
                string sizeText = $"{ocrRect.Width} x {ocrRect.Height}";
                e.Graphics.FillRectangle(Brushes.Black, ocrRect.X, ocrRect.Y - 20, 70, 18);
                e.Graphics.DrawString(sizeText, this.Font, Brushes.White, ocrRect.X + 2, ocrRect.Y - 18);
            }

            // --- 文校模式 (TextReview) 独有层 (可选) ---
            if (_currentDownAction == DoTextReviewMouseDown)
            {
                e.Graphics.ResetTransform();
                // 在左上角画个小标识，提醒用户正在文校模式
                e.Graphics.DrawString("● TextReview Mode", this.Font, Brushes.Green, 10, 10);

            }

        }
        private void DrawEmptyStatePrompt(Graphics g)
        {
            g.Clear(Color.SeaShell); // 统一背景色

            using (Font titleFont = new Font("微软雅黑", 14, FontStyle.Bold))
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Near;     // 水平居中
                sf.LineAlignment = StringAlignment.Center;  // 垂直居中


                // 绘制主体文字
                g.DrawString(prompt, titleFont, Brushes.DimGray, PicView.ClientRectangle, sf);
            }
        }
        #endregion

        #region PicView事件

        private Point mouseDownLocation;
        private Point _rightClickStartPoint;
        private bool isDragging = false;
        private bool isPotentialClick = false; // 用来标记是否可能是点击
        private const int ClickThreshold = 5; // 像素阈值


        private ImageLabel _pendingLabel = null;
        private ImageLabel _draggingLabel = null;
        private Point ocrStartPoint;         // 截图起点
        private Rectangle ocrRect;           // 当前拉框的矩形区域（屏幕坐标）
        private bool isSelectingRect = false; // 是否正在拉框

        private Action<MouseEventArgs> _currentDownAction;
        private Action<MouseEventArgs> _currentMoveAction;
        private Action<MouseEventArgs> _currentUpAction;
        // 切换模式时，直接更换这个 Action
        private void SetMode(string mode)
        {
            // 1. 清理通用状态
            ResetInternalState();
            // 2. 改激活按钮颜色
            RefreshButtonVisuals(mode);
            // 3. 绑定对应的委托
            switch (mode)
            {
                case "OCR":
                    BindActions(DoOCRMouseDown, DoOCRMouseMove, DoOCRMouseUp);
                    break;
                case "TextReview":
                    BindActions(DoTextReviewMouseDown, DoTextReviewMouseMove, DoTextReviewMouseUp);
                    break;
                case "LabelMode": // 显式定义你的默认打点模式
                default:
                    BindActions(DoDefaultMouseDown, DoDefaultMouseMove, DoDefaultMouseUp);
                    break;
            }
            PicView.Invalidate();
        }
        // 辅助方法：统一重置状态
        private void ResetInternalState()
        {
            isSelectingRect = false;
            isDragging = false;
            isPotentialClick = false;
            _pendingLabel = null;
            _draggingLabel = null;
            TextReviewtoolTip.Hide(PicView);
        }

        // 辅助方法：统一绑定委托
        private void BindActions(Action<MouseEventArgs> down, Action<MouseEventArgs> move, Action<MouseEventArgs> up)
        {
            _currentDownAction = down;
            _currentMoveAction = move;
            _currentUpAction = up;
        }
        private void RefreshButtonVisuals(string activeMode)
        {
            // 定义 按钮 -> 匹配模式 的映射
            var modeMap = new Dictionary<Button, bool>
            {
                { OCRMode, activeMode == "OCR" },
                { TextReviewMode, activeMode == "TextReview" },
                { LabelMode, activeMode == "LabelMode" || activeMode == "Default" }
            };

            // 获取当前主题色（只获取一次）
            bool isDark = Modules.ThemeManager.IsDarkMode;
            Color activeBg = Modules.ThemeManager.AccentColor;
            Color defaultBg = isDark ? Color.FromArgb(60, 60, 60) : SystemColors.Control;
            Color activeFore = isDark ? Color.White : Color.FromArgb(30, 70, 32);
            Color defaultFore = Modules.ThemeManager.TextColor;

            // 循环遍历，自动更新样式
            foreach (var item in modeMap)
            {
                Button btn = item.Key;
                bool isActive = item.Value;

                btn.BackColor = isActive ? activeBg : defaultBg;
                btn.ForeColor = isActive ? activeFore : defaultFore;

                // 可选：如果是激活状态，可以稍微加粗字体或改变边框
                // btn.Font = new Font(btn.Font, isActive ? FontStyle.Bold : FontStyle.Regular);
            }
        }
        private void PicView_MouseDown(object sender, MouseEventArgs e)
        {
            if (image == null) return;
            if (e.Button == MouseButtons.Right)
            {
                _rightClickStartPoint = e.Location;
            }
            else if (e.Button == MouseButtons.Left)
                _currentDownAction?.Invoke(e);
        }
        #region down函数
        private void DoDefaultMouseDown(MouseEventArgs e)// --- 模式 A: 默认按下逻辑 (打点与拖拽准备) ---
        {
            mouseDownLocation = e.Location;
            isDragging = false;
            isPotentialClick = true;
        }
        private void DoOCRMouseDown(MouseEventArgs e)// --- 模式 B: OCR 按下逻辑 (开始拉框) ---
        {
            isSelectingRect = true;
            ocrStartPoint = e.Location;
            ocrRect = new Rectangle(e.Location, new Size(0, 0));
        }
        private void DoTextReviewMouseDown(MouseEventArgs e)// --- 模式 C: 文校模式按下逻辑 (TextReview) ---
        {
            DoDefaultMouseDown(e);
            // 通常文校模式点击也需要支持拖拽（移动图片查看不同标记）所以直接复用默认逻辑即可
            // 关键：如果当前悬停在一个标签上，则准备拖拽该标签
            if (_pendingLabel != null)
            {
                _draggingLabel = _pendingLabel;
                isDragging = true; // 情况A：点中标签，明确开启了拖拽
                isPotentialClick = false;
            }
            else
            {
                // 没点中标签，清空之前的拖拽对象，准备拖动图片
                _draggingLabel = null;
            }
        }
        #endregion
        private void PicView_MouseMove(object sender, MouseEventArgs e)
        {
            _currentMoveAction?.Invoke(e);
        }
        #region move函数
        private void DoDefaultMouseMove(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            CheckDragStarted(e.Location);
            if (isDragging) MoveImageOffset(e.Location);
        }// --- 模式 A: 默认/拖拽模式 ---
        private void DoOCRMouseMove(MouseEventArgs e)
        {
            if (isSelectingRect)
            {
                int x = Math.Min(ocrStartPoint.X, e.X);
                int y = Math.Min(ocrStartPoint.Y, e.Y);
                int width = Math.Abs(ocrStartPoint.X - e.X);
                int height = Math.Abs(ocrStartPoint.Y - e.Y);
                ocrRect = new Rectangle(x, y, width, height);
                PicView.Invalidate();
            }
        }// --- 模式 B: OCR 拉框模式 ---
        private ImageLabel _showingLabel = null; // 当前正在显示的标签对象
        private void DoTextReviewMouseMove(MouseEventArgs e)// --- 模式 C: 文校模式 (包含拖拽 + 悬停检测) ---
        {
            // 1. 拖拽逻辑处理
            if (e.Button == MouseButtons.Left)
            {
                CheckDragStarted(e.Location);
                if (isDragging)
                {
                    if (_draggingLabel != null) // 正在拖拽标签
                    {
                        PointF imgPt = ScreenToImage(e.Location);
                        _draggingLabel.Position = new BoundingBox
                        {
                            X = Math.Clamp(imgPt.X / image.Width, 0, 1),
                            Y = Math.Clamp(imgPt.Y / image.Height, 0, 1)
                        };
                        //TextReviewtoolTip.Show(_draggingLabel.Text, PicView, e.X + 15, e.Y + 15);
                        PicView.Invalidate();
                    }
                    else // 正在拖拽底图
                    {
                        MoveImageOffset(e.Location);
                    }
                    return; // 拖拽时不进行悬停检测
                }
            }

            // 2. 悬停检测逻辑 (优化性能：使用距离平方比较)
            HandleHoverDetection(e.Location);

        }
        private void HandleHoverDetection(Point mousePos)// 2. 悬停检测逻辑 (优化性能：使用距离平方比较)
        {
            if (CurrentInfo?.Labels == null || image == null) return;

            float imgX = (mousePos.X - offset.X) / scale;
            float imgY = (mousePos.Y - offset.Y) / scale;
            float thresholdSq = (30f / scale) * (30f / scale);

            var foundLabel = CurrentInfo.Labels.FirstOrDefault(l =>
            {
                float lx = (float)(l.Position.X * image.Width);
                float ly = (float)(l.Position.Y * image.Height);
                return Math.Pow(lx - imgX, 2) + Math.Pow(ly - imgY, 2) < thresholdSq;
            });

            if (foundLabel != _pendingLabel)
            {
                _pendingLabel = foundLabel;
                hoverTimer.Stop();
                if (_pendingLabel != null)
                {
                    hoverTimer.Start();
                }
                else
                {
                    _showingLabel = null;
                    TextReviewtoolTip.Hide(PicView);
                }
            }
        }
        private void hoverTimer_Tick(object sender, EventArgs e)
        {
            hoverTimer.Stop(); // 停止计时

            if (_pendingLabel != null && _pendingLabel != _showingLabel)
            {
                _showingLabel = _pendingLabel;
                int index = imageLabelBindingSource.IndexOf(_pendingLabel);
                if (index >= 0)
                {
                    imageLabelBindingSource.Position = index;
                }
                // 2. 获取当前对象（即刚刚切换过去的 _pendingLabel）
                if (CurrentSelectedLabel == null) return;

                // 3. 立即聚焦并编辑
                if (!LabelTextBox.Focused)
                {
                    LabelTextBox.Focus();
                }
                LabelTextBox.SelectionStart = LabelTextBox.Text.Length;
                LabelTextBox.SelectionLength = 0;
                // 4. 视觉反馈：显示 ToolTip
                TextReviewtoolTip.Show(CurrentSelectedLabel.Text, PicView,
                    PicView.PointToClient(Cursor.Position).X + 15,
                    PicView.PointToClient(Cursor.Position).Y + 15);
            }
        }

        // 计算两点间距离的平方（比直接算距离快，因为不需要开方）
        private double GetDistanceSq(Point p1, Point p2) => Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2);

        // 处理图片平移的通用逻辑
        private void MoveImageOffset(Point currentPos)
        {
            offset.X += currentPos.X - mouseDownLocation.X;
            offset.Y += currentPos.Y - mouseDownLocation.Y;
            mouseDownLocation = currentPos;
            PicView.Invalidate();
        }

        // 检查是否触发拖拽阈值
        private bool CheckDragStarted(Point currentPos)
        {
            if (isPotentialClick && GetDistanceSq(currentPos, mouseDownLocation) > ClickThreshold * ClickThreshold)
            {
                isDragging = true;
                isPotentialClick = false;
                return true;
            }
            return false;
        }
        #endregion
        private async void PicView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int dx = e.X - _rightClickStartPoint.X;
                int dy = e.Y - _rightClickStartPoint.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                // 仅当位移极小时，才认为是点击，调用删除逻辑
                if (distance < 5)
                {
                    HandleRightClickDelete(e);
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                _currentUpAction?.Invoke(e);
            }
            // 无论什么模式，松开鼠标后这些标志位都要重置
            _draggingLabel = null;
            isDragging = false;
            isPotentialClick = false;
        }
        #region up函数
        // --- 模式 A: 普通打点释放逻辑 ---
        private void DoDefaultMouseUp(MouseEventArgs e)
        {
            if (CurrentInfo == null || image == null) return;
            if (e.Button == MouseButtons.Right)
            {
                // 1. 计算鼠标抬起和按下的距离差
                int dx = e.X - _rightClickStartPoint.X;
                int dy = e.Y - _rightClickStartPoint.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                // 2. 如果移动距离小于 5 像素，判定为“点击”而非“拖拽”
                if (distance < 5)
                {
                    HandleRightClickDelete(e);
                }
            }
            if (!isDragging && isPotentialClick)
            {
                PointF imgPt = ScreenToImage(e.Location);

                if (imgPt.X >= 0 && imgPt.Y >= 0 && imgPt.X <= image.Width && imgPt.Y <= image.Height)
                {
                    var label = new ImageLabel
                    {
                        Position = new BoundingBox { X = imgPt.X / image.Width, Y = imgPt.Y / image.Height }
                    };
                    var cmd = new AddDeleteCommand(CurrentInfo, label, true);
                    _undoManager.Execute(cmd);

                    // 3. UI 选中与快照初始化
                    imageLabelBindingSource.Position = imageLabelBindingSource.IndexOf(label);
                }
            }
        }
        // --- 模式 B: OCR 释放逻辑 ---
        private void DoOCRMouseUp(MouseEventArgs e)
        {
            isSelectingRect = false;
            if (ocrRect.Width > 5 && ocrRect.Height > 5)
            {
                string filePath = CaptureOCRImageAndGetPath();
                ShowOCRWebPage(filePath);

                PointF cornerPt = ScreenToImage(new Point(ocrRect.Right, ocrRect.Top));

                var ocrLabel = CurrentInfo.AddLabelFromPixels(
                            cornerPt.X, cornerPt.Y, 0, 0, image.Width, image.Height);

                imageLabelBindingSource.Position = imageLabelBindingSource.IndexOf(ocrLabel);
            }
        }
        // --- 模式 C: 文校模式释放逻辑 (TextReview) ---
        private void DoTextReviewMouseUp(MouseEventArgs e)
        {
            // 如果文校模式也允许打点，直接调用 DoDefaultMouseUp(e);
            if (!isDragging && isPotentialClick)
            {
                DoDefaultMouseUp(e);
            }
        }
        private void HandleRightClickDelete(MouseEventArgs e)
        {
            if (CurrentInfo == null || image == null) return;

            // 坐标转换
            float imgX = (e.X - offset.X) / scale;
            float imgY = (e.Y - offset.Y) / scale;
            float threshold = 20f / scale;

            var targetLabel = CurrentInfo.Labels.FirstOrDefault(l =>
            {
                float lpX = (float)(l.Position.X * image.Width);
                float lpY = (float)(l.Position.Y * image.Height);
                return Math.Sqrt(Math.Pow(lpX - imgX, 2) + Math.Pow(lpY - imgY, 2)) < threshold;
            });

            if (targetLabel != null)
            {
                // 3. 撤销结算拦截
                if (imageLabelBindingSource.Current == targetLabel)
                {
                    CheckAndPushUndoCommand();
                    // 结算后清空临时指针，防止删除后 PositionChanged 再次访问已删除对象
                    _lastSelectedLabel = null;
                    _snapshotBeforeEdit = null;
                }
                var cmd = new AddDeleteCommand(CurrentInfo, targetLabel, false);
                _undoManager.Execute(cmd);
                PicView.Invalidate();
            }
        }
        #endregion
        private void PicView_MouseWheel(object sender, MouseEventArgs e)
        {
            float oldScale = scale;
            if (e.Delta > 0) scale *= 1.1f;
            else scale /= 1.1f;

            // 缩放围绕鼠标位置
            offset.X = e.X - (e.X - offset.X) * (scale / oldScale);
            offset.Y = e.Y - (e.Y - offset.Y) * (scale / oldScale);

            PicView.Invalidate();
        }
        #endregion

        #region 菜单栏功能
        private void NewTranslation_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                saveFileDialog.Title = "新建翻译文件";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentTranslationPath = saveFileDialog.FileName;
                    currentFolder = Path.GetDirectoryName(currentTranslationPath);

                    // 1. 创建空的翻译文件
                    File.WriteAllText(currentTranslationPath, string.Empty);

                    // 2. 扫描同文件夹内的图片
                    var imageFiles = Directory.EnumerateFiles(currentFolder)
                                               .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()));

                    // 3. 构建新的数据列表 (直接使用 List)
                    var newList = new List<ImageInfo>();
                    foreach (var imgPath in imageFiles)
                    {
                        newList.Add(new ImageInfo { ImageName = Path.GetFileName(imgPath) });
                    }

                    // 4. 同步到内存字典（如果你其他地方还要用到 imageDatabase）
                    imageDatabase = newList.ToDictionary(k => k.ImageName, v => v);

                    // --- 核心改动：刷新 BindingSource ---

                    // 重新挂载数据源，这将自动清空旧 UI 并填充新图片列表
                    PicNameBindingSource.DataSource = newList;

                    if (PicNameBindingSource.Count > 0)
                    {
                        PicNameBindingSource.Position = 0; // 自动触发加载图片和标签
                        FittoView(null, null);
                        MessageBox.Show($"文件已新建！\n识别到 {newList.Count} 张图片。", "成功");
                    }
                    else
                    {
                        MessageBox.Show("文件已新建，但该文件夹内未发现图片。", "提示");
                        image = null;
                        PicView.Invalidate();
                    }
                }
            }
        }

        private void OpenTranslation_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Filter = "文本文件|*.txt";
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 解析数据并获取字典
                        string content = File.ReadAllText(openFile.FileName);
                        this.imageDatabase = mylabel.Modules.Modules.ParseTextToLabels(content);
                        // 2. 更新路径
                        this.currentTranslationPath = openFile.FileName;
                        this.currentFolder = System.IO.Path.GetDirectoryName(this.currentTranslationPath);
                        // 3. UI 刷新
                        PicNameBindingSource.DataSource = imageDatabase.Values.ToList();

                        // 默认加载第一张图的数据（如果在 ComboBox 中有项）
                        if (PicName.Items.Count > 0)
                        {
                            PicNameBindingSource.Position = 0;
                            FittoView(null, null);
                        }
                        else
                        {
                            // 如果文件是空的，清空当前显示
                            image = null;
                        }

                        //MessageBox.Show($"解析完成，共加载 {imageDatabase.Count} 张图片的标注。");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("解析失败: " + ex.Message);
                    }
                }
            }
        }

        private void SaveTranslation_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentTranslationPath))
            {
                MessageBox.Show("路径为空");
                return;
            }

            try
            {
                // 直接把整个字典传进去
                string outputText = mylabel.Modules.Modules.LabelsToText(this.imageDatabase);

                System.IO.File.WriteAllText(currentTranslationPath, outputText, System.Text.Encoding.UTF8);
                _undoManager.MarkAsSaved();// 2. 标记已保存
                MessageBox.Show("保存成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"错误：{ex.Message}");
            }
        }
        private void SaveAsTranslation_Click(object sender, EventArgs e)
        {
            // 1. 检查数据源是否为空
            if (imageDatabase == null || imageDatabase.Count == 0)
            {
                MessageBox.Show("当前没有可以保存的数据。", "提示");
                return;
            }

            // 2. 弹出保存对话框
            using (SaveFileDialog saveFile = new SaveFileDialog())
            {
                saveFile.Filter = "文本文件|*.txt";
                saveFile.Title = "另存为标注文件";

                // 如果当前已经有路径，默认定位到该文件夹并建议文件名
                if (!string.IsNullOrEmpty(currentTranslationPath))
                {
                    saveFile.InitialDirectory = System.IO.Path.GetDirectoryName(currentTranslationPath);
                    saveFile.FileName = System.IO.Path.GetFileName(currentTranslationPath);
                }

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 3. 调用你的树状结构转化逻辑
                        string outputText = mylabel.Modules.Modules.LabelsToText(this.imageDatabase);

                        // 4. 写入用户选择的新路径
                        System.IO.File.WriteAllText(saveFile.FileName, outputText, System.Text.Encoding.UTF8);

                        // 5. 【关键】保存成功后，更新当前文件路径变量
                        this.currentTranslationPath = saveFile.FileName;
                        this.currentFolder = System.IO.Path.GetDirectoryName(this.currentTranslationPath);
                        _undoManager.MarkAsSaved();// 2. 标记已保存
                        MessageBox.Show("另存为成功！", "成功");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"另存为失败：\n{ex.Message}", "错误");
                    }
                }
            }
        }


        private void ModifyGroup_Click(object sender, EventArgs e)
        {
            // 创建一个简单的右键菜单，让用户选择是“增加”还是“删除”
            ContextMenuStrip menu = new ContextMenuStrip();

            var addBtn = new ToolStripMenuItem("添加新分组");
            addBtn.Click += (s, ev) =>
            {
                // 使用简单的输入框获取新组名
                string newGroup = Microsoft.VisualBasic.Interaction.InputBox("请输入新分组名称：", "添加分组", "");
                if (!string.IsNullOrWhiteSpace(newGroup))
                {
                    if (!GroupcomboBox.Items.Contains(newGroup))
                    {
                        GroupcomboBox.Items.Add(newGroup);
                        GroupcomboBox.SelectedItem = newGroup; // 选中新加的
                    }
                    else
                    {
                        MessageBox.Show("该分组已存在");
                    }
                }
            };

            var delBtn = new ToolStripMenuItem("删除当前分组");
            delBtn.Click += (s, ev) =>
            {
                if (GroupcomboBox.SelectedItem != null)
                {
                    string current = GroupcomboBox.SelectedItem.ToString();
                    var result = MessageBox.Show($"确定要删除分组 [{current}] 吗？", "确认删除", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        GroupcomboBox.Items.Remove(current);
                        if (GroupcomboBox.Items.Count > 0) GroupcomboBox.SelectedIndex = 0;
                    }
                }
            };

            menu.Items.Add(addBtn);
            menu.Items.Add(delBtn);

            // 在按钮下方弹出菜单
            menu.Show(Control.MousePosition);
        }

        private void LabelTextFontsize_Click(object sender, EventArgs e)
        {
            ContextMenuStrip fontMenu = new ContextMenuStrip();

            var increaseBtn = new ToolStripMenuItem("增大字号 (+2)");
            increaseBtn.Click += (s, ev) => ChangeFontSize(2);

            var decreaseBtn = new ToolStripMenuItem("缩小字号 (-2)");
            decreaseBtn.Click += (s, ev) => ChangeFontSize(-2);

            var resetBtn = new ToolStripMenuItem("重置默认");
            resetBtn.Click += (s, ev) => {
                LabelTextBox.Font = new Font(LabelTextBox.Font.FontFamily, 12f); // 假设 9 是默认值
            };

            fontMenu.Items.Add(increaseBtn);
            fontMenu.Items.Add(decreaseBtn);
            fontMenu.Items.Add(new ToolStripSeparator());
            fontMenu.Items.Add(resetBtn);

            fontMenu.Show(Control.MousePosition);
        }
        private void ChangeFontSize(float delta)
        {
            float currentSize = LabelTextBox.Font.Size;
            float newSize = currentSize + delta;

            // 限制范围，防止字体变负数或太大导致界面崩溃
            if (newSize < 6) newSize = 6;
            if (newSize > 48) newSize = 48;

            Font newFont = new Font(LabelTextBox.Font.FontFamily, newSize);

            // 同时应用到两个输入框，保持视觉统一
            LabelTextBox.Font = newFont;
        }// 通用的字体缩放方法
        #endregion

        #region LabelView相关
        private void deleteLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentInfo == null || CurrentSelectedLabel == null) return;

            _lastSelectedLabel = null;
            _snapshotBeforeEdit = null;

            CheckAndPushUndoCommand();
            var cmd = new AddDeleteCommand(CurrentInfo, CurrentSelectedLabel, false);
            _undoManager.Execute(cmd);
            PicView.Invalidate();
        }
        private void LabelView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                LabelView.CurrentCell = LabelView.Rows[e.RowIndex].Cells[e.ColumnIndex >= 0 ? e.ColumnIndex : 0];
                LabelView.Rows[e.RowIndex].Selected = true;
            }
        }
        #endregion

        #region 数据绑定
        private void imageLabelBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            // 1. 获取当前选中的标注对象
            var current = imageLabelBindingSource.Current as ImageLabel;

            // 2. 更新只读的坐标显示
            if (current != null)
            {
                Locationshowlabel.Text = $"X: {current.X:F4}\nY: {current.Y:F4}";
            }
            else
            {
                Locationshowlabel.Text = "X: 0.0000\nY: 0.0000";
            }

            // 3. 让画布重绘（因为对象属性已经通过绑定自动更新了）
            PicView.Invalidate();
        }
        private void PicNameBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            // 1. 获取当前选中的图片信息对象
            var currentImg = PicNameBindingSource.Current as ImageInfo;
            if (currentImg == null) return;

            string fullPath = Path.Combine(this.currentFolder, currentImg.ImageName);

            if (File.Exists(fullPath))
            {
                image?.Dispose();
                using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    image = Image.FromStream(fs);
                }
            }
            else
            {
                image = null; // 找不到图时清空
            }

        }

        private ImageLabel _snapshotBeforeEdit; // 编辑前的快照
        private ImageLabel _lastSelectedLabel;  // 上一个选中的对象（结算用）
        private void imageLabelBindingSource_PositionChanged(object sender, EventArgs e)// 选中标签
        {
            // 1. 结算旧目标的变更（如果有的话）
            CheckAndPushUndoCommand();

            // 2. 为新选中的目标拍摄快照
            var current = imageLabelBindingSource.Current as ImageLabel;
            _snapshotBeforeEdit = current?.Clone();

            // 3. UI 反馈
            PicView.Invalidate();
        }
        private void CheckAndPushUndoCommand()
        {
            // 对比“上一个选中的对象”和“它当时的快照”
            if (_lastSelectedLabel != null && _snapshotBeforeEdit != null)
            {
                // 比较：文本、备注、坐标（任何一项变了都存入撤销栈）
                if (_lastSelectedLabel.Text != _snapshotBeforeEdit.Text ||
                    _lastSelectedLabel.Remark != _snapshotBeforeEdit.Remark ||
                    !_lastSelectedLabel.Position.Equals(_snapshotBeforeEdit.Position))
                {
                    var cmd = new LabelStateCommand(
                        _lastSelectedLabel,
                        _snapshotBeforeEdit,      // 之前的样子
                        _lastSelectedLabel.Clone(), // 现在的样子
                        () => { PicView.Invalidate(); }
                    );
                    _undoManager.PushManual(cmd);
                }
            }

            // 重要：结算完旧的，把指针指向“当前正在使用的对象”，作为下一次切换时的“旧对象”
            _lastSelectedLabel = imageLabelBindingSource.Current as ImageLabel;
        }
        #endregion

        #region 功能模式栏
        private void LabelMode_Click(object sender, EventArgs e)
        {
            SetMode("LabelMode");
        }
        private void TextReviewMode_Click(object sender, EventArgs e)
        {
            SetMode(_currentDownAction == DoTextReviewMouseDown ? "LabelMode" : "TextReview");
        }
        private void OCRMode_Click(object sender, EventArgs e)
        {
            SetMode(_currentDownAction == DoOCRMouseDown ? "LabelMode" : "OCR");
        }

        private string CaptureOCRImageAndGetPath()
        {
            try
            {
                // 1. 坐标转换：将屏幕选区转换回图片的原始像素坐标
                PointF imgStart = ScreenToImage(new Point(ocrRect.Left, ocrRect.Top));
                PointF imgEnd = ScreenToImage(new Point(ocrRect.Right, ocrRect.Bottom));

                float x = Math.Max(0, imgStart.X);
                float y = Math.Max(0, imgStart.Y);
                float width = Math.Min(image.Width - x, imgEnd.X - imgStart.X);
                float height = Math.Min(image.Height - y, imgEnd.Y - imgStart.Y);

                if (width <= 0 || height <= 0) return null;

                // 2. 准备目录
                string tempDir = Path.Combine(Application.StartupPath, "OCRtemp");
                if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
                string filePath = Path.Combine(tempDir, $"OCR_{DateTime.Now:yyyyMMdd_HHmmss}.png");

                // 3. 创建位图并执行裁剪绘制
                // 这里 bmp 必须在 using 中，确保保存后立即释放文件句柄
                using (Bitmap bmp = new Bitmap((int)width, (int)height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        // 设置高质量插值，有助于提升 OCR 识别率
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(image,
                            new Rectangle(0, 0, bmp.Width, bmp.Height),
                            new RectangleF(x, y, width, height),
                            GraphicsUnit.Pixel);
                    }

                    // 4. 保存文件
                    bmp.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show("裁剪图片失败: " + ex.Message);
                return null;
            }
        }
        private void ShowOCRWebPage(string imagePath)
        {
            // 获取用户选择的网站
            string selectedSite = OCRComboBox.SelectedItem?.ToString() ?? "";
            string targetUrl = "https://www.likefont.com/"; // 默认值

            if (selectedSite.Contains("AI识别（YuzuMarker.FontDetection）"))
            {
                targetUrl = "https://huggingface.co/spaces/gyrojeff/YuzuMarker.FontDetection"; // 举例
            }
            else if (selectedSite.Contains("识字体网 (LikeFont)"))
            {
                targetUrl = "https://www.likefont.com/";
            }
            // 检查是否已经有名为 "OCRContainer" 的窗体在运行
            Form existing = Application.OpenForms["OCRContainer"];
            if (existing != null)
            {
                // 如果已打开，只需更新图片和剪贴板，然后置顶
                Clipboard.SetText(imagePath);
                var picBox = existing.Controls.Find("picPreview", true).FirstOrDefault() as PictureBox;
                if (picBox != null) picBox.ImageLocation = imagePath;
                existing.Activate();
                return;
            }

            // 1. 创建大容器窗体
            Form container = new Form
            {
                Name = "OCRContainer",
                Text = $"OCR 助手 - 正在使用 {selectedSite}",
                Size = Screen.PrimaryScreen.WorkingArea.Size,
                WindowState = FormWindowState.Maximized
            };

            // 2. 使用 SplitContainer 实现看图
            SplitContainer split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = container.Width / 4, // 左侧占据 1/4
                FixedPanel = FixedPanel.Panel1
            };
            container.Controls.Add(split);

            // 3. 左侧：放置截图预览
            PictureBox picPreview = new PictureBox
            {
                Name = "picPreview",
                ImageLocation = imagePath,
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill,
                BackColor = Color.DarkGray
            };

            // --- 新增：实现拖拽功能 ---
            // 在事件前先保存局部副本


            picPreview.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    // 关键：不要用变量，直接从 picPreview 控件里实时获取当前的图片路径
                    var pb = s as PictureBox;
                    if (pb != null && !string.IsNullOrEmpty(pb.ImageLocation))
                    {
                        string[] files = new string[] { pb.ImageLocation };
                        DataObject data = new DataObject(DataFormats.FileDrop, files);
                        pb.DoDragDrop(data, DragDropEffects.Copy);
                    }
                }
            };

            // 点击依然保留复制路径的功能，双重保险
            // 1. 右键点击：复制路径
            picPreview.Click += (s, e) =>
            {
                var path = (s as PictureBox)?.ImageLocation;
                if (!string.IsNullOrEmpty(path)) Clipboard.SetText(path);
            };

            split.Panel1.Controls.Add(picPreview);
            // 点击图片再次复制路径（防丢）
            picPreview.Click += (s, e) => Clipboard.SetText(imagePath);
            split.Panel1.Controls.Add(picPreview);

            // 4. 右侧：放置 WebView2
            var webView = new Microsoft.Web.WebView2.WinForms.WebView2 { Dock = DockStyle.Fill };
            split.Panel2.Controls.Add(webView);

            webView.CoreWebView2InitializationCompleted += (s, e) =>
            {
                webView.CoreWebView2.Navigate(targetUrl);
                Clipboard.SetText(imagePath);
            };

            webView.EnsureCoreWebView2Async();
            container.Show();
        }
        #endregion



        #region 界面美化(记得在Load中引用)
        private int radius = 20;
        private void BeautifyUI()
        {
            //Parampanel2_Resize(null, null);
            //LabelViewpanel_Resize(null, null);
        }
        private bool isDarkMode = false;

        private void DarkorWhiteMode_Click(object sender, EventArgs e)
        {
            // 一键切换
            ThemeManager.ToggleTheme(this);
            // 关键：重新调用一次 SetMode，刷新当前处于激活状态按钮的颜色
            if (_currentDownAction == DoOCRMouseDown) SetMode("OCR");
            else if (_currentDownAction == DoTextReviewMouseDown) SetMode("TextReview");
            else SetMode("LabelMode");
            TextBox_FocusChanged(LabelTextBox, null); TextBox_FocusChanged(RemarktextBox, null);
            PicView.Invalidate();
        }
        #endregion


    }

}
