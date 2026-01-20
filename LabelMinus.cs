using LabelMinus;
using LabelMinus.LabelMinus;
using mylabel.Modules;
using OpenTK.Platform.Windows;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static mylabel.Modules.Modules;
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

        private UndoManager _undoManager = new();
        private Dictionary<string, ImageInfo> imageDatabase = new();


        private ImageInfo? CurrentInfo => PicNameBindingSource.Current as ImageInfo;// 快捷获取当前图片
        private ImageLabel? CurrentSelectedLabel => imageLabelBindingSource.Current as ImageLabel;// 快捷获取当前选中的标注

        private Image? image;          // 当前显示的图片
        private float scale = 1f;     // 缩放比例
        private PointF offset = new(0, 0); // 平移偏移
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


        #region 窗口加载关闭要做的
        public LabelMinusForm()
        {
            InitializeComponent();
            Modules.UIHelper.BindFocusTransfer(this, PicView);
        }
        private void LabelMinus_Load(object sender, EventArgs e)
        {
            this.ActiveControl = PicView;
            ClearTempFolders();// 启动清理
            LoadSystemFonts(FontstylecomboBox);
            InitFontSizeComboBox();
            InitOCRComboBox();
            BeautifyUI();
            ThemeManager.ApplyTheme(this);
            SetMode("LabelMode");
            EnableDoubleBuffer(LabelView);
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
                    // 执行保存逻辑
                    DoSave(_currentProject.TxtPath);
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

            InstalledFontCollection fonts = new();
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
        private readonly Dictionary<string, Font> _fontCache = new();

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

        private void ClearTempFolders()
        {
            // 定义需要清理的文件夹列表
            string[] tempFolders = { "OCRtemp", "ScreenShottemp", "Archivetemp" };

            foreach (string folderName in tempFolders)
            {
                try
                {
                    string folderPath = Path.Combine(Application.StartupPath, folderName);

                    if (Directory.Exists(folderPath))
                    {
                        // 使用递归删除：直接把文件夹及其内容全部抹掉
                        // 第二个参数 true 是关键，它会删除所有子文件夹和文件
                        Directory.Delete(folderPath, true);
                    }

                    // 重新创建一个干净的文件夹
                    Directory.CreateDirectory(folderPath);
                }
                catch (Exception ex)
                {
                    // 记录日志或静默处理
                    Console.WriteLine($"清空文件夹 {folderName} 失败: {ex.Message}");
                }
            }
        }

        public static void EnableDoubleBuffer(DataGridView dgv)
        {
            // 利用反射获取并设置该受保护的属性
            typeof(DataGridView).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(dgv, true, null);
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
            "ctrl+Z/ctrl+Y  :  撤销/重做对上一个标记的改动\n";
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
                    (groupName) => GetColorForGroup(groupName),
                    CurrentViewConfig.showIndex, // 直接从元组取值
                    CurrentViewConfig.showText,
                    CurrentViewConfig.showGroup

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
                using var maskRegion = new Region(PicView.ClientRectangle);
                maskRegion.Exclude(ocrRect);// 关键点：从 Region 中减去当前的截图矩形，形成一个“带洞的蒙版”
                using var overlay = new SolidBrush(Color.FromArgb(120, 0, 0, 0));//同样应用简化，作用域到方法结束自动销毁
                e.Graphics.FillRegion(overlay, maskRegion);

                // 3. 画截图框的虚线边框
                using (Pen p = new Pen(Color.Red, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
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
                //e.Graphics.DrawString("● TextReview Mode", this.Font, Brushes.Green, 10, 10);

            }

        }
        private void DrawEmptyStatePrompt(Graphics g)
        {
            g.Clear(Modules.ThemeManager.PicViewBg);

            using (Font titleFont = new Font("微软雅黑", 14, FontStyle.Bold))
            using (StringFormat sf = new StringFormat())
            // 1. 创建画刷
            using (Brush textBrush = new SolidBrush(Color.Gray))
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;

                // 2. 传入画刷对象
                g.DrawString(prompt, titleFont, textBrush, PicView.ClientRectangle, sf);
            }
        }
        #endregion

        #region PicView事件

        private Point mouseDownLocation;
        private Point _rightClickStartPoint;
        private bool isDragging = false;
        private bool isPotentialClick = false; // 用来标记是否可能是点击
        private bool isXLocked = false; // 是否锁定横向（左右）
        private bool isYLocked = false; // 是否锁定竖直（上下）
        private const int ClickThreshold = 5; // 像素阈值


        private ImageLabel? _pendingLabel = null;
        private ImageLabel? _draggingLabel = null;
        private Point ocrStartPoint;         // 截图起点
        private Rectangle ocrRect;           // 当前拉框的矩形区域（屏幕坐标）
        private bool isSelectingRect = false; // 是否正在拉框

        private Action<MouseEventArgs> _currentDownAction;
        private Action<MouseEventArgs> _currentMoveAction;
        private Action<MouseEventArgs> _currentUpAction;
        private (bool showIndex, bool showText, bool showGroup) CurrentViewConfig
            => (true, PicViewText.Checked, PicViewGroup.Checked);
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
                    PicViewText.Checked = false;
                    PicViewGroup.Checked = false;
                    break;
                case "TextReview":
                    BindActions(DoTextReviewMouseDown, DoTextReviewMouseMove, DoTextReviewMouseUp);
                    PicViewText.Checked = false;
                    PicViewGroup.Checked = true;
                    break;
                case "LabelMode": // 显式定义你的默认打点模式
                default:
                    BindActions(DoDefaultMouseDown, DoDefaultMouseMove, DoDefaultMouseUp);
                    PicViewText.Checked = false;
                    PicViewGroup.Checked = true;
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
            Color defaultBg = isDark ? Color.FromArgb(60, 60, 60) : Color.OldLace;
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
        private ImageLabel? _showingLabel = null; // 当前正在显示的标签对象
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
            if (_pendingLabel == null) return;
            // 1. 同步索引
            int index = imageLabelBindingSource.IndexOf(_pendingLabel);
            if (index >= 0 && imageLabelBindingSource.Position != index)
            {
                imageLabelBindingSource.Position = index;
            }

            // 2. 更新当前显示的标记
            _showingLabel = _pendingLabel;

            // 3. 聚焦逻辑
            if (!LabelTextBox.Focused) LabelTextBox.Focus();

            // 4. 显示 ToolTip 逻辑：即便文本为空，也建议显示一个“空”或者干脆不显
            // 但必须确保有文本时一定显示
            if (!string.IsNullOrEmpty(_pendingLabel.Text))
            {
                TextReviewtoolTip.Show(_pendingLabel.Text, PicView,
                    PicView.PointToClient(Cursor.Position).X + 15,
                    PicView.PointToClient(Cursor.Position).Y + 15,
                    30000); // 增加显示时长
            }
        }

        // 计算两点间距离的平方（比直接算距离快，因为不需要开方）
        private double GetDistanceSq(Point p1, Point p2) => Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2);

        // 处理图片平移的通用逻辑
        private void MoveImageOffset(Point currentPos)
        {
            // 计算本次移动的增量
            float deltaX = currentPos.X - mouseDownLocation.X;
            float deltaY = currentPos.Y - mouseDownLocation.Y;

            // 根据锁定状态应用偏移
            if (!isXLocked) // 只有未锁定时，才改变横向偏移
            {
                offset.X += deltaX;
            }

            if (!isYLocked) // 只有未锁定时，才改变竖直偏移
            {
                offset.Y += deltaY;
            }

            // 无论是否锁定，都要更新鼠标位置起点，否则增量会不断累积导致“瞬移”
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
                        Position = new BoundingBox { X = imgPt.X / image.Width, Y = imgPt.Y / image.Height },
                        Group = _currentSelectedGroup // 默认分组
                    };
                    var cmd = new AddDeleteCommand(CurrentInfo, label, true, ApplyFilter);
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

                //PointF cornerPt = ScreenToImage(new Point(ocrRect.Right, ocrRect.Top));

                //var ocrLabel = new ImageLabel
                //{
                //    X = cornerPt.X,
                //    Y = cornerPt.Y,
                //    Group = _currentSelectedGroup, // 默认分组
                //    Text = ""      // 等待 OCR 返回或手动输入
                //};
                //var cmd = new AddDeleteCommand(CurrentInfo, ocrLabel, true, ApplyFilter);
                //_undoManager.Execute(cmd);

                //imageLabelBindingSource.Position = imageLabelBindingSource.IndexOf(ocrLabel);
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
            float thresholdSq = threshold * threshold;

            var targetLabel = CurrentInfo.Labels.FirstOrDefault(l =>
            {
                float lpX = (float)(l.Position.X * image.Width);
                float lpY = (float)(l.Position.Y * image.Height);

                float dx = lpX - imgX;
                float dy = lpY - imgY;
                return (dx * dx + dy * dy) < thresholdSq;
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
                var cmd = new AddDeleteCommand(CurrentInfo, targetLabel, false, ApplyFilter);
                _undoManager.Execute(cmd);
                PicView.Invalidate();
            }
        }
        #endregion
        private void PicView_MouseWheel(object sender, MouseEventArgs e)
        {
            // 1. 如果两个都锁定了，直接不动，退出函数
            if (isXLocked && isYLocked)
            {
                return;
            }
            // 基础移动步长（可以根据需要调整，e.Delta 通常是 120 的倍数）
            float scrollStep = e.Delta / 2;

            // 情况 A：如果 X 或 Y 被锁定，执行平移逻辑
            if (isXLocked || isYLocked)
            {
                if (isXLocked)
                {
                    // X 轴锁定：滚轮控制上下平移 (Y轴移动)
                    offset.Y += scrollStep;
                }

                if (isYLocked)
                {
                    // Y 轴锁定：滚轮控制左右平移 (X轴移动)
                    offset.X += scrollStep;
                }
            }
            // 情况 B：都没有锁定，执行原本的缩放逻辑
            else
            {
                float oldScale = scale;
                if (e.Delta > 0) scale *= 1.1f;
                else scale /= 1.1f;

                // 缩放围绕鼠标位置
                offset.X = e.X - (e.X - offset.X) * (scale / oldScale);
                offset.Y = e.Y - (e.Y - offset.Y) * (scale / oldScale);
            }

            PicView.Invalidate();
        }
        #endregion







        #region 导出栏
        private void ExportOriginal_Click(object sender, EventArgs e)
        {
            DoExport("导出原翻译", ExportMode.Original);
        }

        private void ExportCurrent_Click(object sender, EventArgs e)
        {
            DoExport("导出新翻译", ExportMode.Current);
        }

        private void ExportDiff_Click(object sender, EventArgs e)
        {
            DoExport("导出修改文档", ExportMode.Diff);
        }
        private void DoExport(string title, ExportMode mode)
        {
            if (imageDatabase == null || imageDatabase.Count == 0) return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "文本文件|*.txt";
                sfd.Title = title;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string output = mylabel.Modules.Modules.LabelsToText(this.imageDatabase, _currentProject.ZipName, mode);
                        File.WriteAllText(sfd.FileName, output, Encoding.UTF8);
                        ShowMsg($"{title}成功！");
                    }
                    catch (Exception ex)
                    {
                        ShowMsg($"导出失败: {ex.Message}", true);
                    }
                }
            }
        }
        #endregion

        #region 修改栏
        private void ModifyGroup_Click(object sender, EventArgs e)
        {
            // 弹出管理对话框，并获取用户最后选中的那个组
            ShowGroupManagementDialog(_availableGroups);

            // 刷新主界面上的 ComboBox 或列表
            BindGroups(_availableGroups);
        }

        private void ShowGroupManagementDialog(List<string> availableGroups)
        {
            using (Form form = new Form())
            {
                form.Text = "管理分组";
                form.Size = new Size(300, 320);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;

                // 1. 顶部提示
                Label lbl = new() { Text = "当前所有分组列表：", Dock = DockStyle.Top, Height = 30, TextAlign = ContentAlignment.MiddleLeft };

                // 2. 纯列表显示
                ListBox lb = new() { Dock = DockStyle.Top, Height = 200, Font = new Font("微软雅黑", 10) };
                foreach (var g in availableGroups) lb.Items.Add(g);

                // 3. 按钮区
                Button btnAdd = new Button { Text = "添加新组", Left = 55, Top = 230, Width = 80, Height = 30 };
                btnAdd.Click += (s, e) =>
                {
                    string newGroup = Microsoft.VisualBasic.Interaction.InputBox("请输入新分组名称：", "添加分组", "");
                    if (!string.IsNullOrWhiteSpace(newGroup) && !availableGroups.Contains(newGroup))
                    {
                        availableGroups.Add(newGroup);
                        lb.Items.Add(newGroup);
                    }
                };

                Button btnDel = new Button { Text = "删除选中", Left = 150, Top = 230, Width = 80, Height = 30 };
                btnDel.Click += (s, e) =>
                {
                    if (lb.SelectedItem == null) return;
                    string target = lb.SelectedItem.ToString();

                    // 基础分组保护
                    if (target == "框内" || target == "框外")
                    {
                        MessageBox.Show("基础分组不允许删除");
                        return;
                    }

                    if (MessageBox.Show($"确定删除分组 [{target}] 吗？", "确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        availableGroups.Remove(target);
                        lb.Items.Remove(target);

                        // 如果删掉的是主界面正在用的组，重置一下当前选中
                        if (_currentSelectedGroup == target)
                            _currentSelectedGroup = availableGroups.FirstOrDefault() ?? "";
                    }
                };


                form.Controls.AddRange(new Control[] { lb, lbl, btnAdd, btnDel });

                // 显示窗口
                form.ShowDialog();

                // 无论如何，退出后都刷新主界面的 UI
                BindGroups(availableGroups);
            }
        }
        private void LabelTextFontsize_Click(object sender, EventArgs e)
        {
            ContextMenuStrip fontMenu = new ContextMenuStrip();

            var increaseBtn = new ToolStripMenuItem("增大字号 (+2)");
            increaseBtn.Click += (s, ev) => ChangeFontSize(2);

            var decreaseBtn = new ToolStripMenuItem("缩小字号 (-2)");
            decreaseBtn.Click += (s, ev) => ChangeFontSize(-2);

            var resetBtn = new ToolStripMenuItem("重置默认");
            resetBtn.Click += (s, ev) =>
            {
                LabelTextBox.Font = new Font(LabelTextBox.Font.FontFamily, 12f);
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



        private void CompressPic_Click(object sender, EventArgs e)//加载压缩图片
        {
            RefreshImageDisplay();
        }
        private void RefreshImageDisplay()
        {
            // 1. 调用你那个“不动”的原版安全加载函数
            LoadImageSafe();

            // 2. 如果加载成功，且用户开启了压缩模式
            if (image != null && CompressPic.Checked)
            {
                // 记录原始尺寸（如果后续坐标换算需要的话）
                // Size originalSize = image.Size; 

                // 调用压缩函数（限制在 2048px）
                Image compressed = GetScaledImage(image, 2048);

                // 释放原图，替换为压缩图
                image.Dispose();
                image = compressed;
            }

            // 3. 无论是否压缩，最后执行重绘
            fittoview();
            PicView.Invalidate();
        }
        private Image GetScaledImage(Image sourceImage, int maxSide)
        {
            if (sourceImage == null) return null;

            // 1. 如果图片本身就比限制尺寸小，直接克隆一份返回，不浪费计算量
            if (sourceImage.Width <= maxSide && sourceImage.Height <= maxSide)
            {
                return new Bitmap(sourceImage);
            }

            // 2. 计算等比例缩小的尺寸
            float scale = Math.Min((float)maxSide / sourceImage.Width, (float)maxSide / sourceImage.Height);
            int newWidth = (int)(sourceImage.Width * scale);
            int newHeight = (int)(sourceImage.Height * scale);

            // 3. 创建画布并重绘
            Bitmap bmp = new(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // 设置高质量压缩选项，防止图片里的字变模糊
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                g.DrawImage(sourceImage, 0, 0, newWidth, newHeight);
            }
            return bmp;
        }
        #endregion


        #region 数据绑定
        private ImageLabel? _snapshotBeforeEdit; // 编辑前的快照
        private ImageLabel? _lastSelectedLabel;  // 上一个选中的对象（结算用）
        private void RefreshUI(bool reloadImage = false)// 统一刷新 UI 的辅助方法
        {
            if (reloadImage) RefreshImageDisplay();

            if (CurrentSelectedLabel != null)
            {
                float curX = CurrentSelectedLabel.X; // 强制触发 getter
                float curY = CurrentSelectedLabel.Y;
                Locationshowlabel.Text = $"X: {curX:F4}\nY: {curY:F4}";
            }
            else
            {
                Locationshowlabel.Text = "X: 0.0000\nY: 0.0000";
            }

            PicView.Invalidate();
        }
        private void UpdateSnapshot()// 提取快照逻辑
        {
            _lastSelectedLabel = CurrentSelectedLabel;
            _snapshotBeforeEdit = CurrentSelectedLabel?.Clone();
        }
        private void imageLabelBindingSource_CurrentItemChanged(object sender, EventArgs e) => RefreshUI();

        private void PicNameBindingSource_CurrentItemChanged(object sender, EventArgs e)
        {
            if (PicNameBindingSource.Current == null)
            {
                imageLabelBindingSource.DataSource = typeof(ImageLabel); // 保持结构，但清空数据
                return;
            }
            CheckAndPushUndoCommand(); // 结算旧数据 
            imageLabelBindingSource.DataSource = CurrentInfo?.ActiveLabels;// 切换数据源        
            UpdateSnapshot();// 初始化新快照（直接调用逻辑一致的 PositionChanged 内容）
            RefreshUI(true);
        }


        private void imageLabelBindingSource_PositionChanged(object sender, EventArgs e)// 选中标签
        {
            CheckAndPushUndoCommand();
            UpdateSnapshot();

            if (CurrentSelectedLabel != null)
                UpdateGroupSelectionUI(CurrentSelectedLabel.Group);
        }
        private void CheckAndPushUndoCommand()
        {
            if (_lastSelectedLabel == null || _snapshotBeforeEdit == null) return;

            bool isChanged = _lastSelectedLabel.Text != _snapshotBeforeEdit.Text ||
                             _lastSelectedLabel.Remark != _snapshotBeforeEdit.Remark ||
                             !_lastSelectedLabel.Position.Equals(_snapshotBeforeEdit.Position);
            if (isChanged)
            {
                _undoManager.PushManual(new LabelStateCommand(
                    _lastSelectedLabel,
                    _snapshotBeforeEdit,
                    _lastSelectedLabel.Clone(),
                    () => PicView.Invalidate()
                ));
            }

            // 重要：结算完旧的，把指针指向“当前正在使用的对象”，作为下一次切换时的“旧对象”
            _lastSelectedLabel = CurrentSelectedLabel;
        }
        // TODO: 之后需要修改这里，适配 7z 和 Rar 的解压逻辑
        private byte[]? GetFileBytesSafe(string path) => File.Exists(path) ? File.ReadAllBytes(path) : null;
        private void LoadImageSafe()// 安全加载图片，防止删除/移动文件时提示“进程被占用”
        {
            image?.Dispose();
            image = null;
            if (CurrentInfo == null) return;

            try
            {
                // 1. 获取字节数组 (通过简洁的判断逻辑)
                byte[]? bytes = _currentProject.IsArchiveMode
                        ? ArchiveHelper.ExtractFileToBytes(_currentProject.ZipPath, CurrentInfo.ImageName)// 压缩包模式
                        : GetFileBytesSafe(Path.Combine(_currentProject.BaseFolderPath, CurrentInfo.ImageName));// 文件夹模式


                // 2. 转换为图片
                if (bytes != null)
                {
                    using var ms = new MemoryStream(bytes);
                    image = Image.FromStream(ms);
                }
            }
            catch { image = null; }
        }

        private void ApplyFilter()// 过滤方法
        {
            // 以后只需这一行，语义更清晰
            imageLabelBindingSource.DataSource = CurrentInfo?.ActiveLabels;
        }

        private void deleteLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentInfo == null || CurrentSelectedLabel == null) return;
            CheckAndPushUndoCommand();
            _lastSelectedLabel = null;
            _snapshotBeforeEdit = null;


            var cmd = new AddDeleteCommand(CurrentInfo, CurrentSelectedLabel, false, ApplyFilter);
            _undoManager.Execute(cmd);
            PicView.Invalidate();
        }
        #endregion





        #region 界面美化(记得在Load中引用)
        private int radius = 20;
        private void BeautifyUI()
        {
        }
        private void DarkorWhiteMode_Click(object sender, EventArgs e)
        {
            // 一键切换
            ThemeManager.ToggleTheme(this);
            // 关键：重新调用一次 SetMode，刷新当前处于激活状态按钮的颜色
            if (_currentDownAction == DoOCRMouseDown) SetMode("OCR");
            else if (_currentDownAction == DoTextReviewMouseDown) SetMode("TextReview");
            else SetMode("LabelMode");
            TextBox_FocusChanged(LabelTextBox, null); TextBox_FocusChanged(RemarktextBox, null);
            UpdateLockButtonUI(LockX, isXLocked);
            UpdateLockButtonUI(LockY, isYLocked);
            PicView.Invalidate();
        }
        #endregion



        #region OCR相关
        private Dictionary<string, string> _ocrSites = new()
            {
                { "识字体网 (LikeFont)", "https://www.likefont.com/" },
                { "AI识别 (YuzuMarker)", "https://huggingface.co/spaces/gyrojeff/YuzuMarker.FontDetection" }
            };
        private void InitOCRComboBox()
        {
            OCRComboBox.Items.Clear();
            foreach (var siteName in _ocrSites.Keys)
            {
                OCRComboBox.Items.Add(siteName);
            }
            if (OCRComboBox.Items.Count > 0) OCRComboBox.SelectedIndex = 0;
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
                using (Bitmap bmp = new((int)width, (int)height))
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
                ShowMsg("裁剪图片失败: " + ex.Message, true);
                return null;
            }
        }
        private void ShowOCRWebPage(string imagePath)
        {
            string selectedSite = OCRComboBox.SelectedItem?.ToString() ?? "";

            // 直接从字典查找 URL，找不到则给个默认值
            if (!_ocrSites.TryGetValue(selectedSite, out string targetUrl))
            {
                targetUrl = "https://www.baidu.com";
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
            Form container = new()
            {
                Name = "OCRContainer",
                Text = $"OCR 助手 - 正在使用 {selectedSite}",
                Size = Screen.PrimaryScreen.WorkingArea.Size,
                WindowState = FormWindowState.Maximized
            };

            // 2. 使用 SplitContainer 实现看图
            SplitContainer split = new()
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = container.Width / 4, // 左侧占据 1/4
                FixedPanel = FixedPanel.Panel1
            };
            container.Controls.Add(split);

            // 3. 左侧：放置截图预览
            PictureBox picPreview = new()
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
                        string[] files = [pb.ImageLocation];
                        DataObject data = new(DataFormats.FileDrop, files);
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
        private ContextMenuStrip _ocrMenu;

        private void InitOCRManagementMenu()
        {
            _ocrMenu = new ContextMenuStrip();

            // 添加“新增”菜单项
            var addMenuItem = new ToolStripMenuItem("➕ 添加新网站");
            addMenuItem.Click += btnAddOCRSite_Click;

            // 添加“删除”菜单项
            var delMenuItem = new ToolStripMenuItem("❌ 删除当前选中网站");
            delMenuItem.Click += btnDeleteOCRSite_Click;

            _ocrMenu.Items.Add(addMenuItem);
            _ocrMenu.Items.Add(new ToolStripSeparator()); // 分割线
            _ocrMenu.Items.Add(delMenuItem);
        }
        private void btnAddOCRSite_Click(object sender, EventArgs e)
        {
            // 1. 动态创建一个小型对话框
            using (Form prompt = new())
            {
                prompt.Width = 400;
                prompt.Height = 220;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "添加 OCR 网站";
                prompt.StartPosition = FormStartPosition.CenterParent;

                // 2. 创建控件
                Label lblName = new() { Left = 20, Top = 20, Text = "网站名称:", Width = 350 };
                TextBox txtName = new() { Left = 20, Top = 45, Width = 340, Text = "新网站" };

                Label lblUrl = new() { Left = 20, Top = 80, Text = "网站网址 (URL):", Width = 350 };
                TextBox txtUrl = new() { Left = 20, Top = 105, Width = 340, Text = "https://" };

                Button confirmation = new() { Text = "确定", Left = 160, Width = 100, Top = 145, DialogResult = DialogResult.OK };
                Button cancel = new() { Text = "取消", Left = 270, Width = 90, Top = 145, DialogResult = DialogResult.Cancel };

                prompt.Controls.AddRange([lblName, txtName, lblUrl, txtUrl, confirmation, cancel]);
                prompt.AcceptButton = confirmation; // 按回车键直接确认

                // 3. 显示窗口并处理结果
                if (prompt.ShowDialog() == DialogResult.OK)
                {
                    string name = txtName.Text.Trim();
                    string rawUrl = txtUrl.Text.Trim();

                    // 使用严谨的校验逻辑
                    string validatedUrl = GetValidatedUrl(rawUrl);

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        MessageBox.Show("名称不能为空！");
                        return;
                    }

                    if (validatedUrl == null)
                    {
                        MessageBox.Show("网址格式不正确！必须包含有效的域名且以 http/https 开头。");
                        return;
                    }

                    // 保存并更新 UI
                    _ocrSites[name] = validatedUrl;
                    InitOCRComboBox();
                    OCRComboBox.SelectedItem = name;
                    //SaveOCRSites(); // 保存到本地配置
                }
            }
        }

        // 辅助方法：严谨的 URL 校验与修复
        private string GetValidatedUrl(string inputUrl)
        {
            if (string.IsNullOrWhiteSpace(inputUrl)) return null;
            string target = inputUrl.Trim();

            // 自动补全
            if (!target.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !target.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                target = "https://" + target;
            }

            if (Uri.TryCreate(target, UriKind.Absolute, out Uri validatedUri) &&
               (validatedUri.Scheme == Uri.UriSchemeHttp || validatedUri.Scheme == Uri.UriSchemeHttps))
            {
                if (validatedUri.Host.Contains(".")) return validatedUri.AbsoluteUri;
            }
            return null;
        }
        private void btnDeleteOCRSite_Click(object sender, EventArgs e)
        {
            string selected = OCRComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selected)) return;

            var result = MessageBox.Show($"确定要删除 [{selected}] 吗？", "提示", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                _ocrSites.Remove(selected);
                InitOCRComboBox();
            }
        }
        private void ChangeOCRWeb_Click(object sender, EventArgs e)
        {
            // 1. 确保菜单已初始化
            if (_ocrMenu == null) InitOCRManagementMenu();

            // 2. 动态判断“删除”项是否可用（通过文本查找比索引安全）
            foreach (ToolStripItem item in _ocrMenu.Items)
            {
                if (item.Text.Contains("删除"))
                {
                    item.Enabled = OCRComboBox.SelectedItem != null;
                    break;
                }
            }

            // 3. 处理 ToolStripItem 类型的弹出位置
            if (sender is ToolStripItem toolItem)
            {
                // ToolStripItem 没有 .Height 属性，但有 .Bounds 属性
                // 弹出在按钮的左下角
                _ocrMenu.Show(toolItem.Owner, new Point(toolItem.Bounds.Left, toolItem.Bounds.Bottom));
            }

        }
        #endregion
        #region Group相关
        private string _currentSelectedGroup = "框内";
        private bool _isUpdatingUIFromCode = false; // 防止死循环的锁
        // 1. 定义一个私有列表存储数据
        private List<string> _availableGroups = new List<string>();
        #region 颜色分配逻辑
        // 1. 定义预设的鲜艳颜色池（按优先级排列）
        private readonly Color[] _presetColors = new Color[]
                {
            Color.Red,          // 1. 红 (框内)
            Color.Blue,         // 2. 蓝 (框外)
            Color.LimeGreen,    // 4. 绿
            Color.DarkOrange,   // 5. 橙
            Color.DeepPink,     // 6. 粉
            Color.Cyan          // 8. 青
                };

        // 用于存放已分配的颜色映射
        private Dictionary<string, Color> _groupColors = new Dictionary<string, Color>();

        private Color GetColorForGroup(string groupName)
        {
            // 如果字典里已经有了（无论是预设还是随机生成的），直接返回
            if (_groupColors.ContainsKey(groupName))
                return _groupColors[groupName];

            // 如果还没有分配颜色：
            // 情况 A: 如果当前已分配的数量还没超过预设池的大小，取下一个预设颜色
            if (_groupColors.Count < _presetColors.Length)
            {
                Color preset = _presetColors[_groupColors.Count];
                _groupColors[groupName] = preset;
                return preset;
            }

            // 情况 B: 超过 8 个，开始随机生成（使用 HashCode 保证固定）
            Random rnd = new Random(groupName.GetHashCode());
            // 限制在 0-180 之间，确保颜色不会太浅看不清
            Color newColor = Color.FromArgb(rnd.Next(180), rnd.Next(180), rnd.Next(180));
            _groupColors[groupName] = newColor;
            return newColor;
        }
        #endregion
        // 2. 封装刷新逻辑
        public void BindGroups(List<string> groups)
        {
            _availableGroups = groups;
            _isUpdatingUIFromCode = true;

            flowGroups.SuspendLayout(); // 暂停布局，防止大量控件添加时闪烁
            flowGroups.Controls.Clear();

            foreach (var groupName in _availableGroups)
            {
                // 获取该组对应的颜色（前几个固定，后面的动态生成）
                Color themeColor = GetColorForGroup(groupName);

                RadioButton rb = new RadioButton
                {
                    Text = groupName,
                    Tag = groupName,
                    AutoSize = true,
                    MinimumSize = new Size(40, 30), // 稍微给个最小尺寸更美观
                    Margin = new Padding(5),
                    Checked = (groupName == _currentSelectedGroup),

                    Appearance = Appearance.Button,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent, // 无背景                                                 
                    ForeColor = themeColor,// 设置文字颜色为组别颜色
                    // 字体加粗或正常由状态决定
                    Font = new Font("Microsoft YaHei", 10f, (groupName == _currentSelectedGroup) ? FontStyle.Bold : FontStyle.Regular)
                };
                // 彻底隐藏边框
                rb.FlatAppearance.BorderSize = 0;
                rb.FlatAppearance.CheckedBackColor = Color.Transparent;
                rb.FlatAppearance.MouseDownBackColor = Color.Transparent;
                rb.FlatAppearance.MouseOverBackColor = Color.FromArgb(20, themeColor); // 鼠标悬停时仅显示极淡的底色提示

                // 3. 事件逻辑
                // --- 核心：重绘逻辑 (画底线) ---
                rb.Paint += (s, e) =>
                {
                    RadioButton btn = (RadioButton)s;
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // 1. 设置文字颜色：选中的用原色，未选中的变半透明/灰色
                    Color textColor = btn.Checked ? themeColor : Color.FromArgb(120, themeColor);
                    TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                    // 2. 如果选中，在下方画一条厚实的横线
                    if (btn.Checked)
                    {
                        int lineThickness = 3; // 横线厚度
                        int linePadding = 4;   // 距离左右边缘的缩进
                        using (Pen p = new Pen(themeColor, lineThickness))
                        {
                            // 计算横线位置：位于控件最底部
                            int y = btn.Height - 2;
                            e.Graphics.DrawLine(p, linePadding, y, btn.Width - linePadding, y);
                        }
                    }
                };

                rb.CheckedChanged += (s, e) =>
                {
                    // 切换状态时改变字体（可选，配合横线效果更好）
                    rb.Font = new Font("Microsoft YaHei", 10f, rb.Checked ? FontStyle.Bold : FontStyle.Regular);

                    if (_isUpdatingUIFromCode) return;

                    if (rb.Checked)
                    {
                        _currentSelectedGroup = rb.Tag.ToString();
                        flowGroups.Invalidate(true); // 必须重绘整个容器，让旧按钮的线消失

                        if (imageLabelBindingSource.Current is ImageLabel label)
                        {
                            label.Group = rb.Text;
                            PicView.Invalidate();
                            imageLabelBindingSource.ResetCurrentItem();
                        }
                    }
                };

                flowGroups.Controls.Add(rb);
            }
            flowGroups.ResumeLayout();
            _isUpdatingUIFromCode = false; // 释放锁
        }


        private void UpdateGroupSelectionUI(string groupName)// 辅助方法：根据标签的组别，自动选中对应的 RadioButton
        {
            _isUpdatingUIFromCode = true; // 开启锁
            try
            {
                foreach (Control ctrl in flowGroups.Controls)
                {
                    if (ctrl is RadioButton rb)
                    {
                        // 如果 RadioButton 的文本匹配当前标签的组名，则选中它
                        rb.Checked = (rb.Text == groupName);
                    }
                }
                // 更新当前记录的选中组变量
                _currentSelectedGroup = groupName;
            }
            finally
            {
                _isUpdatingUIFromCode = false; // 释放锁
            }
        }

        #endregion



        #region 文件处理相关
        private ProjectContext? _currentProject = ProjectContext.Empty;
        private void UpdateContext(string baseFolderPath, string? txtName, string? zipName)
        {
            _currentProject = ProjectContext.Create(baseFolderPath, txtName, zipName);
            this.Text = _currentProject.DisplayTitle;// UI 更新
        }
        private void SetupProject(IEnumerable<string> imageNames, string baseFolder, string? txtName, string? zipName, bool saveImmediately)
        {
            // 1. 统一通过 UpdateContext 管理路径和状态
            UpdateContext(baseFolder, txtName, zipName);

            // 2. 转换模型并刷新 UI 
            var infos = imageNames.Select(name => new ImageInfo { ImageName = name });
            RefreshImageDatabaseUI(infos);

            // 3. 撤销管理器状态重置
            _undoManager?.Clear();
            _undoManager?.MarkAsSaved();

            // 4. 根据需求执行初始保存
            // 注意：只有在非预览模式（txtName 不为空）且要求立即保存时才执行
            if (saveImmediately && !string.IsNullOrEmpty(_currentProject.TxtName))
            {
                DoSave(_currentProject.TxtPath);
                ShowMsg($"项目创建成功！识别到 {imageDatabase.Count} 张图片。");
            }
            else
            {
                // 预览模式或未指定保存文件
                ShowMsg($"预览模式已开启，共有 {imageDatabase.Count} 张图片。");
            }
        }
        private void RefreshImageDatabaseUI(IEnumerable<ImageInfo> imageNames)//负责处理字典转列表、绑定数据源和重置状态。
        {
            // 转换并更新字典与数据源
            var list = imageNames?.ToList() ?? new List<ImageInfo>();// 安全转换列表（防止 source 为 null）
            imageDatabase = list.GroupBy(x => x.ImageName)
                                    .ToDictionary(g => g.Key, g => g.First());
            PicNameBindingSource.DataSource = list;


            UpdateGroupsFromSource(list);// 提取所有组别并更新 GroupPanel
            // 统一处理 UI 复位
            image = null; // 清除当前缓存图
            bool hasData = list.Count > 0;
            if (hasData) { PicNameBindingSource.Position = 0; RefreshImageDisplay(); fittoview(); }
            else PicView.Invalidate();
        }
        #region 新建或预览
        private IEnumerable<string> GetFolderImages(string folderPath)// 文件夹模式通用提取逻辑
        {
            return Directory.EnumerateFiles(folderPath)
                .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()))
                .Select(Path.GetFileName)
                .Cast<string>();
        }
        // 按钮：新建翻译 (保存)
        private void NewTranslation_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog { Filter = "文本文件|*.txt" };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            string folder = Path.GetDirectoryName(sfd.FileName)!;
            string fileName = Path.GetFileName(sfd.FileName);

            var imageNames = GetFolderImages(folder);

            // 参数：图片列表, 基准目录, 翻译文件名, 压缩包名(null), 是否立即保存(true)
            SetupProject(imageNames, folder, fileName, null, true);
        }

        // 按钮：预览文件夹图片 (不保存)
        private void OpenFolderPicture_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog { Description = "选择要预览图片的文件夹" };
            if (fbd.ShowDialog() != DialogResult.OK) return;

            string folder = fbd.SelectedPath;
            var imageNames = GetFolderImages(folder);

            // 参数：图片列表, 基准目录, 翻译文件名(null), 压缩包名(null), 是否立即保存(false)
            SetupProject(imageNames, folder, null, null, false);
        }
        private (IEnumerable<string> names, string zipName) GetZipImages(string zipPath)// 压缩包模式通用提取逻辑
        {
            // 提取包内图片列表
            var names = ArchiveHelper.GetImageEntries(zipPath);
            // 获取压缩包文件名（例如 "data.zip"）
            string zipName = Path.GetFileName(zipPath);
            return (names, zipName);
        }
        // 按钮：新建压缩包翻译 (保存)
        private void NewZipTranslation_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "压缩文件|*.zip;*.7z;*.rar", Title = "选择压缩包创建翻译项目" };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            // 1. 获取基础文件夹路径
            string folder = Path.GetDirectoryName(ofd.FileName)!;

            // 2. 提取压缩包内图片和文件名
            var data = GetZipImages(ofd.FileName);

            // 3. 构造默认的翻译文件名 (例如 "data_翻译.txt")
            string defaultTxtName = Path.GetFileNameWithoutExtension(data.zipName) + "_翻译.txt";

            // 4. 调用 SetupProject
            // 参数：图片列表, 基准目录, 翻译文件名, 压缩包名, 是否立即保存(true)
            SetupProject(data.names, folder, defaultTxtName, data.zipName, true);
        }

        // 按钮：预览图片或压缩包 (不保存)
        private void OpenPictureorZip_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "所有支持的格式|*.zip;*.7z;*.rar;*.jpg;*.jpeg;*.png;*.bmp;*.webp",
                Title = "选择图片（支持多选）或压缩包（单个）",
                Multiselect = true // 允许用户按住 Shift 或 Ctrl 多选
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            // 如果用户选了多个文件，检查第一个文件的类型
            string firstFile = ofd.FileName;
            string folder = Path.GetDirectoryName(firstFile)!;
            string ext = Path.GetExtension(firstFile).ToLower();

            if (new[] { ".zip", ".7z", ".rar" }.Contains(ext))
            {
                // 压缩包预览模式
                var data = GetZipImages(firstFile);
                SetupProject(data.names, folder, null, data.zipName, false);
            }
            else
            {
                // 图片多选预览模式
                var selectedNames = ofd.FileNames
                        .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower())) // 只保留图片后缀的文件
                        .Select(Path.GetFileName)
                        .Cast<string>()
                        .ToList(); // 立即求值，方便后续判断
                SetupProject(selectedNames, folder, null, null, false);
            }
        }
        #endregion
        private void OpenTranslation_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "文本文件|*.txt" };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            try
            {
                // 1. 解析文本，获取数据库和 sourceName (比如 "data.zip")
                var db = mylabel.Modules.Modules.ParseTextToLabels(File.ReadAllText(ofd.FileName), out string? ZipName);

                // 2. 路径拆分：获取文件夹基准和 TXT 文件名
                string folder = Path.GetDirectoryName(ofd.FileName) ?? string.Empty;
                string txtName = Path.GetFileName(ofd.FileName);

                // 3. 素材有效性验证 (逻辑与之前一致)
                string fullSourcePath = !string.IsNullOrEmpty(ZipName) ? Path.Combine(folder, ZipName) : "";
                string? finalSourceName = File.Exists(fullSourcePath) ? ZipName : null;// 如果文件不存在，则将 sourceName 设为 null (回归文件夹模式)


                // 4. 获取图片列表 (从解析出来的数据库 Key 中提取)
                var imageNames = db.Values.Select(v => v.ImageName);
                // 5. 状态重置：移除已修改标记，因为这是刚打开的文件
                foreach (var img in db.Values) img.ResetModificationFlags();

                // 6. 统一通过 SetupProject 初始化，但要保留解析出来的 db.Values (包含具体的翻译文本内容)
                SetupProject(imageNames, folder, txtName, finalSourceName, false);
                RefreshImageDatabaseUI(db.Values);

                ShowMsg($"成功加载翻译：{txtName}");
            }
            catch (Exception ex) { ShowMsg("解析失败: " + ex.Message, true); }
        }
        #region 保存逻辑
        private void SaveTranslation_Click(object sender, EventArgs e) => DoSave(_currentProject.TxtPath);
        private void SaveAsTranslation_Click(object sender, EventArgs e) => DoSave(null);

        private void DoSave(string? fullPath)
        {
            if (imageDatabase == null || imageDatabase.Count == 0) { ShowMsg("当前没有任何图片数据，无法保存翻译文件。", true); return; }
            if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
            {
                fullPath = RequestSavePath();
                if (fullPath == null) return; // 用户取消
            }

            try
            {
                string outputText = mylabel.Modules.Modules.LabelsToText(imageDatabase, _currentProject.ZipName);
                File.WriteAllText(fullPath, outputText, Encoding.UTF8);

                // 保存后，拆解绝对路径并更新上下文，更新上下文，从此项目就从“预览”转为“正式”了
                string newFolder = Path.GetDirectoryName(fullPath)!;
                string newTxtName = Path.GetFileName(fullPath);
                UpdateContext(newFolder, newTxtName, _currentProject.ZipName);

                _undoManager.MarkAsSaved();
                ShowMsg($"保存成功：{newTxtName}");
            }
            catch (Exception ex) { ShowMsg($"错误：{ex.Message}", true); }
        }
        private string? RequestSavePath()
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "文本文件|*.txt",
                Title = "设定翻译文件保存位置",
                InitialDirectory = _currentProject.BaseFolderPath,
                // 默认文件名逻辑：优先用压缩包名，否则用“新建翻译”
                FileName = _currentProject.IsArchiveMode
                    ? Path.GetFileNameWithoutExtension(_currentProject.ZipName) + ".txt"
                    : (_currentProject.TxtName ?? "新建翻译.txt")
            };

            return sfd.ShowDialog() == DialogResult.OK ? sfd.FileName : null;
        }
        #endregion
        private void UpdateGroupsFromSource(List<ImageInfo> list)
        {
            // 获取所有图片中出现过的组名，并去重
            var allGroups = list
                .SelectMany(img => img.Labels)
                .Select(l => l.Group)
                .Distinct()
                .OrderBy(g => g == "框内" ? 0 : (g == "框外" ? 1 : 2)) // 保持之前的排序逻辑
                .ThenBy(g => g)
                .ToList();

            // 如果是新文件完全没标签，至少给个默认初始值
            if (allGroups.Count == 0)
            {
                allGroups.Add("框内");
                allGroups.Add("框外");
            }

            // 3. 将结果同步到类级别的成员变量，供右键菜单（添加/删除）使用
            _availableGroups = allGroups;

            // 4. 设置初始选中项：
            // 如果当前没有选中项，或者之前的选中项在新列表中不存在，则默认选第一个
            if (string.IsNullOrEmpty(_currentSelectedGroup) || !_availableGroups.Contains(_currentSelectedGroup))
            {
                _currentSelectedGroup = _availableGroups.FirstOrDefault() ?? "";
            }

            // 5. 调用“伪绑定”方法生成 UI 控件
            BindGroups(_availableGroups);
        }
        private void OpenNowFolder_Click(object sender, EventArgs e)
        {
            // 1. 检查当前是否已经打开了翻译文件或确定了文件夹
            if (string.IsNullOrEmpty(_currentProject.BaseFolderPath) || !Directory.Exists(_currentProject.BaseFolderPath))
            {
                ShowMsg("当前未定位到有效文件夹，请先打开或新建翻译文件。", true);
                return;
            }

            try
            {
                // 2. 调用资源管理器打开文件夹
                // 使用 ProcessStartInfo 确保在不同系统环境下的兼容性
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _currentProject.BaseFolderPath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                ShowMsg($"无法打开文件夹：{ex.Message}");
            }
        }




        #region 修改图片集
        private void Changeimageinfo_Click(object sender, EventArgs e)
        {
            // 1. 检查前提条件
            if (imageDatabase == null || imageDatabase.Count == 0)
            {
                ShowMsg("请先选择图片文件夹或压缩包！", true);
                return;
            }

            List<string> sourceFiles;

            // 2. 根据模式获取可用的图片列表
            if (_currentProject.IsArchiveMode)
            {
                // --- 压缩包模式 ---
                // 从压缩包内获取所有合规的图片 Entry 列表
                sourceFiles = ArchiveHelper.GetImageEntries(_currentProject.ZipPath).OrderBy(n => n).ToList();
            }
            else
            {
                // --- 文件夹模式 ---
                if (!Directory.Exists(_currentProject.BaseFolderPath)) return;
                sourceFiles = Directory.EnumerateFiles(_currentProject.BaseFolderPath)
                    .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .Select(f => Path.GetFileName(f)!) // 这里的 ! 告诉编译器：别担心，它不是 null
                    .OrderBy(n => n)
                    .ToList();
            }

            if (sourceFiles == null || sourceFiles.Count == 0)
            {
                ShowMsg("素材源中没有找到支持的图片。", true);
                return;
            }

            // 3. 弹出多选框，并预勾选【已经在数据库中】的图片
            // 传入当前数据库中已有的 Key 列表作为预选参考
            List<string> currentlyInDb = imageDatabase.Keys.ToList();
            List<string> selectedFiles = ShowImageSelectionDialog(sourceFiles, currentlyInDb);

            if (selectedFiles == null) return; // 用户点击了取消

            // 4. 更新 imageDatabase
            // A. 移除未选中的
            var keysToRemove = imageDatabase.Keys.Where(k => !selectedFiles.Contains(k)).ToList();
            foreach (var key in keysToRemove) imageDatabase.Remove(key);

            // B. 添加新选中的
            foreach (var fileName in selectedFiles)
            {
                if (!imageDatabase.ContainsKey(fileName))
                {
                    imageDatabase.Add(fileName, new ImageInfo { ImageName = fileName });
                }
            }

            // 5. 刷新 UI 和 保存文件
            // 这里的 RefreshImageDatabaseUI 需要能接受 Dictionary 或 Values
            RefreshImageDatabaseUI(imageDatabase.Values);

            // 调用你之前的保存逻辑(先不保存)
            //string content = LabelsToText(imageDatabase, ExportMode.Current);
            //File.WriteAllText(currentTranslationPath, content, System.Text.Encoding.UTF8);
            string modeStr = string.IsNullOrEmpty(_currentProject.ZipName) ? "文件夹" : "压缩包";
            ShowMsg($"更新完毕！已从{modeStr}同步图片，当前共 {imageDatabase.Count} 张。");
        }
        private List<string> ShowImageSelectionDialog(List<string> allDiskFiles, List<string> currentlyInDb)
        {
            using (Form form = new())
            {
                string sourceDetail = _currentProject?.ZipName ?? "当前文件夹";
                form.Text = "管理图片：请勾选要保留在翻译文档中的文件";
                form.Size = new Size(450, 600);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;

                Panel topPanel = new() { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
                Label lblSource = new()
                {
                    Text = $"路径：{sourceDetail}",
                    Dock = DockStyle.Fill,
                    ForeColor = Color.Black,
                    Font = new Font(this.Font, FontStyle.Bold)
                };
                topPanel.Controls.Add(lblSource);
                CheckedListBox clb = new() { Dock = DockStyle.Top, Height = 400, CheckOnClick = true };

                // 填充列表并设置勾选状态
                foreach (var file in allDiskFiles)
                {
                    bool isExisting = currentlyInDb.Contains(file);
                    string displayName = isExisting ? file : $"[新发现] {file}";
                    clb.Items.Add(file, isExisting); // 只有原本就在数据库里的才默认勾选
                }
                Panel btnPanel = new() { Dock = DockStyle.Bottom, Height = 60 };
                Button btnAll = new() { Text = "全选", Left = 12, Top = 15, Width = 75 };
                btnAll.Click += (s, e) => { for (int i = 0; i < clb.Items.Count; i++) clb.SetItemChecked(i, true); };

                Button btnNone = new() { Text = "全不选", Left = 95, Top = 15, Width = 75 };
                btnNone.Click += (s, e) => { for (int i = 0; i < clb.Items.Count; i++) clb.SetItemChecked(i, false); };

                Button btnOk = new() { Text = "确定同步", DialogResult = DialogResult.OK, Left = 240, Top = 15, Width = 90, BackColor = Color.LightBlue };
                Button btnCancel = new() { Text = "取消", DialogResult = DialogResult.Cancel, Left = 340, Top = 15, Width = 80 };
                btnPanel.Controls.AddRange(new Control[] { btnAll, btnNone, btnOk, btnCancel });

                form.Controls.Add(btnPanel);
                form.Controls.Add(clb);
                form.Controls.Add(topPanel);
                form.AcceptButton = btnOk;

                if (form.ShowDialog() == DialogResult.OK)
                {
                    return clb.CheckedItems.Cast<string>().ToList();
                }
                return null;
            }
        }
        #endregion
        private void AssociateZip_Click(object sender, EventArgs e)
        {
            // 1. 前提检查：必须先有图片数据才能关联
            if (imageDatabase == null || imageDatabase.Count == 0)
            {
                ShowMsg("当前没有任何图片数据，无法关联素材包。", true);
                return;
            }

            // 2. 让用户选择新的压缩包
            using var ofd = new OpenFileDialog
            {
                Filter = "压缩文件|*.zip;*.7z;*.rar",
                Title = "选择要关联的素材压缩包",
                InitialDirectory = _currentProject.BaseFolderPath
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            // 3. 提取新路径信息
            string newZipPath = ofd.FileName;
            string newFolder = Path.GetDirectoryName(newZipPath)!;
            string newZipName = Path.GetFileName(newZipPath);

            // 4. (核心) 验证压缩包内容与当前数据库的匹配度
            var zipEntries = ArchiveHelper.GetImageEntries(newZipPath).ToList();
            int matchCount = imageDatabase.Keys.Count(k => zipEntries.Contains(k));

            if (matchCount == 0)
            {
                ShowMsg("关联失败：所选压缩包内不包含当前项目中的任何图片文件名！", true);
                return;
            }

            // 5. 更新上下文 (保留当前的 TxtName)
            UpdateContext(newFolder, _currentProject.TxtName, newZipName);

            // 6. 标记项目已修改（因为关联素材变了，需要保存 .txt 头部信息）
            _undoManager?.MarkAsUnsaved();

            ShowMsg($"已关联新压缩包：{newZipName} (匹配到 {matchCount} 张图片)");

            // 7. 强制刷新预览图
            RefreshUI(true);
        }
        #endregion





    }
    public class ProjectContext
    {       
        public string BaseFolderPath { get; set; } = string.Empty;// 显式存储文件夹路径        
        public string TxtName { get; set; } = null;// 翻译文件路径（预览模式下为空）       
        public string? ZipName { get; set; } = null;// 压缩包文件名（文件夹模式下为空）
        public static ProjectContext Empty => new ProjectContext();
        // 静态工厂方法：封装创建逻辑
        public static ProjectContext Create(string baseFolder, string? txtName, string? zipName)
        {
            return new ProjectContext
            {
                BaseFolderPath = baseFolder,
                TxtName = string.IsNullOrWhiteSpace(txtName) ? null : txtName,
                ZipName = string.IsNullOrWhiteSpace(zipName) ? null : zipName
            };
        }
        // 计算属性：翻译文件的文件名
        public string TxtPath => !string.IsNullOrEmpty(TxtName)
                ? Path.Combine(BaseFolderPath, TxtName)
                : string.Empty;

        // 计算属性：判断当前是否为压缩包模式
        public string ZipPath => !string.IsNullOrEmpty(ZipName)
                ? Path.Combine(BaseFolderPath, ZipName)
                : string.Empty;
        public bool IsArchiveMode => !string.IsNullOrEmpty(ZipName);
        // 计算属性：生成标题栏文字
        public string DisplayTitle => $"LabelMinus - {TxtName} [{(IsArchiveMode ? $"关联压缩包:{ZipName}" : "文件夹")}]";
    }

}
