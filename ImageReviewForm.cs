using SharpCompress.Archives;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Data;


namespace mylabel
{
    public partial class ImageReviewForm : Form
    {
        private bool _isKeyDown = false;

        #region 主窗口相关
        // 定义一个变量来保存主窗口的引用
        private LabelMinusForm _mainOwner;

        // 修改构造函数，接收 LabelMinusForm
        public ImageReviewForm(LabelMinusForm owner)
        {
            InitializeComponent();
            this.Name = "ImageReviewForm";
            _mainOwner = owner; // 拿到主窗口的“遥控器”
        }
        #endregion
        private readonly List<string> imageExtensions = [".jpg", ".png", ".bmp", ".jpeg", ".webp"];
        private readonly List<string> archiveExts = [".zip", ".7z", ".rar"];

        private class ViewState
        {
            public SKBitmap Image { get; set; }           // 给 GPU 渲染用 (SkiaSharp)
            public System.Drawing.Image GdiImage { get; set; } // 给截图/合成/保存用 (GDI+)
            public float Scale { get; set; } = 1f;
            public PointF Offset { get; set; } = new PointF(0, 0);
            public string ImagePath { get; set; } = string.Empty;
        }
        private Dictionary<Control, ViewState> _viewStates = new Dictionary<Control, ViewState>();

        private List<string> _leftFolderFiles = [];
        private List<string> _rightFolderFiles = [];
        private string? _leftZipPath = null;
        private string? _rightZipPath = null;
        private string GetPureFileName(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            // 去掉自定义的 [Zip] 前缀
            string cleanPath = path.StartsWith("[Zip]") ? path.Substring(5) : path;
            // 提取文件名（不管它在不在压缩包子文件夹里）
            return Path.GetFileNameWithoutExtension(cleanPath);
        }
        private PointF ScreenToImage(Control pb, Point screenPoint)
        {
            // 如果字典里没有这个控件的状态，直接返回
            if (!_viewStates.ContainsKey(pb)) return PointF.Empty;

            var state = _viewStates[pb];
            // 公式：(当前坐标 - 偏移量) / 缩放比例 = 原始图片像素坐标
            float x = (screenPoint.X - state.Offset.X) / state.Scale;
            float y = (screenPoint.Y - state.Offset.Y) / state.Scale;
            return new PointF(x, y);
        }
        private Point mouseDownLocation;
        private bool isDragging = false;
        private bool isPotentialClick = false; // 用来标记是否可能是点击
        private bool isLinked = true;
        private const int ClickThreshold = 5; // 像素阈值


        private bool isScreenShotMode = false;      // 是否处于截图模式
        private Point ScreenShotStartPoint;         // 截图起点
        private Rectangle ScreenShotRect;           // 当前拉框的矩形区域（屏幕坐标）
        private bool isSelectingRect = false; // 是否正在拉框
        private string currentScreenShotpath = null;

        private Dictionary<Control, List<SKPath>> _viewPaths = new Dictionary<Control, List<SKPath>>();
        private List<SKPath> GetPathsForControl(Control c)
        {
            if (!_viewPaths.ContainsKey(c))
            {
                _viewPaths[c] = new List<SKPath>();
            }
            return _viewPaths[c];
        }

        private SKGLControl PicReview1 = null!;
        private SKGLControl PicReview2 = null!;
        public ImageReviewForm()
        {
            InitializeComponent();

        }

        private void ImageReviewForm_Load(object sender, EventArgs e)
        {
            InitGpuControls();
            // 强制开启 PictureBox 的双缓冲
            var prop = typeof(Control).GetProperty("DoubleBuffered",
                       System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Modules.ThemeManager.ApplyTheme(this);

            InitPreviewPopup();

            UpdateLinkViewUI();
        }
        #region GPU渲染
        private void InitGpuControls()
        {
            // 1. 创建新控件
            PicReview1 = CreateGpuControl(PicReview1_Placeholder);
            PicReview2 = CreateGpuControl(PicReview2_Placeholder);

            RemovePlaceholder(PicReview1_Placeholder);
            RemovePlaceholder(PicReview2_Placeholder);
        }

        private void RemovePlaceholder(PictureBox pb)
        {
            if (pb == null) return;

            // 从父容器（SplitContainer 的 Panel）中移除
            if (pb.Parent != null)
            {
                pb.Parent.Controls.Remove(pb);
            }

            // 彻底销毁对象，释放句柄资源
            pb.Dispose();
        }
        private SKGLControl CreateGpuControl(PictureBox placeholder)
        {
            // 1. 获取 PictureBox 所在的 Panel (SplitContainer 的 Panel1 或 Panel2)
            Control parent = placeholder.Parent;
            if (parent == null) return null;

            var ctrl = new SKGLControl();

            // 2. 必须在移除旧控件前，先复制布局属性
            ctrl.Dock = DockStyle.Fill; // 既然是 Fill 在 SplitContainer 里，直接强制 Fill
            ctrl.Name = "RealPicReview_" + placeholder.Name;

            // 3. 绑定事件
            ctrl.PaintSurface += PicView_PaintSurface;
            ctrl.Click += PicView_Click;
            ctrl.MouseDown += PicView_MouseDown;
            ctrl.MouseMove += PicView_MouseMove;
            ctrl.MouseWheel += PicView_MouseWheel;
            ctrl.MouseUp += PicView_MouseUp;
            ctrl.AllowDrop = true; // 必须开启
            ctrl.DragEnter += Control_DragEnter; // 改变鼠标图标
            bool isLeft = placeholder.Name.ToLower().Contains("1");
            ctrl.DragDrop += (s, e) => HandleDrop(e, isLeft);
            // --- 在这里初始化字典，确保 Key 是新的 SKControl ---
            _viewStates[ctrl] = new ViewState();

            parent.Controls.Remove(placeholder);
            parent.Controls.Add(ctrl);
            ctrl.BringToFront();
            parent.PerformLayout();
            ctrl.Invalidate();
            return ctrl;
        }

        #endregion
        #region 图像绘制
        string rawText = "点击此处选择图片/压缩包\n" +
                        "Q(按住)：截图\n" +
                        "A：上一张\n" +
                        "D：下一张\n" +
                        "R：重置显示\n" +
                        "截图后可红笔进行标记(清空限当次标记)";
        private void PicView_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            //MessageBox.Show("Paint 事件触发了！");
            // 将触发事件的控件识别为 pb
            if (!(sender is Control pb)) return;
            if (!_viewStates.ContainsKey(pb)) return;
            var state = _viewStates[pb];
            SKCanvas canvas = e.Surface.Canvas;
            SKColor backgroundColor = Modules.ThemeManager.IsDarkMode
                ? SKColor.Parse("#2D2D30")
                : SKColors.SeaShell;
            // 2. 清除画布
            canvas.Clear(backgroundColor);

            if (state.Image != null)
            {
                canvas.Save();
                canvas.Translate(state.Offset.X, state.Offset.Y);
                canvas.Scale(state.Scale, state.Scale);

                // 使用高质量抗锯齿，GPU 会处理这些计算，不会卡顿
                using (var paint = new SKPaint { IsAntialias = true, IsDither = true })
                {
                    var sampling = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
                    using var skImage = SKImage.FromBitmap(state.Image);
                    canvas.DrawImage(skImage, 0, 0, sampling, paint);

                }
                canvas.Restore();
            }
            else
            {
                // 1. 创建画笔
                using (var paint = new SKPaint())
                {
                    paint.Color = SKColors.Gray;
                    paint.IsAntialias = true;

                    // 2. 创建字体
                    var typeface = SKTypeface.FromFamilyName("Microsoft YaHei");
                    using (var font = new SKFont(typeface, 24))
                    {
                        // --- 换行逻辑开始 ---

                        // 使用 \n 分隔字符串，或者手动拆分

                        string[] lines = rawText.Split('\n');

                        // 计算总高度，用于整体垂直居中
                        float lineHeight = font.Metrics.Descent - font.Metrics.Ascent; // 字体实际占用高度
                        float spacing = 10; // 行间距
                        float totalHeight = (lines.Length * lineHeight) + ((lines.Length - 1) * spacing);

                        // 计算起始起始 Y 坐标（垂直居中）
                        float startY = (pb.Height - totalHeight) / 2 - font.Metrics.Ascent;

                        for (int i = 0; i < lines.Length; i++)
                        {
                            string line = lines[i];

                            // 计算当前行的宽度以水平居中
                            float lineWidth = font.MeasureText(line);
                            float x = Math.Max(10, (pb.Width - lineWidth) / 2);

                            // 计算当前行的 Y 轴位置
                            float currentY = startY + i * (lineHeight + spacing);

                            // 绘制当前行
                            canvas.DrawText(line, x, currentY, font, paint);
                        }

                        // --- 换行逻辑结束 ---
                    }
                }
            }
            // 绘制截图红框
            if (isScreenShotMode && isSelectingRect)
            {
                using (var p = new SKPaint
                {
                    Color = SKColors.Red,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 2,
                    PathEffect = SKPathEffect.CreateDash(new float[] { 6, 4 }, 0)
                })
                {
                    canvas.DrawRect(new SKRect(ScreenShotRect.Left, ScreenShotRect.Top, ScreenShotRect.Right, ScreenShotRect.Bottom), p);
                }
            }
        }

