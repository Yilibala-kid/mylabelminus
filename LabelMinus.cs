using mylabel.Modules;
using System.Drawing.Text;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace mylabel
{
    public partial class LabelMinusForm : Form
    {
        private string currentFilePath = string.Empty;
        private string GetFullImagePath(string relativeName)
        {
            if (string.IsNullOrEmpty(currentFilePath)) return relativeName;

            // 获取 txt 所在的目录
            string folder = System.IO.Path.GetDirectoryName(currentFilePath);
            // 拼接成绝对路径
            return System.IO.Path.Combine(folder, relativeName);
        }
        private bool _isUpdatingUI = false;
        private Image image;          // 当前显示的图片
        private float scale = 1f;     // 缩放比例
        private PointF offset = new PointF(0, 0); // 平移偏移
        private Point lastMouse;      // 上一次鼠标位置
        private PointF ScreenToImage(Point screenPoint)
        {
            float x = (screenPoint.X - offset.X) / scale;
            float y = (screenPoint.Y - offset.Y) / scale;
            return new PointF(x, y);
        }
        private Point mouseDownLocation;
        private bool isDragging = false;
        private bool isPotentialClick = false; // 用来标记是否可能是点击
        private const int ClickThreshold = 5; // 像素阈值
        private List<imageLabel> imageLabels = new();
        private string currentImageName = @"E:\psd\shuangqi.png";
        private int currentIndex = 1;
        private int selectedLabelIndex = -1;
        public LabelMinusForm()
        {
            InitializeComponent();

            // 假设 PicView 是你的 PictureBox
            PicView.Image = null;
            PicView.SizeMode = PictureBoxSizeMode.Normal;
            PicView.Paint += PicView_Paint;
            PicView.MouseDown += PicView_MouseDown;
            PicView.MouseMove += PicView_MouseMove;
            PicView.MouseUp += PicView_MouseUp;
            PicView.MouseWheel += PicView_MouseWheel;
            PicView.Focus(); // 确保接收鼠标滚轮事件

            // 测试图片
            image = Image.FromFile(@"E:\psd\shuangqi.png");

        }

        private void LabelMinus_Load(object sender, EventArgs e)
        {
            LoadSystemFonts(FontstylecomboBox);
            InitFontSizeComboBox();
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
        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            ComboBox combo = (ComboBox)sender;
            // 现在获取到的是字符串
            string fontName = combo.Items[e.Index].ToString();

            e.DrawBackground();

            try
            {
                // 根据名称创建字体
                using (Font font = new Font(fontName, 12, FontStyle.Regular))
                {
                    Brush brush = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                                  ? Brushes.White : Brushes.Black;

                    e.Graphics.DrawString(fontName, font, brush, e.Bounds);
                }
            }
            catch
            {
                // 容错处理：如果该字体名称无法生成 Regular 样式，使用默认 UI 字体绘制名称
                e.Graphics.DrawString(fontName, e.Font, Brushes.Black, e.Bounds);
            }

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

        #region 图像绘制
        private void PicView_Paint(object sender, PaintEventArgs e)
        {
            if (image == null) return;

            // A. 基础环境设置
            e.Graphics.Clear(Color.Gray);
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
            mylabel.Modules.Modules.DrawAnnotations(
                e.Graphics,
                imageLabels,
                currentImageName,
                image.Size,
                scale,
                selectedLabelIndex
            );


        }
        #endregion

        #region PicView事件
        private void PicView_MouseDown(object sender, MouseEventArgs e)
        {
            if (image == null) return;

            if (e.Button == MouseButtons.Left)
            {
                mouseDownLocation = e.Location;
                isDragging = false;
                isPotentialClick = true; // 假设是点击
            }
        }

        private void PicView_MouseMove(object sender, MouseEventArgs e)
        {
            //PointF imgPt = ScreenToImage(e.Location);
            //TextBox.Text = $"X: {e.Location.X:F4}, Y: {e.Location.Y:F4},scale:{scale:F4},imgPtX:{imgPt.X}";
            if (e.Button == MouseButtons.Left && isPotentialClick)
            {
                // 计算移动距离
                int dx = e.X - mouseDownLocation.X;
                int dy = e.Y - mouseDownLocation.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance > ClickThreshold)
                {
                    isDragging = true;      // 超过阈值认定为拖动
                    isPotentialClick = false;
                }
            }

            if (isDragging)
            {
                offset.X += e.X - mouseDownLocation.X;
                offset.Y += e.Y - mouseDownLocation.Y;
                mouseDownLocation = e.Location;
                PicView.Invalidate();
            }
        }

        private void PicView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!isDragging && isPotentialClick)
                {
                    // 点击逻辑：添加标注
                    PointF imgPt = ScreenToImage(e.Location);

                    if (imgPt.X < 0 || imgPt.Y < 0 ||
                        imgPt.X > image.Width || imgPt.Y > image.Height)
                        return;

                    float normX = imgPt.X / image.Width;
                    float normY = imgPt.Y / image.Height;

                    var label = new imageLabel
                    {
                        ImageName = currentImageName,
                        Index = currentIndex++,
                        Position = new BoundingBox
                        {
                            X = normX,
                            Y = normY,
                            Width = 0f,
                            Height = 0f
                        },
                        Text = "",
                        Group = LabelGroup.Other
                    };
                    imageLabels.Add(label);

                    PicView.Invalidate();

                }
                RefreshDataGridView();
                isDragging = false;
                isPotentialClick = false;
            }
        }

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
        private void 打开图片_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 只显示常见图片文件
            openFileDialog.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // 释放旧图片（重要，避免文件被锁）
                if (image != null)
                    image.Dispose();

                // 赋值给真正用于绘制的 image
                image = Image.FromFile(openFileDialog.FileName);
                currentImageName = openFileDialog.FileName;
                // 重置缩放和平移（推荐）
                scale = 1f;
                offset = Point.Empty;

                PicView.Invalidate(); // 触发重绘
            }
        }
        private void NewTranslation_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                saveFileDialog.Title = "新建翻译文件";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    currentFilePath = saveFileDialog.FileName;
                    // 创建一个空文件或写入初始内容
                    File.WriteAllText(currentFilePath, string.Empty);
                    MessageBox.Show($"文件已新建并记录路径：\n{currentFilePath}", "提示");
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
                        string content = File.ReadAllText(openFile.FileName);

                        // --- 直接通过 命名空间.类名.方法名 调用 ---
                        this.imageLabels = mylabel.Modules.Modules.ParseTextToLabels(content);

                        // 更新 UI
                        this.currentFilePath = openFile.FileName;
                        LabelView.DataSource = null;
                        LabelView.DataSource = this.imageLabels;
                        RefreshPicNameComboBox();

                        MessageBox.Show($"解析完成，共加载 {imageLabels.Count} 个标注。");
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
            if (string.IsNullOrEmpty(currentFilePath))
            {
                MessageBox.Show("当前没有打开的文件路径，请先新建或打开。", "提示");
                return;
            }

            try
            {
                // 1. 调用 Modules 里的转化逻辑
                string outputText = mylabel.Modules.Modules.LabelsToText(this.imageLabels);

                // 2. 写入文件（明确使用 System.IO 避免冲突）
                System.IO.File.WriteAllText(currentFilePath, outputText, System.Text.Encoding.UTF8);

                MessageBox.Show("保存成功！", "成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存过程中发生错误：\n{ex.Message}", "错误");
            }
        }
        #endregion

        #region 底栏功能
        private void FittoViewButton_Click(object sender, EventArgs e)
        {
            if (image == null) return;

            // 1. PictureBox 的可用绘制区域
            float viewW = PicView.ClientSize.Width;
            float viewH = PicView.ClientSize.Height;

            // 2. 图片原始尺寸
            float imgW = image.Width;
            float imgH = image.Height;

            // 3. 计算等比缩放（保证完整显示）
            float scaleX = viewW / imgW;
            float scaleY = viewH / imgH;
            scale = Math.Min(scaleX, scaleY);

            // 4. 计算居中偏移
            float drawW = imgW * scale;
            float drawH = imgH * scale;

            offset.X = (viewW - drawW) / 2f;
            offset.Y = (viewH - drawH) / 2f;

            // 5. 重绘
            PicView.Invalidate();
        }
        private void PicName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string fileName = PicName.Text;
            string fullPath = GetFullImagePath(fileName);

            if (System.IO.File.Exists(fullPath))
            {
                // 加载图片到 PicView
                PicName.Image = Image.FromFile(fullPath);
            }
            else
            {
                // 如果图片没找到，可以显示一个占位图或提示
                MessageBox.Show($"找不到图片文件：\n{fullPath}");
            }
        }
        private void RefreshPicNameComboBox()
        {
            // 1. 暂时解除事件绑定，防止清空/填充时触发 SelectedIndexChanged 逻辑
            PicName.SelectedIndexChanged -= PicName_SelectedIndexChanged;

            try
            {
                // 记录当前选中的图片名，方便刷新后重新定位
                string currentSelected = PicName.Text;

                // 2. 从数据源中提取唯一的图片名并排序
                // 使用 LINQ 可以非常方便地去重 (Distinct)
                var distinctImageNames = imageLabels
                    .Select(l => l.ImageName)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList();

                // 3. 更新 ComboBox 项目
                PicName.Items.Clear();
                foreach (var name in distinctImageNames)
                {
                    PicName.Items.Add(name);
                }

                // 4. 尝试恢复之前的选中项
                if (!string.IsNullOrEmpty(currentSelected) && PicName.Items.Contains(currentSelected))
                {
                    PicName.SelectedItem = currentSelected;
                }
                else if (PicName.Items.Count > 0)
                {
                    // 如果没选过，默认选第一张
                    PicName.SelectedIndex = 0;
                }
            }
            finally
            {
                // 5. 重新绑定事件
                PicName.SelectedIndexChanged += PicName_SelectedIndexChanged;
            }
        }


        #endregion



        #region LabelView相关
        private void RefreshDataGridView()
        {
            _isUpdatingUI = true; // 【关键】告诉程序：我现在在排版，别去改我的数据对象！

            try
            {
                LabelView.Rows.Clear();

                foreach (var label in imageLabels)
                {
                    // 这里判断是否属于当前图片
                    if (label.ImageName != currentImageName)
                        continue;

                    LabelView.Rows.Add(
                        label.Index,
                        label.Text,
                        label.Group
                    );
                }
            }
            finally
            {
                _isUpdatingUI = false; // 【关键】刷新完了，恢复正常逻辑
            }

        }
        private void LabelView_SelectionChanged(object sender, EventArgs e)
        {
            if (_isUpdatingUI) return; // 如果正在刷新列表，直接退出，不处理任何逻辑
            _isUpdatingUI = true; // 【开启屏蔽】
            try
            {
                var selectedLabel = GetSelectedLabel();

                if (selectedLabel != null)
                {
                    // 0. 显示文本
                    TextBox.Text = selectedLabel.Text;
                    // 1. 备注文本框
                    RemarktextBox.Text = selectedLabel.Remark;

                    // 2. 字体下拉框 (FontstylecomboBox)
                    // 尝试直接匹配（比如 "微软雅黑" 找 "微软雅黑"）
                    int fontIndex = FontstylecomboBox.FindStringExact(selectedLabel.FontFamily);
                    // 如果没找到，尝试通过 FontFamily 的 Name 转换匹配
                    if (fontIndex == -1)
                    {
                        try
                        {
                            // 尝试把存储的字符串（如 "Microsoft YaHei"）转成本地 FontFamily
                            using (var tempFamily = new FontFamily(selectedLabel.FontFamily))
                            {
                                // 获取该字体在当前系统显示的名称（如 "微软雅黑"）
                                string localName = tempFamily.Name;
                                fontIndex = FontstylecomboBox.FindStringExact(localName);
                            }
                        }
                        catch { /* 字体不存在于系统 */ }
                    }
                    //执行选中
                    FontstylecomboBox.SelectedIndex = fontIndex;
                    // 如果还是 -1，说明系统确实没装这个字体
                    if (fontIndex == -1)
                    {
                        Console.WriteLine($"警告：系统未找到字体 {selectedLabel.FontFamily}");
                    }

                    // 3. 字号下拉框 (FontsizecomboBox)
                    // 注意 double 到 string 的转换，保持一致性
                    FontsizecomboBox.Text = selectedLabel.FontSize.ToString("F0");

                    // 4. 位置显示 (Locationshowlabel)
                    // 假设 Position 里的 BoundingBox 有 X, Y 属性
                    Locationshowlabel.Text = $"X: {selectedLabel.Position.X:F4}" + Environment.NewLine + $"Y: {selectedLabel.Position.Y:F4}";

                    // 5. 原有的逻辑：刷新图片预览
                    PicView.Invalidate();
                }
                else
                {
                    // 清空 UI 状态
                    RemarktextBox.Clear();
                    FontstylecomboBox.SelectedIndex = -1;
                    FontsizecomboBox.Text = string.Empty;
                    Locationshowlabel.Text = "No Selection";
                }
            }
            finally
            {
                _isUpdatingUI = false; // 【关闭屏蔽】
            }
        }
        private void deleteLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 确认是否有选中的行
            if (LabelView.SelectedRows.Count > 0)
            {
                // 获取第一列的值（即 Label 的 Index）
                var cellValue = LabelView.SelectedRows[0].Cells[0].Value;
                if (cellValue == null) return;

                int targetIndex = (int)cellValue;

                // 1. 从数据源 Labels 中移除
                var itemToRemove = imageLabels.FirstOrDefault(l =>
                    l.Index == targetIndex && l.ImageName == currentImageName);

                if (itemToRemove != null)
                {
                    imageLabels.Remove(itemToRemove);

                    // 2. 刷新表格显示
                    RefreshDataGridView();

                    // 3. 刷新图片显示（去掉红点）
                    PicView.Invalidate();

                }
            }
        }

        private imageLabel GetSelectedLabel()
        {
            if (LabelView.CurrentRow != null && LabelView.CurrentRow.Index >= 0)
            {
                // 获取第一列存储的 Index
                var cellValue = LabelView.CurrentRow.Cells[0].Value;
                if (cellValue != null && cellValue is int targetIndex)
                {
                    selectedLabelIndex = targetIndex;
                    return imageLabels.FirstOrDefault(l =>
                        l.Index == targetIndex && l.ImageName == currentImageName);
                }
            }
            return null;
        }
        #endregion
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            // 1. 获取当前选中的行
            var selectedLabel = GetSelectedLabel();

            if (selectedLabel != null)
            {
                // 2. 将 TextBox 的内容实时同步到后台数据对象
                selectedLabel.Text = TextBox.Text;

                // 3. 同步更新 DataGridView 中对应的单元格显示（假设 Text 在第 2 列，索引为 1）
                if (LabelView.CurrentRow != null)
                {
                    // 直接修改单元格的值，不需要重新 RefreshDataGridView (那样会闪烁)
                    LabelView.CurrentRow.Cells[1].Value = TextBox.Text;
                }

                // 4. 如果你的图片上也要显示文字，调用重绘
                PicView.Invalidate();
            }
        }
        #region 同步 UI 修改到数据对象
        private void UpdateSelectedLabelFromUI()
        {
            // 如果正在初始化 UI（比如切换行），则不回写数据
            if (_isUpdatingUI) return;

            var selectedLabel = GetSelectedLabel();
            if (selectedLabel == null) return;

            // 1. 同步字体
            selectedLabel.FontFamily = FontstylecomboBox.Text;

            // 2. 同步字号 (处理 double 转换)
            if (double.TryParse(FontsizecomboBox.Text, out double size))
            {
                selectedLabel.FontSize = size;
            }

            // 3. 同步备注
            selectedLabel.Remark = RemarktextBox.Text;

            // 4. 同步后刷新预览图
            PicView.Invalidate();

            // 5. 如果 DataGridView 显示了这些信息，也刷新它
            LabelView.Refresh();
        }

        private void FontstylecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedLabelFromUI();
        }
        private void FontsizecomboBox_TextChanged(object sender, EventArgs e)
        {
            UpdateSelectedLabelFromUI();
        }
        private void RemarktextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateSelectedLabelFromUI();
        }
        #endregion


        
    }
}
