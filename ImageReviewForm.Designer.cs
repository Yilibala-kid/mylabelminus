namespace mylabel
{
    partial class ImageReviewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageReviewForm));
            splitContainer1 = new SplitContainer();
            PicReview1_Placeholder = new PictureBox();
            PicReview2_Placeholder = new PictureBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            ChangePic2 = new Button();
            panel1 = new Panel();
            Showlabel = new Label();
            PicNamecomboBox = new ComboBox();
            ScreenShotButton = new Button();
            LastPic = new Button();
            NextPic = new Button();
            ChangePic1 = new Button();
            panel5 = new Panel();
            Openfolder1 = new Button();
            OpenPicandArc1 = new Button();
            panel6 = new Panel();
            Openfolder2 = new Button();
            OpenPicandArc2 = new Button();
            ClearPic = new Button();
            tableLayoutPanel2 = new TableLayoutPanel();
            FittoReViewButton = new Button();
            panel2 = new Panel();
            LinkView = new Button();
            ClearBrush = new Button();
            BrushButton = new Button();
            panel3 = new Panel();
            label1 = new Label();
            panel4 = new Panel();
            ShowShotScreen = new Button();
            ScreenShotFolderbutton = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PicReview1_Placeholder).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PicReview2_Placeholder).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 60);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(PicReview1_Placeholder);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(PicReview2_Placeholder);
            splitContainer1.Size = new Size(800, 340);
            splitContainer1.SplitterDistance = 400;
            splitContainer1.TabIndex = 2;
            // 
            // PicReview1_Placeholder
            // 
            PicReview1_Placeholder.BackColor = Color.SeaShell;
            PicReview1_Placeholder.BorderStyle = BorderStyle.FixedSingle;
            PicReview1_Placeholder.Dock = DockStyle.Fill;
            PicReview1_Placeholder.Location = new Point(0, 0);
            PicReview1_Placeholder.Name = "PicReview1_Placeholder";
            PicReview1_Placeholder.Size = new Size(400, 340);
            PicReview1_Placeholder.TabIndex = 0;
            PicReview1_Placeholder.TabStop = false;
            PicReview1_Placeholder.Click += PicView_Click;
            PicReview1_Placeholder.MouseDown += PicView_MouseDown;
            PicReview1_Placeholder.MouseMove += PicView_MouseMove;
            PicReview1_Placeholder.MouseUp += PicView_MouseUp;
            PicReview1_Placeholder.MouseWheel += PicView_MouseWheel;
            // 
            // PicReview2_Placeholder
            // 
            PicReview2_Placeholder.BackColor = Color.SeaShell;
            PicReview2_Placeholder.BorderStyle = BorderStyle.FixedSingle;
            PicReview2_Placeholder.Dock = DockStyle.Fill;
            PicReview2_Placeholder.Location = new Point(0, 0);
            PicReview2_Placeholder.Name = "PicReview2_Placeholder";
            PicReview2_Placeholder.Size = new Size(396, 340);
            PicReview2_Placeholder.TabIndex = 0;
            PicReview2_Placeholder.TabStop = false;
            PicReview2_Placeholder.Click += PicView_Click;
            PicReview2_Placeholder.MouseDown += PicView_MouseDown;
            PicReview2_Placeholder.MouseMove += PicView_MouseMove;
            PicReview2_Placeholder.MouseUp += PicView_MouseUp;
            PicReview2_Placeholder.MouseWheel += PicView_MouseWheel;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(ChangePic2, 4, 0);
            tableLayoutPanel1.Controls.Add(panel1, 2, 0);
            tableLayoutPanel1.Controls.Add(ChangePic1, 0, 0);
            tableLayoutPanel1.Controls.Add(panel5, 1, 0);
            tableLayoutPanel1.Controls.Add(panel6, 3, 0);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(800, 60);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // ChangePic2
            // 
            ChangePic2.Dock = DockStyle.Bottom;
            ChangePic2.Location = new Point(722, 27);
            ChangePic2.Name = "ChangePic2";
            ChangePic2.Size = new Size(75, 30);
            ChangePic2.TabIndex = 4;
            ChangePic2.Text = "更换图片";
            ChangePic2.UseVisualStyleBackColor = true;
            ChangePic2.Click += ChangePic2_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(Showlabel);
            panel1.Controls.Add(PicNamecomboBox);
            panel1.Controls.Add(ScreenShotButton);
            panel1.Controls.Add(LastPic);
            panel1.Controls.Add(NextPic);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(200, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(400, 54);
            panel1.TabIndex = 2;
            // 
            // Showlabel
            // 
            Showlabel.AutoSize = true;
            Showlabel.Dock = DockStyle.Left;
            Showlabel.Location = new Point(75, 0);
            Showlabel.Name = "Showlabel";
            Showlabel.Size = new Size(128, 17);
            Showlabel.TabIndex = 3;
            Showlabel.Text = "截图自动复制到剪贴板";
            Showlabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PicNamecomboBox
            // 
            PicNamecomboBox.Dock = DockStyle.Bottom;
            PicNamecomboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            PicNamecomboBox.Font = new Font("Microsoft YaHei UI", 10F);
            PicNamecomboBox.FormattingEnabled = true;
            PicNamecomboBox.Location = new Point(75, 27);
            PicNamecomboBox.Name = "PicNamecomboBox";
            PicNamecomboBox.Size = new Size(175, 27);
            PicNamecomboBox.TabIndex = 1;
            PicNamecomboBox.SelectedIndexChanged += PicNamecomboBox_SelectedIndexChanged;
            // 
            // ScreenShotButton
            // 
            ScreenShotButton.AutoSize = true;
            ScreenShotButton.Dock = DockStyle.Left;
            ScreenShotButton.Location = new Point(0, 0);
            ScreenShotButton.Name = "ScreenShotButton";
            ScreenShotButton.Size = new Size(75, 54);
            ScreenShotButton.TabIndex = 0;
            ScreenShotButton.Text = "截图(Q)";
            ScreenShotButton.UseVisualStyleBackColor = true;
            ScreenShotButton.Click += ScreenShotButton_Click;
            // 
            // LastPic
            // 
            LastPic.Dock = DockStyle.Right;
            LastPic.Location = new Point(250, 0);
            LastPic.Name = "LastPic";
            LastPic.Size = new Size(75, 54);
            LastPic.TabIndex = 3;
            LastPic.Text = "上一张";
            LastPic.UseVisualStyleBackColor = true;
            LastPic.Click += LastPic_Click;
            // 
            // NextPic
            // 
            NextPic.Dock = DockStyle.Right;
            NextPic.Location = new Point(325, 0);
            NextPic.Name = "NextPic";
            NextPic.Size = new Size(75, 54);
            NextPic.TabIndex = 2;
            NextPic.Text = "下一张";
            NextPic.UseVisualStyleBackColor = true;
            NextPic.Click += NextPic_Click;
            // 
            // ChangePic1
            // 
            ChangePic1.Dock = DockStyle.Bottom;
            ChangePic1.Location = new Point(3, 27);
            ChangePic1.Name = "ChangePic1";
            ChangePic1.Size = new Size(75, 30);
            ChangePic1.TabIndex = 3;
            ChangePic1.Text = "更换图片";
            ChangePic1.UseVisualStyleBackColor = true;
            ChangePic1.Click += ChangePic1_Click;
            // 
            // panel5
            // 
            panel5.Controls.Add(Openfolder1);
            panel5.Controls.Add(OpenPicandArc1);
            panel5.Dock = DockStyle.Left;
            panel5.Location = new Point(81, 0);
            panel5.Margin = new Padding(0);
            panel5.Name = "panel5";
            panel5.Size = new Size(116, 60);
            panel5.TabIndex = 6;
            // 
            // Openfolder1
            // 
            Openfolder1.Dock = DockStyle.Fill;
            Openfolder1.Location = new Point(0, 30);
            Openfolder1.Margin = new Padding(0);
            Openfolder1.Name = "Openfolder1";
            Openfolder1.Size = new Size(116, 30);
            Openfolder1.TabIndex = 7;
            Openfolder1.Text = "选择文件夹";
            Openfolder1.UseVisualStyleBackColor = true;
            Openfolder1.Click += Openfolder1_Click_1;
            // 
            // OpenPicandArc1
            // 
            OpenPicandArc1.Dock = DockStyle.Top;
            OpenPicandArc1.Location = new Point(0, 0);
            OpenPicandArc1.Margin = new Padding(0);
            OpenPicandArc1.Name = "OpenPicandArc1";
            OpenPicandArc1.Size = new Size(116, 30);
            OpenPicandArc1.TabIndex = 6;
            OpenPicandArc1.Text = "选择图片/压缩包";
            OpenPicandArc1.UseVisualStyleBackColor = true;
            OpenPicandArc1.Click += OpenPicandArc1_Click;
            // 
            // panel6
            // 
            panel6.Controls.Add(Openfolder2);
            panel6.Controls.Add(OpenPicandArc2);
            panel6.Dock = DockStyle.Right;
            panel6.Location = new Point(603, 0);
            panel6.Margin = new Padding(0);
            panel6.Name = "panel6";
            panel6.Size = new Size(116, 60);
            panel6.TabIndex = 7;
            // 
            // Openfolder2
            // 
            Openfolder2.Dock = DockStyle.Fill;
            Openfolder2.Location = new Point(0, 30);
            Openfolder2.Name = "Openfolder2";
            Openfolder2.Size = new Size(116, 30);
            Openfolder2.TabIndex = 6;
            Openfolder2.Text = "选择文件夹";
            Openfolder2.UseVisualStyleBackColor = true;
            Openfolder2.Click += Openfolder2_Click_1;
            // 
            // OpenPicandArc2
            // 
            OpenPicandArc2.Dock = DockStyle.Top;
            OpenPicandArc2.Location = new Point(0, 0);
            OpenPicandArc2.Name = "OpenPicandArc2";
            OpenPicandArc2.Size = new Size(116, 30);
            OpenPicandArc2.TabIndex = 5;
            OpenPicandArc2.Text = "选择图片/压缩包";
            OpenPicandArc2.UseVisualStyleBackColor = true;
            OpenPicandArc2.Click += OpenPicandArc2_Click;
            // 
            // ClearPic
            // 
            ClearPic.Dock = DockStyle.Right;
            ClearPic.Location = new Point(0, 0);
            ClearPic.Name = "ClearPic";
            ClearPic.Size = new Size(75, 44);
            ClearPic.TabIndex = 2;
            ClearPic.Text = "清空图片";
            ClearPic.UseVisualStyleBackColor = true;
            ClearPic.Click += ClearPic_Click;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 5;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel2.Controls.Add(FittoReViewButton, 0, 0);
            tableLayoutPanel2.Controls.Add(panel2, 2, 0);
            tableLayoutPanel2.Controls.Add(panel3, 4, 0);
            tableLayoutPanel2.Controls.Add(label1, 3, 0);
            tableLayoutPanel2.Controls.Add(panel4, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Bottom;
            tableLayoutPanel2.Location = new Point(0, 400);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(800, 50);
            tableLayoutPanel2.TabIndex = 4;
            // 
            // FittoReViewButton
            // 
            FittoReViewButton.Dock = DockStyle.Left;
            FittoReViewButton.Location = new Point(3, 3);
            FittoReViewButton.Name = "FittoReViewButton";
            FittoReViewButton.Size = new Size(75, 44);
            FittoReViewButton.TabIndex = 1;
            FittoReViewButton.Text = "适应屏幕";
            FittoReViewButton.UseVisualStyleBackColor = true;
            FittoReViewButton.Click += FittoReViewButton_Click;
            // 
            // panel2
            // 
            panel2.Controls.Add(LinkView);
            panel2.Controls.Add(ClearBrush);
            panel2.Controls.Add(BrushButton);
            panel2.Location = new Point(270, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(260, 44);
            panel2.TabIndex = 3;
            // 
            // LinkView
            // 
            LinkView.Dock = DockStyle.Fill;
            LinkView.Location = new Point(90, 0);
            LinkView.Name = "LinkView";
            LinkView.Size = new Size(80, 44);
            LinkView.TabIndex = 4;
            LinkView.Text = "同步显示";
            LinkView.UseVisualStyleBackColor = true;
            LinkView.Click += LinkView_Click;
            // 
            // ClearBrush
            // 
            ClearBrush.Dock = DockStyle.Right;
            ClearBrush.Location = new Point(170, 0);
            ClearBrush.Name = "ClearBrush";
            ClearBrush.Size = new Size(90, 44);
            ClearBrush.TabIndex = 3;
            ClearBrush.Text = "清空笔迹";
            ClearBrush.UseVisualStyleBackColor = true;
            ClearBrush.Click += ClearBrush_Click;
            // 
            // BrushButton
            // 
            BrushButton.Dock = DockStyle.Left;
            BrushButton.Location = new Point(0, 0);
            BrushButton.Name = "BrushButton";
            BrushButton.Size = new Size(90, 44);
            BrushButton.TabIndex = 2;
            BrushButton.Text = "画图(F)";
            BrushButton.UseVisualStyleBackColor = true;
            BrushButton.Click += BrushButton_Click;
            // 
            // panel3
            // 
            panel3.AutoSize = true;
            panel3.Controls.Add(ClearPic);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(722, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(75, 44);
            panel3.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Right;
            label1.Location = new Point(540, 0);
            label1.Name = "label1";
            label1.Size = new Size(176, 50);
            label1.TabIndex = 3;
            label1.Text = "导入图片支持多选，支持导入.zip/.7z/.rar压缩包\r\n若要选择文件夹，请点进文件夹后全选图片";
            // 
            // panel4
            // 
            panel4.BackColor = SystemColors.Info;
            panel4.Controls.Add(ShowShotScreen);
            panel4.Controls.Add(ScreenShotFolderbutton);
            panel4.Location = new Point(84, 3);
            panel4.Name = "panel4";
            panel4.Size = new Size(180, 44);
            panel4.TabIndex = 5;
            // 
            // ShowShotScreen
            // 
            ShowShotScreen.Dock = DockStyle.Left;
            ShowShotScreen.Location = new Point(103, 0);
            ShowShotScreen.Name = "ShowShotScreen";
            ShowShotScreen.Size = new Size(75, 44);
            ShowShotScreen.TabIndex = 6;
            ShowShotScreen.Text = "编辑截图";
            ShowShotScreen.UseVisualStyleBackColor = true;
            ShowShotScreen.MouseEnter += ShowShotScreen_MouseEnter;
            ShowShotScreen.MouseLeave += ShowShotScreen_MouseLeave;
            // 
            // ScreenShotFolderbutton
            // 
            ScreenShotFolderbutton.Dock = DockStyle.Left;
            ScreenShotFolderbutton.Location = new Point(0, 0);
            ScreenShotFolderbutton.Name = "ScreenShotFolderbutton";
            ScreenShotFolderbutton.Size = new Size(103, 44);
            ScreenShotFolderbutton.TabIndex = 5;
            ScreenShotFolderbutton.Text = "打开截图文件夹";
            ScreenShotFolderbutton.UseVisualStyleBackColor = true;
            ScreenShotFolderbutton.Click += ScreenShotFolderbutton_Click;
            // 
            // ImageReviewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(tableLayoutPanel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Name = "ImageReviewForm";
            Text = "图校";
            Load += ImageReviewForm_Load;
            KeyDown += myKeyDown;
            KeyUp += myKeyUp;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PicReview1_Placeholder).EndInit();
            ((System.ComponentModel.ISupportInitialize)PicReview2_Placeholder).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel5.ResumeLayout(false);
            panel6.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            panel2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private SplitContainer splitContainer1;
        private TableLayoutPanel tableLayoutPanel1;
        private Button ScreenShotButton;
        private TableLayoutPanel tableLayoutPanel2;
        private ComboBox PicNamecomboBox;
        private Panel panel1;
        private Button ClearPic;
        private Button FittoReViewButton;
        private Button BrushButton;
        private Panel panel2;
        private Button LinkView;
        private Button ClearBrush;
        private Button ChangePic2;
        private Button ChangePic1;
        private Button OpenPicandArc2;
        private Button OpenPicandArc1;
        private Button LastPic;
        private Button NextPic;
        private Panel panel3;
        private Label Showlabel;
        private Label label1;
        private Button ScreenShotFolderbutton;
        private PictureBox PicReview1_Placeholder;
        private PictureBox PicReview2_Placeholder;
        private Panel panel4;
        private Button ShowShotScreen;
        private Panel panel5;
        private Button Openfolder1;
        private Panel panel6;
        private Button Openfolder2;
    }
}