        #endregion
        #region PicView事件通用逻辑        
        private void PicView_MouseDown(object sender, MouseEventArgs e)
        {
            if (!(sender is Control pb) || !_viewStates.ContainsKey(pb)) return;
            var state = _viewStates[pb];
            if (state.Image == null) return;

            if (e.Button == MouseButtons.Left)
            {
                if (isScreenShotMode)
                {
                    isSelectingRect = true;
                    ScreenShotStartPoint = e.Location;
                    ScreenShotRect = new Rectangle(e.Location, new Size(0, 0));
                }
                else
                {
                    mouseDownLocation = e.Location;
                    isDragging = false;
                    isPotentialClick = true;
                }
            }
        }

        private void PicView_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is Control pb) || !_viewStates.ContainsKey(pb)) return;
            var state = _viewStates[pb];
            if (state.Image == null) return;

            // --- 原有模式 1：截图 ---
            if (isScreenShotMode && isSelectingRect)
            {
                int x = Math.Min(ScreenShotStartPoint.X, e.X);
                int y = Math.Min(ScreenShotStartPoint.Y, e.Y);
                int width = Math.Abs(ScreenShotStartPoint.X - e.X);
                int height = Math.Abs(ScreenShotStartPoint.Y - e.Y);
                ScreenShotRect = new Rectangle(x, y, width, height);
                pb.Invalidate();
                return;
            }

            // --- 原有模式 2 & 3：拖拽 ---
            if (e.Button == MouseButtons.Left && isPotentialClick)
            {
                int dx = e.X - mouseDownLocation.X;
                int dy = e.Y - mouseDownLocation.Y;
                if (Math.Sqrt(dx * dx + dy * dy) > ClickThreshold)
                {
                    isDragging = true;
                    isPotentialClick = false;
                }
            }

            if (isDragging)
            {
                PointF tempOffset = state.Offset;
                tempOffset.X += e.X - mouseDownLocation.X;
                tempOffset.Y += e.Y - mouseDownLocation.Y;
                state.Offset = tempOffset;
                mouseDownLocation = e.Location;
                pb.Invalidate();
            }
        }

        private async void PicView_MouseUp(object sender, MouseEventArgs e)
        {
            if (!(sender is Control currentPic)) return;

            if (e.Button != MouseButtons.Left) return;

            // --- 原有截图/同步逻辑 ---
            if (isScreenShotMode && isSelectingRect)
            {
                isSelectingRect = false;
                if (ScreenShotRect.Width > 5 && ScreenShotRect.Height > 5)
                {
                    currentScreenShotpath = CaptureScreenShotImageAndGetPath(currentPic);
                    PointF cornerPt = ScreenToImage(currentPic, new Point(ScreenShotRect.Right, ScreenShotRect.Top));
                }
            }
            else if (isDragging && isLinked)
            {
                Control otherPb = (currentPic == PicReview1) ? PicReview2 : PicReview1;
                SyncView(currentPic, otherPb);
            }
            isDragging = false;
            isPotentialClick = false;
        }
        private void PicView_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!(sender is Control pb) || !_viewStates.ContainsKey(pb)) return;
            var state = _viewStates[pb];
            if (state.Image == null) return;
            float oldScale = state.Scale;

            if (e.Delta > 0) state.Scale *= 1.1f;
            else state.Scale /= 1.1f;

            // 缩放围绕鼠标位置，使用当前状态的 Offset
            PointF currentOffset = state.Offset;
            currentOffset.X = e.X - (e.X - currentOffset.X) * (state.Scale / oldScale);
            currentOffset.Y = e.Y - (e.Y - currentOffset.Y) * (state.Scale / oldScale);
            state.Offset = currentOffset;

            pb.Invalidate();

            // 处理联动同步逻辑
            if (isLinked)
            {
                Control otherPb = (pb == PicReview1) ? PicReview2 : PicReview1;
                if (otherPb != null && _viewStates.ContainsKey(otherPb) && _viewStates[otherPb].Image != null)
                {
                    SyncView(pb, otherPb);
                }
            }
        }
        private void PicView_Click(object sender, EventArgs e)
        {
            // 1. 确保 sender 是 Control 类型
            if (!(sender is Control pb)) return;
            bool hasNoImage = !_viewStates.ContainsKey(pb) || _viewStates[pb].Image == null;
            if (hasNoImage)
            {
                // 3. 判断是左边还是右边，打开文件夹
                OpenFolderOrArchive(pb == PicReview1);
            }
        }
        #endregion


        #region 顶栏功能

        private void PicNamecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 这里的 SelectedItem 就是我们在 UpdateComboBox 里放入的“纯文件名”
            string selectedName = PicNamecomboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedName)) return;

            // 左右两边各自去自己的 list 里找这个“纯文件名”
            ProcessAndLoad(PicReview1, _leftFolderFiles, _leftZipPath, selectedName, true);
            ProcessAndLoad(PicReview2, _rightFolderFiles, _rightZipPath, selectedName, false);
            this.Focus();           // 尝试给窗体
        }
        /// <summary>
        /// 统一处理加载逻辑：自动识别是磁盘文件还是压缩包条目
        /// </summary>
        private void ProcessAndLoad(Control pb, List<string> fileList, string zipPath, string selectedName, bool isLeft)
        {
            if (fileList == null) { ClearPictureBox(pb); return; }

            // 关键修改：统一提取列表项的“纯文件名”进行比对
            string match = fileList.FirstOrDefault(f => GetPureFileName(f) == selectedName);
            if (!string.IsNullOrEmpty(match))
            {
                // 如果是压缩包路径
                if (match.StartsWith("[Zip]"))
                {
                    // 还原出压缩包内部的真实 Entry Key (带路径的那个)
                    string entryKey = match.Substring(5);
                    LoadFromArchive(pb, zipPath, entryKey, isLeft);
                }
                else
                {
                    // 普通物理路径加载
                    ApplyImageToPictureBox(pb, match);
                }
            }
            else
            {
                // 如果该侧没有同名文件，清空预览
                ClearPictureBox(pb);
            }
        }
        // 辅助方法：清空图片并重置状态
        private void ClearPictureBox(Control pb)
        {
            if (pb == null) return;

            // 检查字典是否初始化（防止在构造函数中过早调用）
            if (_viewStates == null) return;
            // 1. 检查字典中是否有该控件的状态
            if (_viewStates.ContainsKey(pb))
            {
                var state = _viewStates[pb];

                // 2. 释放 GPU 位图资源 (极其重要，防止显存泄漏)
                if (state.Image != null)
                {
                    state.Image.Dispose();
                    state.Image = null;
                }

                // 3. 如果你有 GDI+ 备用图，也一并释放
                if (state.GdiImage != null) { state.GdiImage.Dispose(); state.GdiImage = null; }

                state.ImagePath = string.Empty;
                state.Scale = 1.0f;
                state.Offset = new PointF(0, 0);
            }

            // 4. 触发重绘，PaintSurface 会因为 Image 为 null 而画出提示文字
            pb.Invalidate();
        }
        private void UpdateComboBox()
        {
            PicNamecomboBox.Items.Clear();
            // 1. 处理左侧列表：安全检查并转换
            var leftNames = (_leftFolderFiles ?? new List<string>())
                                .Select(GetPureFileName)
                                .Where(n => !string.IsNullOrEmpty(n));
            // 2. 处理右侧列表：安全检查并转换
            var rightNames = (_rightFolderFiles ?? new List<string>())
                             .Select(GetPureFileName)
                             .Where(n => !string.IsNullOrEmpty(n));
            // 3. 取并集（去重）并排序
            var allNames = leftNames.Union(rightNames)
                                    .OrderBy(n => n)
                                    .ToArray();
            // 4. 添加到界面
            PicNamecomboBox.Items.AddRange(allNames);
        }

        private void LastPic_Click(object sender, EventArgs e)
        {
            if (PicNamecomboBox.Items.Count <= 1) return;

            int currentIndex = PicNamecomboBox.SelectedIndex;

            // 计算上一个索引
            if (currentIndex > 0)
            {
                PicNamecomboBox.SelectedIndex = currentIndex - 1;
            }
            else
            {
                // 如果想循环播放，就跳到最后一张
                PicNamecomboBox.SelectedIndex = PicNamecomboBox.Items.Count - 1;
            }
        }
        private void NextPic_Click(object sender, EventArgs e)
        {
            if (PicNamecomboBox.Items.Count <= 1) return;

            int currentIndex = PicNamecomboBox.SelectedIndex;

            // 计算下一个索引
            if (currentIndex < PicNamecomboBox.Items.Count - 1)
            {
                PicNamecomboBox.SelectedIndex = currentIndex + 1;
            }
            else
            {
                // 如果想循环播放，就回到第一张
                PicNamecomboBox.SelectedIndex = 0;
            }
        }
        private void OpenPicandArc1_Click(object sender, EventArgs e) => OpenFolderOrArchive(true);
        private void OpenPicandArc2_Click(object sender, EventArgs e) => OpenFolderOrArchive(false);
        private void Openfolder1_Click_1(object sender, EventArgs e) => OpenSpecificFolder(true);
        private void Openfolder2_Click_1(object sender, EventArgs e) => OpenSpecificFolder(false);
        private void ScreenShotButton_Click(object sender, EventArgs e)
        {
            SetScreenShotMode(!isScreenShotMode);
        }

        private void ChangePic1_Click(object sender, EventArgs e) => LoadImageToPictureBox(PicReview1);
        private void ChangePic2_Click(object sender, EventArgs e) => LoadImageToPictureBox(PicReview2);
        private void ClearPic_Click(object sender, EventArgs e)
        {
            // 1. 清空后台文件列表数据
            _leftFolderFiles.Clear();
            _rightFolderFiles.Clear();

            // 2. 清空 ComboBox 列表
            PicNamecomboBox.Items.Clear();
            PicNamecomboBox.Text = ""; // 确保显示的文字也被清除

            // 3. 彻底清空左右两个图片框及对应的 ViewState
            ClearPictureBox(PicReview1);
            ClearPictureBox(PicReview2);

            // 4. 给个简单的状态提示
            MessageBox.Show("所有图片已清空");
        }
        #endregion


        #region 底栏功能
        private void FittoReViewButton_Click(object sender, EventArgs e)
        {
            ResetView(PicReview1);
            ResetView(PicReview2);
        }
        private void ImageReviewForm_SizeChanged(object sender, EventArgs e)
        {
            // 防御性检查：如果是最小化，则不处理
            if (this.WindowState == FormWindowState.Minimized) return;

            // 只有当窗口真的有大小时才重置
            if (this.Width > 0 && this.Height > 0)
            {
                // 直接复用你重置按钮的逻辑
                ResetView(PicReview1);
                ResetView(PicReview2);

                // 如果当前是开启了联动模式，重置后确保两边依然同步
                if (isLinked)
                {
                    SyncView(PicReview1, PicReview2);
                }
            }
        }
        private void LinkView_Click(object sender, EventArgs e)
        {
            isLinked = !isLinked; // 切换状态

            UpdateLinkViewUI();   // 更新视觉

            if (isLinked) SyncView(PicReview1, PicReview2);
        }
        private void UpdateLinkViewUI()
        {
            bool isDark = Modules.ThemeManager.IsDarkMode;
            Color activeBg = Modules.ThemeManager.AccentColor;
            Color defaultBg = isDark ? Color.FromArgb(60, 60, 60) : Color.OldLace;
            Color activeFore = isDark ? Color.White : Color.FromArgb(30, 70, 32);
            Color defaultFore = Modules.ThemeManager.TextColor;

            // 根据当前的 isLinked 状态设置颜色
            LinkView.BackColor = isLinked ? activeBg : defaultBg;
            LinkView.ForeColor = isLinked ? activeFore : defaultFore;
        }
        private void ToRight_Click(object sender, EventArgs e) => SyncView(PicReview1, PicReview2);
        private void ToLeft_Click(object sender, EventArgs e) => SyncView(PicReview2, PicReview1);

        private void SyncView(Control source, Control target)
        {
            // 1. 从字典中提取状态进行判定 (Control 没有 .Image)
            if (!_viewStates.ContainsKey(source) || !_viewStates.ContainsKey(target)) return;

            var sState = _viewStates[source];
            var tState = _viewStates[target];

            // 2. 判定两边是否都有图片
            if (sState.Image == null || tState.Image == null) return;

            // 3. 计算图片宽度比例 (使用 SKBitmap 的宽度)
            // 这样即便左边是 1000px，右边是 2000px，也能完美同步位置
            float ratio = (float)tState.Image.Width / sState.Image.Width;

            // 4. 同步缩放和偏移
            tState.Scale = sState.Scale * ratio;
            tState.Offset = new PointF(sState.Offset.X * ratio, sState.Offset.Y * ratio);

            // 5. 通知目标控件重绘
            target.Invalidate();
        }
        private void ScreenShotFolderbutton_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. 获取你的截图保存目录
                string tempDir = Path.Combine(Application.StartupPath, "ScreenShottemp");

                // 2. 检查文件夹是否存在，不存在则创建（防止报错）
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                // 3. 调用资源管理器打开文件夹
                // 使用 Process.Start 直接打开路径
                System.Diagnostics.Process.Start("explorer.exe", tempDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show("无法打开文件夹: " + ex.Message);
            }
        }
        #endregion


        #region 通用函数/加载文件夹、压缩包等
        private void ResetView(Control pb)
        {
            if (!_viewStates.ContainsKey(pb)) return;
            var state = _viewStates[pb];

            // 注意：这里用 state.Image (SKBitmap) 的宽高来计算
            if (state.Image == null) return;

            float scaleX = (float)pb.Width / state.Image.Width;
            float scaleY = (float)pb.Height / state.Image.Height;
            state.Scale = Math.Min(scaleX, scaleY);

            state.Offset = new PointF(
                (pb.Width - state.Image.Width * state.Scale) / 2,
                (pb.Height - state.Image.Height * state.Scale) / 2
            );

            pb.Invalidate(); // 触发重绘
        }
        private void LoadImageToPictureBox(Control pb)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // 确保字典中有这个 Key
                    if (!_viewStates.ContainsKey(pb)) _viewStates[pb] = new ViewState();
                    var state = _viewStates[pb];

                    // 1. 释放旧资源 (针对 SKBitmap)
                    if (state.Image != null)
                    {
                        state.Image.Dispose();
                        state.Image = null;
                    }

                    // 2. 加载新图片 (使用 SkiaSharp 解码)
                    // 注意：不再使用 System.Drawing.Image，直接生成 SKBitmap
                    using (var stream = System.IO.File.OpenRead(ofd.FileName))
                    {
                        state.Image = SKBitmap.Decode(stream);
                    }
                    state.ImagePath = ofd.FileName;

                    // 3. 重置视角并重绘
                    ResetView(pb);
                    pb.Invalidate();
                }
            }
        }

        private void OpenFolderOrArchive(bool isLeft)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                // --- 关键修改：允许用户多选文件 ---
                ofd.Multiselect = true;
                ofd.Filter = "支持的格式|*.zip;*.7z;*.rar;*.jpg;*.png;*.bmp|压缩包|*.zip;*.7z;*.rar|图片|*.jpg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    List<string> finalFiles = new List<string>();
                    string zipPath = null;

                    // 如果用户只选了一个文件，且是压缩包
                    if (ofd.FileNames.Length == 1 && archiveExts.Contains(Path.GetExtension(ofd.FileName).ToLower()))
                    {
                        zipPath = ofd.FileName;
                        finalFiles = LoadEntriesFromZip(zipPath);
                    }
                    else
                    {
                        // 如果用户选了多个文件，或者选了一个普通图片
                        // 我们只加载用户选中的那些图片，而不是整个文件夹
                        zipPath = null;
                        finalFiles = ofd.FileNames
                            .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()))
                            .ToList();
                    }

                    // 赋值到全局变量
                    if (isLeft) { _leftZipPath = zipPath; _leftFolderFiles = finalFiles; }
                    else { _rightZipPath = zipPath; _rightFolderFiles = finalFiles; }

                    // 更新 UI
                    UpdateComboBox();
                    if (PicNamecomboBox.Items.Count > 0) PicNamecomboBox.SelectedIndex = 0;
                }
            }
        }
        private void OpenSpecificFolder(bool isLeft)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "选择包含图片的文件夹";
                fbd.ShowNewFolderButton = false;

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = fbd.SelectedPath;

                    // 获取文件夹下所有支持的图片格式
                    List<string> finalFiles = Directory.GetFiles(folderPath)
                        .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()))
                        .OrderBy(f => f) // 排序一下，通常按文件名排序比较自然
                        .ToList();

                    if (finalFiles.Count == 0)
                    {
                        MessageBox.Show("该文件夹内没有找到支持的图片。");
                        return;
                    }

                    // 赋值到全局变量（文件夹模式下 zipPath 设为 null）
                    if (isLeft)
                    {
                        _leftZipPath = null;
                        _leftFolderFiles = finalFiles;
                    }
                    else
                    {
                        _rightZipPath = null;
                        _rightFolderFiles = finalFiles;
                    }

                    // 更新 UI
                    UpdateComboBox();
                    if (PicNamecomboBox.Items.Count > 0) PicNamecomboBox.SelectedIndex = 0;
                }
            }
        }
        private List<string> LoadEntriesFromZip(string zipFilePath)
        {
            List<string> entryNames = new List<string>();
            using (var archive = SharpCompress.Archives.ArchiveFactory.Open(zipFilePath))
            {
                foreach (var entry in archive.Entries.Where(e => !e.IsDirectory))
                {
                    string ext = Path.GetExtension(entry.Key).ToLower();
                    if (imageExtensions.Contains(ext))
                    {
                        // 统一格式，方便后续 ProcessAndLoad 匹配
                        entryNames.Add("[Zip]" + entry.Key);
                    }
                }
            }
            return entryNames;
        }
        private void LoadFromArchive(Control pb, string zipPath, string entryKey, bool isLeft)
        {
            try
            {
                // 1. 确定临时保存路径 (使用不同的前缀防止左右窗口冲突)
                string side = isLeft ? "left" : "right";
                string tempDir = Path.Combine(Application.StartupPath, "Archivetemp");
                if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);

                string tempFile = Path.Combine(tempDir, $"zip_{side}_{Path.GetFileName(entryKey)}");

                // 2. 解压条目
                using (var archive = SharpCompress.Archives.ArchiveFactory.Open(zipPath))
                {
                    var entry = archive.Entries.FirstOrDefault(e => e.Key == entryKey);
                    if (entry != null)
                    {
                        using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                        {
                            entry.WriteTo(fs);
                        }

                        // 3. 调用你已有的通用加载函数
                        ApplyImageToPictureBox(pb, tempFile);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"解压失败: {ex.Message}");
            }
        }
        private void ApplyImageToPictureBox(Control pb, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

            // 确保字典中有这个控件的状态记录
            if (!_viewStates.ContainsKey(pb)) _viewStates[pb] = new ViewState();
            var state = _viewStates[pb];

            // 1. 释放旧资源 (现在通过 state.Image 访问，它是 SKBitmap)
            if (state.Image != null)
            {
                state.Image.Dispose();
                state.Image = null; // 相当于以前的 pb.Image = null
            }

            // 2. 加载新图片 (使用 SkiaSharp 专用解码，不锁定文件)
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // 直接将流解码为 SKBitmap 存入字典
                    state.Image = SKBitmap.Decode(fs);
                    state.ImagePath = filePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"图片加载失败: {ex.Message}");
                return;
            }

            // 3. 重置视角并重绘
            ResetView(pb);
            // 4. 触发 GPU 控件的 PaintSurface 事件进行渲染
            pb.Invalidate();
        }
        #endregion

        #region 按键功能
        // 按键按下
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // 拦截方向键
            if (PicNamecomboBox.Items.Count > 0)
            {
                // 注意：ProcessCmdKey 也会受到长按重发的影响
                // 如果想禁止长按连发，这里依然需要 if (_isKeyDown) return true;

                switch (keyData)
                {
                    case Keys.Left:
                    case Keys.Up:
                        LastPic_Click(null, null);
                        return true;
                    case Keys.Right:
                    case Keys.Down:
                        NextPic_Click(null, null);
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void myKeyDown(object sender, KeyEventArgs e)
        {
            if (_isKeyDown) return; // 防止按键重复触发
            _isKeyDown = true;
            bool hasItems = PicNamecomboBox.Items.Count > 0;// 如果列表为空，方向键逻辑不执行

            switch (e.KeyCode)
            {
                // A键截图
                case Keys.Q:
                    SetScreenShotMode(true);
                    e.Handled = true;
                    break;
                // R 键重置显示
                case Keys.R:
                    ResetView(PicReview1);
                    ResetView(PicReview2);
                    e.Handled = true;
                    break;
                // S 键显示截图
                // Q切换到上一张
                case Keys.A:
                    if (hasItems)
                    {
                        LastPic_Click(null, null);
                        e.Handled = true;
                    }
                    break;

                // E切换到下一张
                case Keys.D:

                    if (hasItems)
                    {
                        NextPic_Click(null, null);
                        e.Handled = true;
                    }
                    break;
            }
        }
        // 按键松开
        private void myKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Q:
                    SetScreenShotMode(false);
                    break;
            }
            _isKeyDown = false;
            e.Handled = true;
        }
        // 将核心逻辑提取出来，接受一个明确的 bool 值
        private void SetScreenShotMode(bool active)
        {
            // 避免重复触发：如果状态没变，直接返回
            if (isScreenShotMode == active) return;
            isScreenShotMode = active;

            bool isDark = Modules.ThemeManager.IsDarkMode;
            Color activeBg = Modules.ThemeManager.AccentColor;
            Color defaultBg = isDark ? Color.FromArgb(60, 60, 60) : Color.OldLace;
            Color activeFore = isDark ? Color.White : Color.FromArgb(30, 70, 32);
            Color defaultFore = Modules.ThemeManager.TextColor;
            // 1. UI 视觉反馈
            ScreenShotButton.BackColor = active ? activeBg : defaultBg;
            ScreenShotButton.ForeColor = active ? activeFore : defaultFore;
            ScreenShotButton.Text = isScreenShotMode ? "退出截图" : "截图(Q)";

            // 2. 重置拉框状态
            isSelectingRect = false;
            ScreenShotRect = Rectangle.Empty;

            // 3. 刷新视图
            PicReview1.Invalidate();
            PicReview2.Invalidate();
        }
        #endregion

        private void Control_DragEnter(object sender, DragEventArgs e)
        {
            // 只接受文件拖入
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void HandleDrop(DragEventArgs e, bool isLeft)
        {
            // 获取拖入的文件路径列表
            string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (paths == null || paths.Length == 0) return;

            List<string> finalFiles = new List<string>();
            string zipPath = null;

            try
            {
                string firstPath = paths[0];

                // A. 拖入的是文件夹
                if (Directory.Exists(firstPath))
                {
                    zipPath = null;
                    finalFiles = Directory.GetFiles(firstPath)
                        .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()))
                        .OrderBy(f => f)
                        .ToList();
                }
                // B. 拖入的是文件
                else if (File.Exists(firstPath))
                {
                    string ext = Path.GetExtension(firstPath).ToLower();

                    // 如果是单个压缩包
                    if (paths.Length == 1 && archiveExts.Contains(ext))
                    {
                        zipPath = firstPath;
                        finalFiles = LoadEntriesFromZip(zipPath); // 调用你已有的解压列表方法
                    }
                    else
                    {
                        // 拖入的是单张或多张图片
                        zipPath = null;
                        finalFiles = paths
                            .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()))
                            .OrderBy(f => f)
                            .ToList();
                    }
                }

                // --- 应用结果到全局变量 ---
                if (finalFiles.Count > 0)
                {
                    if (isLeft) { _leftZipPath = zipPath; _leftFolderFiles = finalFiles; }
                    else { _rightZipPath = zipPath; _rightFolderFiles = finalFiles; }

                    UpdateComboBox(); // 更新下拉列表显示

                    // 自动选中第一张并触发渲染
                    if (PicNamecomboBox.Items.Count > 0)
                        PicNamecomboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"拖放处理失败: {ex.Message}");
            }
        }


        #region 截图功能
        /// <summary>
        /// 核心函数：根据当前截图矩形，从两个 PictureBox 中裁剪对应区域，
        /// </summary>
        private string CaptureScreenShotImageAndGetPath(Control pb)
        {

            try
            {
                // 1. 判定逻辑简化
                var state1 = _viewStates.ContainsKey(PicReview1) ? _viewStates[PicReview1] : null;
                var state2 = _viewStates.ContainsKey(PicReview2) ? _viewStates[PicReview2] : null;
                if (state1?.Image == null && state2?.Image == null) return null;

                var activeState = _viewStates[pb];
                var otherState = (pb == PicReview1) ? state2 : state1;
                bool hasOther = otherState?.Image != null;

                // 2. 获取裁剪区域 (x1, y1, w1, h1)
                PointF imgStart = ScreenToImage(pb, new Point(ScreenShotRect.Left, ScreenShotRect.Top));
                PointF imgEnd = ScreenToImage(pb, new Point(ScreenShotRect.Right, ScreenShotRect.Bottom));
                SKRect srcRectActive = new SKRect(Math.Max(0, imgStart.X), Math.Max(0, imgStart.Y),
                                                  Math.Min(activeState.Image.Width, imgEnd.X), Math.Min(activeState.Image.Height, imgEnd.Y));
                if (srcRectActive.Width <= 0 || srcRectActive.Height <= 0) return null;

                // 3. 计算布局尺寸
                SKRect srcRectOther = default;
                if (hasOther)
                {
                    // 比例同步计算
                    float rX = srcRectActive.Left / activeState.Image.Width;
                    float rY = srcRectActive.Top / activeState.Image.Height;
                    float rW = srcRectActive.Width / activeState.Image.Width;
                    float rH = srcRectActive.Height / activeState.Image.Height;
                    srcRectOther = new SKRect(rX * otherState.Image.Width, rY * otherState.Image.Height,
                                             (rX + rW) * otherState.Image.Width, (rY + rH) * otherState.Image.Height);
                }

                int mainContentW = hasOther ? (int)(srcRectActive.Width + srcRectOther.Width) : (int)srcRectActive.Width;
                int mainContentH = (int)(hasOther ? Math.Max(srcRectActive.Height, srcRectOther.Height) : srcRectActive.Height);
                // 逻辑：取高度的12%作为页脚，但最小不低于60像素(保证看清)，最大不超过150像素(防止占地太大)
                // 针对“很扁”的图，通过这个 Math.Clamp 保证了页脚依然有足够的高度
                int footerH = Math.Clamp((int)(mainContentH * 0.12), 60, 150);
                if (mainContentW > mainContentH * 3) footerH = Math.Max(footerH, 80);

                int canvasW = mainContentW;
                int canvasH = mainContentH + footerH;
                // 4. 使用 SkiaSharp 绘图 (替代 GDI+)
                using var combinedBitmap = new SKBitmap(canvasW, canvasH);
                using (var canvas = new SKCanvas(combinedBitmap))
                {
                    canvas.Clear(SKColors.White);

                    // 画左图和右图
                    var leftImg = (pb == PicReview1 || !hasOther) ? activeState.Image : otherState.Image;
                    var rightImg = (pb == PicReview1) ? otherState.Image : activeState.Image;
                    var leftSrc = (pb == PicReview1 || !hasOther) ? srcRectActive : srcRectOther;
                    var rightSrc = (pb == PicReview1) ? srcRectOther : srcRectActive;

                    canvas.DrawBitmap(leftImg, leftSrc, new SKRect(0, 0, leftSrc.Width, leftSrc.Height));
                    if (hasOther)
                    {
                        canvas.DrawBitmap(rightImg, rightSrc, new SKRect(leftSrc.Width, 0, leftSrc.Width + rightSrc.Width, rightSrc.Height));
                        // 画分割线
                        using var paint = new SKPaint { Color = SKColors.RoyalBlue, StrokeWidth = 2 };
                        canvas.DrawLine(leftSrc.Width, 0, leftSrc.Width, Math.Max(leftSrc.Height, rightSrc.Height), paint);
                    }

                    // 画页脚 (封装一个 Skia 版本的 DrawFooter)
                    DrawFooterText(canvas, PicNamecomboBox.SelectedItem?.ToString() ?? "Capture", canvasW, canvasH, footerH);
                }
                // 5. 统一压缩保存
                string filePath = Path.Combine(Application.StartupPath, "ScreenShottemp", $"ScreenShot_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                SaveImageWithCompression(
                                combinedBitmap,
                                filePath,
                                1024 * 1024,
                                path => this.BeginInvoke(new Action(() => UpdateThumbnail(path)))
                            );


                return filePath;

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); return null; }
        }
        private void DrawFooterText(SKCanvas canvas, string text, int w, int h, int fh)
        {
            // 背景矩形
            var rect = new SKRect(0, h - fh, w, h);
            using var bgPaint = new SKPaint { Color = SKColors.SeaShell, IsAntialias = true };
            canvas.DrawRect(rect, bgPaint);

            // 动态计算字号
            float dynamicTextSize = fh * 0.8f;
            using var boldTypeface = SKTypeface.FromFamilyName("微软雅黑", SKFontStyle.Bold);
            using var font = new SKFont(boldTypeface, dynamicTextSize);

            using var textPaint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
            using var arrowPaint = new SKPaint { Color = SKColors.RoyalBlue, IsAntialias = true };

            // 计算位置
            string arrowStr = "▲ ";
            float arrowW = font.MeasureText(arrowStr);
            float textW = font.MeasureText(text);
            float startX = (w - (arrowW + textW)) / 2;

            // 精确垂直居中计算
            var metrics = font.Metrics;
            float offset = (metrics.Ascent + metrics.Descent) / 2;
            float startY = (h - fh / 2f) - offset;

            canvas.DrawText(arrowStr, startX, startY, font, arrowPaint);
            canvas.DrawText(text, startX + arrowW, startY, font, textPaint);
        }

        public static byte[] SaveImageWithCompression(SKBitmap bitmap, string path, long limitBytes, Action<string> onSaved = null)
        {
            if (bitmap == null) return null;

            try
            {
                // 1. 自动创建目录
                string dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

                byte[] finalData = null;
                int quality = 90;

                using (var skImage = SKImage.FromBitmap(bitmap))
                {
                    // 2. 迭代压缩
                    do
                    {
                        using var encoded = skImage.Encode(SKEncodedImageFormat.Jpeg, quality);
                        finalData = encoded.ToArray();
                        if (finalData.Length <= limitBytes || quality <= 20) break;
                        quality -= 10;
                    } while (true);

                    // 3. 异步写入文件 (或同步写入)
                    File.WriteAllBytes(path, finalData);

                    // 4. 执行回调（更新 UI）
                    onSaved?.Invoke(path);

                    // 5. 写入剪贴板（复用内存字节数组）

                    try
                    {
                        using var ms = new MemoryStream(finalData);
                        using var gdiImg = System.Drawing.Image.FromStream(ms);
                        Clipboard.SetImage(gdiImg);
                    }
                    catch { /* 忽略剪贴板锁定异常 */ }

                }
                return finalData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image Process Error: {ex.Message}");
                return null;
            }
        }
        #endregion
        #region 编辑截图

        private Form previewPopup;
        private SKGLControl glControl;
        private SKBitmap baseBitmap;
        private SKCanvas drawingCanvas; // 用于持久化画迹
        private bool isDrawing = false;
        private SKPoint lastPt;
        private System.Windows.Forms.Timer closeTimer;
        private void InitPreviewPopup()
        {
            if (previewPopup != null) return;

            previewPopup = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                TopMost = true,
                Size = new Size(500, 350), // 悬浮窗稍大一点方便画画
                BackColor = Color.RoyalBlue,
                Padding = new Padding(2)
            };
            // --- 初始化计时器 ---
            closeTimer = new System.Windows.Forms.Timer();
            closeTimer.Interval = 500; // 0.5秒
            closeTimer.Tick += (s, e) =>
            {
                // 获取当前鼠标相对于屏幕的坐标
                Point mousePos = Control.MousePosition;

                // 检查鼠标是否在 PictureBox 内
                bool inButton = ShowShotScreen.Bounds.Contains(ShowShotScreen.Parent.PointToClient(mousePos));
                // 检查鼠标是否在 预览窗 内
                bool inPopup = previewPopup.Bounds.Contains(mousePos);

                if (inButton || inPopup || isDrawing)
                {
                    // 鼠标还在范围内，或者正在画画，不关闭，重新计时或停止
                    return;
                }

                closeTimer.Stop();
                SaveAndHidePopup();
            };
            // 预览窗的事件：鼠标进入取消倒计时，离开启动倒计时

            previewPopup.Deactivate += (s, e) =>
            {
                // 如果正在画画（鼠标没松开），不关闭
                if (isDrawing) return;

                // 使用 BeginInvoke 避开 WndProc 消息循环冲突
                previewPopup.BeginInvoke(new Action(() =>
                {
                    SaveAndHidePopup();
                }));
            };
            glControl = new SKGLControl
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(1) // 留出 1px 边框
            };

            // 绑定事件
            glControl.PaintSurface += GlControl_PaintSurface;
            glControl.MouseDown += GlControl_MouseDown;
            glControl.MouseMove += GlControl_MouseMove;
            glControl.MouseUp += GlControl_MouseUp;
            glControl.MouseEnter += (s, e) =>
            {
                glControl.Cursor = Cursors.Cross; // 设置为十字
                closeTimer.Stop();
            };
            glControl.MouseLeave += (s, e) =>
            {
                glControl.Cursor = Cursors.Default;
                // 只有当窗口可见且没在画画时才启动倒计时
                if (previewPopup.Visible && !isDrawing) closeTimer.Start();
            };
            glControl.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    ResetDoodles();
                }
            };
            previewPopup.Controls.Add(glControl);
            previewPopup.Opacity = 0;
            previewPopup.Show();
            previewPopup.Hide();
            previewPopup.Opacity = 1;
        }
        private void SaveAndHidePopup()
        {
            // 防止重复进入（比如弹出提示框时又触发了 Deactivate）
            if (previewPopup == null || !previewPopup.Visible) return;

            try
            {
                // 1. 必须先释放 Canvas，确保所有绘制操作已从缓存刷入 baseBitmap
                drawingCanvas?.Dispose();
                drawingCanvas = null;

                if (baseBitmap != null)
                {
                    SaveImageWithCompression(
                                    baseBitmap,
                                    currentScreenShotpath,
                                    1024 * 1024,
                                    path => this.BeginInvoke(new Action(() => UpdateThumbnail(path)))
                                );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("保存失败: " + ex.Message);
            }
            finally
            {
                // 隐藏窗口，并清理位图
                previewPopup.Hide();
                baseBitmap?.Dispose();
                baseBitmap = null;
                this.Activate();
                this.Focus();
            }
        }
        // --- 鼠标进入按钮：显示并准备画布 ---
        private void ShowShotScreen_MouseEnter(object sender, EventArgs e)
        {
            if (!File.Exists(currentScreenShotpath)) return;

            InitPreviewPopup();
            closeTimer?.Stop(); // 鼠标回来了，停止倒计时
            if (!previewPopup.Visible)
            {
                // 释放旧资源并加载新图
                drawingCanvas?.Dispose();
                baseBitmap?.Dispose();

                // 加载为可读写的 Bitmap
                baseBitmap = SKBitmap.Decode(currentScreenShotpath);
                drawingCanvas = new SKCanvas(baseBitmap);

                // 2. ---【关键】智能尺寸计算 ---
                // 获取屏幕可用工作区尺寸
                var workingArea = Screen.PrimaryScreen.WorkingArea;
                float maxW = workingArea.Width * 0.8f; // 最大宽度占屏幕80%
                float maxH = workingArea.Height * 0.6f; // 最大高度占屏幕80%

                float imgW = baseBitmap.Width;
                float imgH = baseBitmap.Height;

                // 计算缩放比，确保图片能完整装入最大限制区域
                float scale = Math.Min(maxW / imgW, maxH / imgH);

                // 如果图片本身很小，就不放大（可选：scale = Math.Min(1.0f, scale);）
                // 最终窗口大小 = 图片原始大小 * 缩放比
                previewPopup.Size = new Size((int)(imgW * scale), (int)(imgH * scale));

                // 3. 定位（保持在按钮上方，且不超出屏幕顶部）
                Rectangle mainFormRect = this.RectangleToScreen(this.ClientRectangle);
                int posX = mainFormRect.Left + (mainFormRect.Width - previewPopup.Width) / 2;// 2. 计算 X 坐标：主窗体中心点 - 预览窗宽度的一半
                int margin = 50; // 距离底部的间距
                int posY = mainFormRect.Bottom - previewPopup.Height - margin;// 3. 计算 Y 坐标：主窗体底部 - 预览窗高度 - 边距

                // 4. 安全检查：确保不超出屏幕可见区域（WorkingArea）
                if (posX < workingArea.Left) posX = workingArea.Left;
                if (posX + previewPopup.Width > workingArea.Right) posX = workingArea.Right - previewPopup.Width;
                if (posY + previewPopup.Height > workingArea.Bottom) posY = workingArea.Bottom - previewPopup.Height;

                previewPopup.Location = new Point(posX, posY);


                glControl.Invalidate();
                previewPopup.Show();
            }
        }
        private void ShowShotScreen_MouseLeave(object sender, EventArgs e)
        {
            // 鼠标离开按钮，如果没进入预览窗，2秒后关闭
            if (previewPopup != null && previewPopup.Visible && !isDrawing)
            {
                closeTimer.Start();
            }
        }
        // --- 绘图逻辑 ---
        private void GlControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                lastPt = GetImagePoint(e.X, e.Y);
                // --- 新增：点击即画一个点 ---
                using (var paint = new SKPaint
                {
                    Color = SKColors.Red,
                    Style = SKPaintStyle.Fill, // 使用填充模式画点
                    IsAntialias = true
                })
                {
                    // 画一个半径为 2.5 的圆（对应你 StrokeWidth=5 的效果）
                    drawingCanvas.DrawCircle(lastPt.X, lastPt.Y, 2.5f, paint);
                }
                glControl.Invalidate();
            }
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                var currPt = GetImagePoint(e.X, e.Y);
                using (var paint = new SKPaint
                {
                    Color = SKColors.Red,
                    StrokeWidth = 5,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    StrokeCap = SKStrokeCap.Round
                })
                {
                    drawingCanvas.DrawLine(lastPt, currPt, paint);
                }
                lastPt = currPt;
                glControl.Invalidate();
            }
        }

        private void GlControl_MouseUp(object sender, MouseEventArgs e) => isDrawing = false;

        private SKPoint GetImagePoint(int x, int y)
        {
            float cw = glControl.CanvasSize.Width;
            float ch = glControl.CanvasSize.Height;
            float ratio = Math.Min(cw / baseBitmap.Width, ch / baseBitmap.Height);
            float ox = (cw - baseBitmap.Width * ratio) / 2;
            float oy = (ch - baseBitmap.Height * ratio) / 2;
            return new SKPoint((x * (float)glControl.CanvasSize.Width / glControl.Width - ox) / ratio,
                               (y * (float)glControl.CanvasSize.Height / glControl.Height - oy) / ratio);
        }// --- 坐标映射 (适配 Zoom 模式) ---

        private void GlControl_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            if (baseBitmap == null) return;

            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Black);

            // 1. 创建高质量绘图配置
            using (var paint = new SKPaint())
            {
                // 核心：设置抗锯齿和高质量过滤
                paint.IsAntialias = true;

                // 2. 计算缩放和偏移
                float ratio = Math.Min((float)glControl.CanvasSize.Width / baseBitmap.Width,
                                       (float)glControl.CanvasSize.Height / baseBitmap.Height);

                // 为了防止坐标计算导致微小的边缘锯齿，我们直接计算目标矩形 (DestRect)
                float destW = baseBitmap.Width * ratio;
                float destH = baseBitmap.Height * ratio;
                float destX = (glControl.CanvasSize.Width - destW) / 2;
                float destY = (glControl.CanvasSize.Height - destH) / 2;

                SKRect destRect = new SKRect(destX, destY, destX + destW, destY + destH);
                using var skImage = SKImage.FromBitmap(baseBitmap);
                var sampling = new SKSamplingOptions(SKCubicResampler.CatmullRom);
                // 3. 使用带有 FilterQuality 的 paint 进行绘制
                canvas.DrawImage(skImage, destRect, sampling, paint);
            }
        }

        private void ResetDoodles()
        {
            if (string.IsNullOrEmpty(currentScreenShotpath) || !File.Exists(currentScreenShotpath)) return;

            // 1. 释放当前正在被涂鸦的资源
            drawingCanvas?.Dispose();
            baseBitmap?.Dispose();

            // 2. 重新从原始文件加载干净的图片
            baseBitmap = SKBitmap.Decode(currentScreenShotpath);
            drawingCanvas = new SKCanvas(baseBitmap);

            // 3. 通知 GPU 控件重新渲染
            glControl?.Invalidate();
        }
        private void UpdateThumbnail(string imagePath)
        {
            if (!File.Exists(imagePath)) return;

            try
            {
                // 使用流读取，防止文件被 PictureBox 锁定导致无法编辑保存
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    // 释放旧图片资源
                    ShowShotScreen.Image?.Dispose();
                    ShowShotScreen.Image = Image.FromStream(fs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("缩略图更新失败: " + ex.Message);
            }
        }
        #endregion


    }

}
