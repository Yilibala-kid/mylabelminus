using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using static mylabel.Modules.Modules;

namespace mylabel
{
    public partial class LabelMinusForm // 类名也必须一致，且带 partial
    {
        #region 快捷键功能/相对独立的功能
        private bool _isQDown = false;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // 拦截 Ctrl + Z
            if (keyData == (Keys.Control | Keys.Z))
            {
                _undoManager.Undo();
                imageLabelBindingSource.ResetCurrentItem();
                PicView.Invalidate();
                return true;
            }

            // 拦截 Ctrl + Y 
            if (keyData == (Keys.Control | Keys.Y))
            {
                _undoManager.Redo();
                imageLabelBindingSource.ResetCurrentItem();
                PicView.Invalidate();
                return true;
            }
            // 获取当前拥有焦点的最底层控件
            Control focusedControl = GetDeepActiveControl(this);

            // 只要是在输入类的控件里，就判定为“正在编辑”
            bool isEditing = focusedControl is TextBox || focusedControl is ComboBox;

            if (!isEditing)
            {
                switch (keyData)
                {
                    case Keys.A: // 上一张图片
                        PicNameBindingSource.MovePrevious();
                        return true;

                    case Keys.D: // 下一张图片
                        PicNameBindingSource.MoveNext();
                        return true;

                    case Keys.W: // 上一个标签
                        imageLabelBindingSource.MovePrevious();
                        return true;

                    case Keys.S: // 下一个标签
                        imageLabelBindingSource.MoveNext();
                        return true;

                    case Keys.R: // 适应屏幕
                        fittoview();
                        return true;

                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private Control GetDeepActiveControl(ContainerControl container)
        {
            Control active = container.ActiveControl;
            if (active is ContainerControl nestedContainer)
            {
                return GetDeepActiveControl(nestedContainer);
            }
            return active;
        }
        private void ImageReviewButton_Click(object sender, EventArgs e)
        {
            // 检查是否已经打开，防止重复打开
            Form existing = Application.OpenForms["ImageReviewForm"];
            if (existing != null)
            {
                existing.Activate();
                return;
            }

            // 创建并打开子窗口，把自己 (this) 传过去
            ImageReviewForm reviewForm = new ImageReviewForm(this);
            reviewForm.ShowDialog(); // 如果希望主窗口还能点，用 Show；如果不许点主窗口，用 ShowDialog
        }

        private void TextBox_FocusChanged(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
            {
                // 判断是进入还是离开
                bool isFocused = tb.Focused;

                tb.BackColor = isFocused ? Modules.ThemeManager.InputActiveBg : Modules.ThemeManager.InputIdleBg;
                tb.ForeColor = isFocused ? Modules.ThemeManager.InputActiveText : Modules.ThemeManager.InputIdleText;

                // 可选：激活时增加一点边框感（如果是 Fixed3D 风格）
                // tb.BorderStyle = isFocused ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
            }
        }
        private void TextReviewtoolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            // 背景和边框
            e.Graphics.FillRectangle(SystemBrushes.Info, e.Bounds);
            e.DrawBorder();

            using (Font myFont = new Font("微软雅黑", 12f, FontStyle.Regular))
            {

                // 定义绘图区域（稍微缩进一点，防止贴着边框）
                RectangleF layoutRect = new RectangleF(
                    e.Bounds.X + 5,
                    e.Bounds.Y + 5,
                    e.Bounds.Width - 10,
                    e.Bounds.Height - 10);

                // 使用 StringFormat 来控制换行
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Near;      // 左对齐
                    sf.LineAlignment = StringAlignment.Near;  // 顶对齐
                    sf.FormatFlags = StringFormatFlags.LineLimit; // 完整显示行，不截断半行

                    e.Graphics.DrawString(e.ToolTipText, myFont, Brushes.Black, layoutRect, sf);
                }
            }
        }
        private void TextReviewtoolTip_Popup(object sender, PopupEventArgs e)
        {
            using (Font myFont = new Font("微软雅黑", 12f, FontStyle.Regular))
            {
                string text = TextReviewtoolTip.GetToolTip(e.AssociatedControl);
                int maxWidth = 400; // 设置你希望的最大宽度

                // 使用 WordBreak 标记来计算换行后的尺寸
                Size textSize = TextRenderer.MeasureText(
                    text,
                    myFont,
                    new Size(maxWidth, 0),
                    TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

                e.ToolTipSize = new Size(textSize.Width + 15, textSize.Height + 15);
            }
        }

        #region 图片显示调整相关
        private void FittoView(object sender, EventArgs e)
        {
            fittoview();
        }
        private void fittoview()
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

        private void FitToWidth_Click(object sender, EventArgs e)
        {
            if (image == null) return;

            // 1. 获取控件宽度
            float viewW = PicView.ClientSize.Width;
            float imgW = image.Width;

            // 2. 缩放比例仅由宽度决定
            scale = viewW / imgW;

            // 3. 计算偏移：横向居中（实际上是 0），纵向通常置顶
            // 如果你想让它垂直也居中，可以用 (PicView.ClientSize.Height - image.Height * scale) / 2f
            offset.X = 0;
            offset.Y = (PicView.ClientSize.Height - image.Height * scale) / 2f;
            if (offset.Y < 0) offset.Y = 0;
            PicView.Invalidate();
        }

        private void FitToHeight_Click(object sender, EventArgs e)
        {
            if (image == null) return;

            // 1. 获取控件高度
            float viewH = PicView.ClientSize.Height;
            float imgH = image.Height;

            // 2. 缩放比例仅由高度决定
            scale = viewH / imgH;

            // 3. 计算偏移：横向通常靠左或居中，纵向为 0
            // 适应高度并垂直居中
            offset.X = (PicView.ClientSize.Width - image.Width * scale) / 2f;
            offset.Y = 0;
            // 如果计算出的 offset.Y < 0，说明图片比屏幕高，建议强制设为 0 从头显示
            if (offset.X < 0) offset.X = 0;

            PicView.Invalidate();
        }
        private void LockX_Click(object sender, EventArgs e)// 锁定横向按钮 (X 轴不动，只能上下移)
        {
            isXLocked = !isXLocked;
            UpdateLockButtonUI(sender as Button, isXLocked);
        }


        private void LockY_Click(object sender, EventArgs e)// 锁定竖直按钮 (Y 轴不动，只能左右移)
        {
            isYLocked = !isYLocked;
            UpdateLockButtonUI(sender as Button, isYLocked);
        }


        private void UpdateLockButtonUI(Button btn, bool isLocked)// 通用的 UI 反馈方法
        {
            Color oricolor = Modules.ThemeManager.IsDarkMode ? Color.FromArgb(60, 60, 60) : Color.OldLace;
            if (btn != null)
            {
                // 改变颜色或图标以示区别
                btn.BackColor = isLocked ? Color.LightCoral : oricolor;
            }
        }
        private void LP_Click(object sender, EventArgs e)
        {
            PicNameBindingSource.MovePrevious();
        }
        private void NP_Click(object sender, EventArgs e)
        {
            PicNameBindingSource.MoveNext();
        }
        private void Indexlabel_Paint(object sender, PaintEventArgs e)
        {
            Label lbl = (Label)sender;

            // 1. 定义画笔颜色和粗细
            using (Pen dashedPen = new Pen(Color.DimGray, 1))
            {
                // 2. 设置为虚线样式
                dashedPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                // 3. 计算边框范围（稍微收缩 1 像素，避免贴边看不清）
                Rectangle rect = new Rectangle(0, 0, lbl.Width - 1, lbl.Height - 1);

                // 4. 执行绘制
                e.Graphics.DrawRectangle(dashedPen, rect);
            }
        }
        #endregion
        #endregion
        #region 显示栏
        private void ParamHide_Click(object sender, EventArgs e)
        {
            if (ParamHide.Checked) Parampanel1.Visible = true;
            else
                Parampanel1.Visible = false;
        }

        private void LabelTextBoxOnly_Click(object sender, EventArgs e)
        {
            SetMenuChecked(LabelTextBoxOnly);
            splitContainer2.Panel1Collapsed = true;
            splitContainer2.Panel2Collapsed = false;
        }

        private void LabelViewOnly_Click(object sender, EventArgs e)
        {
            SetMenuChecked(LabelViewOnly);
            splitContainer2.Panel1Collapsed = false;
            splitContainer2.Panel2Collapsed = true;
        }

        private void BothShow_Click(object sender, EventArgs e)
        {
            SetMenuChecked(BothShow);
            splitContainer2.Panel1Collapsed = false;
            splitContainer2.Panel2Collapsed = false;
        }
        private void SetMenuChecked(ToolStripMenuItem activeItem)
        {
            LabelTextBoxOnly.Checked = (activeItem == LabelTextBoxOnly);
            LabelViewOnly.Checked = (activeItem == LabelViewOnly);
            BothShow.Checked = (activeItem == BothShow);
        }
        private void PicOnly_Click(object sender, EventArgs e)
        {
            if (PicOnly.Checked)
            {
                splitContainer1.Panel2Collapsed = true;  // 折叠 Panel2
                splitContainer1.Panel1Collapsed = false; // 确保 Panel1 是开启的
            }
            else
            {
                splitContainer1.Panel2Collapsed = false; // 展开 Panel2
            }
            fittoview();
        }
        private void LabelViewIndexShow_Click(object sender, EventArgs e)
        {
            if (LabelView.Columns.Contains("LabelIndex"))
            {
                LabelView.Columns["LabelIndex"].Visible = LabelViewIndexShow.Checked;
            }
        }
        private void LabelViewGroupShow_Click(object sender, EventArgs e)
        {
            if (LabelView.Columns.Contains("LabelGroup"))
            {
                LabelView.Columns["LabelGroup"].Visible = LabelViewGroupShow.Checked;
            }
        }
        private void PicViewGroup_Click(object sender, EventArgs e)
        {
            PicView.Invalidate();
        }
        private void PicViewText_Click(object sender, EventArgs e)
        {
            PicView.Invalidate();
        }
        #endregion
        #region 温馨提示功能
        private void ToGithub_Click(object sender, EventArgs e)
        {
            string url = "https://github.com/Yilibala-kid/mylabelminus";

            try
            {
                // .NET Core / .NET 5+ 必须设置 UseShellExecute = true 才能直接打开网页
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                ShowMsg($"无法打开网页", true);
            }
        }
        private void Notations_Click(object sender, EventArgs e)
        {
            var texts = "请将翻译文本与压缩包放在同一文件夹内";
            ShowMsg(texts);
        }
        private void MessageTimer_Tick(object sender, EventArgs e)
        {
            MessageLabel.Text = ""; // 恢复默认文字
            MessageTimer.Stop();
        }
        public void ShowMsg(string message, bool isError = false)
        {
            MessageTimer?.Stop(); // 如果上一次还没结束，先停止旧的

            MessageLabel.Text = message;
            MessageLabel.ForeColor = isError ? Color.Crimson : Color.RoyalBlue;

            // 强制 UI 刷新，防止在高负载任务中 Label 不更新
            MessageLabel.Refresh();

            MessageTimer.Start();
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

        #region 右键打开文件的逻辑/注册表注册删除  
        // 定义右键菜单的名称，保持与注册表一致
        private const string ContextMenuName = "使用LabelMinus打开";
        private readonly string[] TargetExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".webp", ".zip", ".7z", ".rar", ".txt" };
        // 检查当前是否是管理员
        private bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        // 以管理员身份重启程序
        private void RestartAsAdmin()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Application.ExecutablePath;
            startInfo.Verb = "runas"; // 关键：触发 UAC 提权弹窗

            try
            {
                Process.Start(startInfo);
                Application.Exit(); // 退出当前普通权限的进程
            }
            catch (Exception)
            {
                // 如果用户在 UAC 弹窗点击了“否”
                MessageBox.Show("未获得管理员权限，无法执行注册表操作。", "提示");
            }
        }
        // 【核心方法】切换右键菜单状态
        private void ToggleContextMenu()
        {
            // --- 新增权限检查 ---
            if (!IsRunAsAdmin())
            {
                var result = MessageBox.Show(
                    "修改右键菜单需要管理员权限。是否以管理员身份重启程序？",
                    "权限请求",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    RestartAsAdmin();
                }
                return; // 结束当前逻辑
            }

            // --- 原有的逻辑保持不变 ---
            if (IsContextMenuInstalled())
            {
                UninstallContextMenu();
                ShowMsg("右键菜单功能已关闭。");
            }
            else
            {
                InstallContextMenu();
                ShowMsg("右键菜单功能已开启！现在可以在文件或文件夹上右键使用了。");
            }
            UpdateContextMenuButtonState();
        }

