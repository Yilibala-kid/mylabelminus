using System;
using System.Collections.Generic;
using System.Text;

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
                        FittoView(null, null);
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
        private void FittoView(object sender, EventArgs e)
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
        private void LP_Click(object sender, EventArgs e)
        {
            // 上一张 (Last Picture)
            if (PicName.Items.Count > 0)
            {
                // 如果当前不是第一张，索引减 1；如果是第一张，则跳到最后一张（循环模式）
                int newIndex = PicName.SelectedIndex - 1;

                if (newIndex < 0)
                {
                    newIndex = PicName.Items.Count - 1; // 循环到最后
                }

                PicName.SelectedIndex = newIndex;
            }
        }
        private void NP_Click(object sender, EventArgs e)
        {
            // 下一张 (Next Picture)
            if (PicName.Items.Count > 0)
            {
                // 如果当前不是最后一张，索引加 1；如果是最后一张，则跳到第一张（循环模式）
                int newIndex = PicName.SelectedIndex + 1;

                if (newIndex >= PicName.Items.Count)
                {
                    newIndex = 0; // 循环到开头
                }

                PicName.SelectedIndex = newIndex;
            }
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
    }
}