        // 检查右键菜单是否已安装
        private bool IsContextMenuInstalled()
        {
            // 检查 .jpg 的关联是否存在作为判断标准
            string checkPath = $@"SystemFileAssociations\.jpg\shell\{ContextMenuName}";
            using (var key = Registry.ClassesRoot.OpenSubKey(checkPath))
            {
                return key != null;
            }
        }

        // 安装（写入注册表）
        private void InstallContextMenu()
        {
            string rawExePath = Application.ExecutablePath; // 获取当前程序完整路径
            string quotedExePath = $"\"{rawExePath}\"";
            string[] reviewExtensions = { ".zip", ".7z", ".rar" }; // 压缩包扩展名
            try
            {
                using (var key = Registry.ClassesRoot.CreateSubKey($@"Directory\shell\{ContextMenuName}_Review"))
                {
                    key.SetValue("", "使用 LabelMinus 图校");
                    key.SetValue("Icon", $"{rawExePath},0");
                    using (var cmd = key.CreateSubKey("command"))
                    {
                        // 关键点：末尾增加了 -review 参数
                        cmd.SetValue("", $"{quotedExePath} \"%1\" -review");
                    }
                }

                // 2. 处理压缩包的“图校”菜单
                foreach (string ext in reviewExtensions)
                {
                    using (var key = Registry.ClassesRoot.CreateSubKey($@"SystemFileAssociations\{ext}\shell\{ContextMenuName}_Review"))
                    {
                        key.SetValue("", "使用 LabelMinus 图校");
                        key.SetValue("Icon", $"{rawExePath},0");
                        using (var cmd = key.CreateSubKey("command"))
                        {
                            cmd.SetValue("", $"{quotedExePath} \"%1\" -review");
                        }
                    }
                }
                foreach (string ext in TargetExtensions)
                {
                    string keyPath = $@"SystemFileAssociations\{ext}\shell\{ContextMenuName}";
                    using (var key = Registry.ClassesRoot.CreateSubKey(keyPath))
                    {
                        key.SetValue("", "用 LabelMinus 打开");

                        // --- 图标可以带 ,0 ---
                        key.SetValue("Icon", $"{rawExePath},0");

                        using (var commandKey = key.CreateSubKey("command"))
                        {
                            // 正确格式应该是: "C:\...\LabelMinus.exe" "%1"
                            commandKey.SetValue("", $"{quotedExePath} \"%1\"");
                        }
                    }
                }

                // 依然保留文件夹的注册，方便直接打开整个目录
                using (var dirKey = Registry.ClassesRoot.CreateSubKey($@"Directory\shell\{ContextMenuName}"))
                {
                    dirKey.SetValue("", "用 LabelMinus 预览文件夹");
                    dirKey.SetValue("Icon", $"{rawExePath},0");
                    using (var dirCommand = dirKey.CreateSubKey("command"))
                    {
                        dirCommand.SetValue("", $"{quotedExePath} \"%1\"");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"注册表写入失败，请尝试以管理员身份运行程序。\n错误信息：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 卸载（删除注册表项）
        private void UninstallContextMenu()
        {
            try
            {
                foreach (string ext in TargetExtensions)
                {
                    // 删除普通打开菜单
                    string standardKey = $@"SystemFileAssociations\{ext}\shell\{ContextMenuName}";
                    Registry.ClassesRoot.DeleteSubKeyTree(standardKey, false);

                    // 删除“图校”菜单
                    string reviewKey = $@"SystemFileAssociations\{ext}\shell\{ContextMenuName}_Review";
                    Registry.ClassesRoot.DeleteSubKeyTree(reviewKey, false);
                }

                // 清理文件夹注册
                // 删除文件夹普通预览
                Registry.ClassesRoot.DeleteSubKeyTree($@"Directory\shell\{ContextMenuName}", false);

                // 删除文件夹“图校”
                Registry.ClassesRoot.DeleteSubKeyTree($@"Directory\shell\{ContextMenuName}_Review", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"移除右键菜单失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 更新按钮文字状态
        private void UpdateContextMenuButtonState()
        {
            bool isInstalled = IsContextMenuInstalled();

            // 直接访问名为 EasyRightClick 的 ToolStripMenuItem
            // (前提是你在设计器中添加了这个菜单项，并且 Name 属性为 EasyRightClick)
            if (EasyRightClick != null)
            {
                EasyRightClick.Checked = isInstalled;
            }
        }
        private void EasyRightClick_Click(object sender, EventArgs e)
        {
            // 点击菜单时，直接调用你之前的 Toggle 方法
            ToggleContextMenu();
        }

        private List<string> pathBuffer = new List<string>();

        // 在构造函数或 Load 事件中初始化计时器
        private void OpenTimer_Tick(object sender, EventArgs e)
        {
            OpenTimer.Stop();
            string[] pathsToOpen;
            lock (pathBuffer) // 1. 必须加锁，应对右键选中大量文件时的并发写入
            {
                if (pathBuffer.Count == 0) return;
                pathsToOpen = pathBuffer.ToArray();
                pathBuffer.Clear();
            }
            // 4. 处理图校模式
            if (_pendingReviewRequest)
            {
                _pendingReviewRequest = false;

                this.BeginInvoke(new Action(() => {
                    // 1. 获取或创建图校窗口
                    ImageReviewForm reviewForm = Application.OpenForms["ImageReviewForm"] as ImageReviewForm;
                    if (reviewForm == null)
                    {
                        reviewForm = new ImageReviewForm(this);
                        reviewForm.Show();
                    }

                    if (pathsToOpen.Length >= 1)
                    {
                        // 第一个选中的路径进左边
                        reviewForm.LoadPathDirectly(pathsToOpen[0], true);
                    }

                    if (pathsToOpen.Length >= 2)
                    {
                        // 如果用户同时选中了两个，第二个进右边
                        reviewForm.LoadPathDirectly(pathsToOpen[1], false);
                    }

                    reviewForm.Activate();
                }));
            }
            else this.OpenResourceByPath(pathsToOpen, false);
        }
        // 暴露一个给外部调用的缓冲方法
        public void EnqueuePaths(string path, bool isReview = false)
        {
            if (isReview) _pendingReviewRequest = true;
            if (string.IsNullOrEmpty(path)) return;

            lock (pathBuffer) // 1. 加上锁
            {
                pathBuffer.Add(path);
            }
            OpenTimer.Stop();
            OpenTimer.Start();
        }
        #endregion

    }
}
