namespace mylabel
{
    partial class LabelMinusForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LabelMinusForm));
            PicView = new PictureBox();
            Menu = new MenuStrip();
            FileMenu = new ToolStripMenuItem();
            NewTranslation = new ToolStripMenuItem();
            OpenTranslation = new ToolStripMenuItem();
            SaveTranslation = new ToolStripMenuItem();
            OpenPicture = new ToolStripMenuItem();
            LabelView = new DataGridView();
            Index = new DataGridViewTextBoxColumn();
            Labeltext = new DataGridViewTextBoxColumn();
            Group = new DataGridViewTextBoxColumn();
            LabelViewMenuStrip = new ContextMenuStrip(components);
            deleteLabelToolStripMenuItem = new ToolStripMenuItem();
            TextBox = new TextBox();
            Tool = new ToolStrip();
            toolStripButton1 = new ToolStripButton();
            toolStripComboBox1 = new ToolStripComboBox();
            splitContainer1 = new SplitContainer();
            toolStrip1 = new ToolStrip();
            FittoViewButton = new ToolStripButton();
            PicName = new ToolStripComboBox();
            Parampanel = new Panel();
            Parampanel1 = new Panel();
            Fontlabel = new Label();
            RemarktextBox = new TextBox();
            FontstylecomboBox = new ComboBox();
            Remarklabel = new Label();
            Fontsizelabel = new Label();
            Locationshowlabel = new Label();
            FontsizecomboBox = new ComboBox();
            Locationlabel = new Label();
            imageLabelBindingSource = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)PicView).BeginInit();
            Menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LabelView).BeginInit();
            LabelViewMenuStrip.SuspendLayout();
            Tool.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            toolStrip1.SuspendLayout();
            Parampanel.SuspendLayout();
            Parampanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)imageLabelBindingSource).BeginInit();
            SuspendLayout();
            // 
            // PicView
            // 
            PicView.BorderStyle = BorderStyle.FixedSingle;
            PicView.Dock = DockStyle.Fill;
            PicView.Location = new Point(0, 0);
            PicView.Name = "PicView";
            PicView.Size = new Size(500, 508);
            PicView.TabIndex = 0;
            PicView.TabStop = false;
            // 
            // Menu
            // 
            Menu.Items.AddRange(new ToolStripItem[] { FileMenu });
            Menu.Location = new Point(0, 0);
            Menu.Name = "Menu";
            Menu.Size = new Size(984, 25);
            Menu.TabIndex = 1;
            Menu.Text = "menuStrip1";
            // 
            // FileMenu
            // 
            FileMenu.DropDownItems.AddRange(new ToolStripItem[] { NewTranslation, OpenTranslation, SaveTranslation, OpenPicture });
            FileMenu.Name = "FileMenu";
            FileMenu.Size = new Size(44, 21);
            FileMenu.Text = "文件";
            // 
            // NewTranslation
            // 
            NewTranslation.Name = "NewTranslation";
            NewTranslation.Size = new Size(124, 22);
            NewTranslation.Text = "新建翻译";
            NewTranslation.Click += NewTranslation_Click;
            // 
            // OpenTranslation
            // 
            OpenTranslation.Name = "OpenTranslation";
            OpenTranslation.Size = new Size(124, 22);
            OpenTranslation.Text = "打开翻译";
            OpenTranslation.Click += OpenTranslation_Click;
            // 
            // SaveTranslation
            // 
            SaveTranslation.Name = "SaveTranslation";
            SaveTranslation.Size = new Size(124, 22);
            SaveTranslation.Text = "保存翻译";
            SaveTranslation.Click += SaveTranslation_Click;
            // 
            // OpenPicture
            // 
            OpenPicture.Name = "OpenPicture";
            OpenPicture.Size = new Size(124, 22);
            OpenPicture.Text = "打开图片";
            OpenPicture.Click += 打开图片_Click;
            // 
            // LabelView
            // 
            LabelView.BackgroundColor = SystemColors.Info;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft YaHei UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            LabelView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            LabelView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            LabelView.Columns.AddRange(new DataGridViewColumn[] { Index, Labeltext, Group });
            LabelView.ContextMenuStrip = LabelViewMenuStrip;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = SystemColors.Window;
            dataGridViewCellStyle5.Font = new Font("Microsoft YaHei UI", 9F);
            dataGridViewCellStyle5.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = Color.FromArgb(255, 192, 128);
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
            LabelView.DefaultCellStyle = dataGridViewCellStyle5;
            LabelView.Dock = DockStyle.Fill;
            LabelView.Location = new Point(0, 0);
            LabelView.Name = "LabelView";
            LabelView.ReadOnly = true;
            LabelView.RowHeadersVisible = false;
            LabelView.ScrollBars = ScrollBars.Horizontal;
            LabelView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            LabelView.Size = new Size(480, 133);
            LabelView.TabIndex = 2;
            LabelView.SelectionChanged += LabelView_SelectionChanged;
            // 
            // Index
            // 
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Index.DefaultCellStyle = dataGridViewCellStyle2;
            Index.HeaderText = "Index";
            Index.MinimumWidth = 80;
            Index.Name = "Index";
            Index.ReadOnly = true;
            Index.SortMode = DataGridViewColumnSortMode.NotSortable;
            Index.Width = 80;
            // 
            // Labeltext
            // 
            Labeltext.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Labeltext.DefaultCellStyle = dataGridViewCellStyle3;
            Labeltext.HeaderText = "Text";
            Labeltext.Name = "Labeltext";
            Labeltext.ReadOnly = true;
            Labeltext.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Group
            // 
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
            Group.DefaultCellStyle = dataGridViewCellStyle4;
            Group.HeaderText = "Group";
            Group.Name = "Group";
            Group.ReadOnly = true;
            Group.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // LabelViewMenuStrip
            // 
            LabelViewMenuStrip.Items.AddRange(new ToolStripItem[] { deleteLabelToolStripMenuItem });
            LabelViewMenuStrip.Name = "LabelViewMenuStrip";
            LabelViewMenuStrip.Size = new Size(145, 26);
            // 
            // deleteLabelToolStripMenuItem
            // 
            deleteLabelToolStripMenuItem.Name = "deleteLabelToolStripMenuItem";
            deleteLabelToolStripMenuItem.Size = new Size(144, 22);
            deleteLabelToolStripMenuItem.Text = "delete label";
            deleteLabelToolStripMenuItem.Click += deleteLabelToolStripMenuItem_Click;
            // 
            // TextBox
            // 
            TextBox.BorderStyle = BorderStyle.FixedSingle;
            TextBox.Dock = DockStyle.Fill;
            TextBox.Location = new Point(0, 0);
            TextBox.Multiline = true;
            TextBox.Name = "TextBox";
            TextBox.Size = new Size(176, 400);
            TextBox.TabIndex = 3;
            TextBox.TextChanged += TextBox_TextChanged;
            // 
            // Tool
            // 
            Tool.Items.AddRange(new ToolStripItem[] { toolStripButton1, toolStripComboBox1 });
            Tool.Location = new Point(0, 25);
            Tool.Name = "Tool";
            Tool.Size = new Size(984, 25);
            Tool.TabIndex = 4;
            Tool.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(23, 22);
            toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripComboBox1
            // 
            toolStripComboBox1.Name = "toolStripComboBox1";
            toolStripComboBox1.Size = new Size(121, 25);
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 50);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(PicView);
            splitContainer1.Panel1.Controls.Add(toolStrip1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(LabelView);
            splitContainer1.Panel2.Controls.Add(Parampanel);
            splitContainer1.Size = new Size(984, 533);
            splitContainer1.SplitterDistance = 500;
            splitContainer1.TabIndex = 3;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.Items.AddRange(new ToolStripItem[] { FittoViewButton, PicName });
            toolStrip1.Location = new Point(0, 508);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(500, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // FittoViewButton
            // 
            FittoViewButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            FittoViewButton.Image = (Image)resources.GetObject("FittoViewButton.Image");
            FittoViewButton.ImageTransparentColor = Color.Magenta;
            FittoViewButton.Name = "FittoViewButton";
            FittoViewButton.Size = new Size(23, 22);
            FittoViewButton.Text = "toolStripButton2";
            FittoViewButton.Click += FittoViewButton_Click;
            // 
            // PicName
            // 
            PicName.Name = "PicName";
            PicName.Size = new Size(121, 25);
            PicName.SelectedIndexChanged += PicName_SelectedIndexChanged;
            // 
            // Parampanel
            // 
            Parampanel.Controls.Add(TextBox);
            Parampanel.Controls.Add(Parampanel1);
            Parampanel.Dock = DockStyle.Bottom;
            Parampanel.Location = new Point(0, 133);
            Parampanel.Name = "Parampanel";
            Parampanel.Size = new Size(480, 400);
            Parampanel.TabIndex = 4;
            // 
            // Parampanel1
            // 
            Parampanel1.Controls.Add(Fontlabel);
            Parampanel1.Controls.Add(RemarktextBox);
            Parampanel1.Controls.Add(FontstylecomboBox);
            Parampanel1.Controls.Add(Remarklabel);
            Parampanel1.Controls.Add(Fontsizelabel);
            Parampanel1.Controls.Add(Locationshowlabel);
            Parampanel1.Controls.Add(FontsizecomboBox);
            Parampanel1.Controls.Add(Locationlabel);
            Parampanel1.Dock = DockStyle.Right;
            Parampanel1.Location = new Point(176, 0);
            Parampanel1.Name = "Parampanel1";
            Parampanel1.Size = new Size(304, 400);
            Parampanel1.TabIndex = 8;
            // 
            // Fontlabel
            // 
            Fontlabel.AutoSize = true;
            Fontlabel.Location = new Point(3, 0);
            Fontlabel.Name = "Fontlabel";
            Fontlabel.Size = new Size(63, 17);
            Fontlabel.TabIndex = 0;
            Fontlabel.Text = "Font style";
            // 
            // RemarktextBox
            // 
            RemarktextBox.Dock = DockStyle.Bottom;
            RemarktextBox.Location = new Point(0, 316);
            RemarktextBox.Multiline = true;
            RemarktextBox.Name = "RemarktextBox";
            RemarktextBox.Size = new Size(304, 84);
            RemarktextBox.TabIndex = 7;
            RemarktextBox.TextChanged += RemarktextBox_TextChanged;
            // 
            // FontstylecomboBox
            // 
            FontstylecomboBox.FormattingEnabled = true;
            FontstylecomboBox.Location = new Point(3, 20);
            FontstylecomboBox.Name = "FontstylecomboBox";
            FontstylecomboBox.Size = new Size(298, 25);
            FontstylecomboBox.TabIndex = 1;
            FontstylecomboBox.SelectedIndexChanged += FontstylecomboBox_SelectedIndexChanged;
            // 
            // Remarklabel
            // 
            Remarklabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Remarklabel.AutoSize = true;
            Remarklabel.Location = new Point(3, 296);
            Remarklabel.Name = "Remarklabel";
            Remarklabel.Size = new Size(53, 17);
            Remarklabel.TabIndex = 6;
            Remarklabel.Text = "Remark";
            // 
            // Fontsizelabel
            // 
            Fontsizelabel.AutoSize = true;
            Fontsizelabel.Location = new Point(3, 48);
            Fontsizelabel.Name = "Fontsizelabel";
            Fontsizelabel.Size = new Size(59, 17);
            Fontsizelabel.TabIndex = 2;
            Fontsizelabel.Text = "Font size";
            // 
            // Locationshowlabel
            // 
            Locationshowlabel.BorderStyle = BorderStyle.FixedSingle;
            Locationshowlabel.Location = new Point(3, 113);
            Locationshowlabel.Name = "Locationshowlabel";
            Locationshowlabel.Size = new Size(100, 45);
            Locationshowlabel.TabIndex = 5;
            // 
            // FontsizecomboBox
            // 
            FontsizecomboBox.FormattingEnabled = true;
            FontsizecomboBox.Location = new Point(3, 68);
            FontsizecomboBox.Name = "FontsizecomboBox";
            FontsizecomboBox.Size = new Size(121, 25);
            FontsizecomboBox.TabIndex = 3;
            FontsizecomboBox.TextChanged += FontsizecomboBox_TextChanged;
            // 
            // Locationlabel
            // 
            Locationlabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            Locationlabel.AutoSize = true;
            Locationlabel.Location = new Point(3, 96);
            Locationlabel.Name = "Locationlabel";
            Locationlabel.Size = new Size(57, 17);
            Locationlabel.TabIndex = 4;
            Locationlabel.Text = "Location";
            // 
            // imageLabelBindingSource
            // 
            imageLabelBindingSource.DataSource = typeof(imageLabel);
            // 
            // LabelMinusForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 583);
            Controls.Add(splitContainer1);
            Controls.Add(Tool);
            Controls.Add(Menu);
            MainMenuStrip = Menu;
            Name = "LabelMinusForm";
            Text = "LabelMinus";
            Load += LabelMinus_Load;
            ((System.ComponentModel.ISupportInitialize)PicView).EndInit();
            Menu.ResumeLayout(false);
            Menu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)LabelView).EndInit();
            LabelViewMenuStrip.ResumeLayout(false);
            Tool.ResumeLayout(false);
            Tool.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            Parampanel.ResumeLayout(false);
            Parampanel.PerformLayout();
            Parampanel1.ResumeLayout(false);
            Parampanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)imageLabelBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox PicView;
        private MenuStrip Menu;
        private ToolStrip Tool;
        private ToolStripMenuItem FileMenu;
        private ToolStripMenuItem NewTranslation;
        private ToolStripMenuItem OpenTranslation;
        private ToolStripMenuItem SaveTranslation;
        private DataGridView LabelView;
        private SplitContainer splitContainer1;
        private TextBox TextBox;
        private ToolStripButton toolStripButton1;
        private ToolStripComboBox toolStripComboBox1;
        private ToolStripMenuItem OpenPicture;
        private ContextMenuStrip LabelViewMenuStrip;
        private ToolStripMenuItem deleteLabelToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripComboBox PicName;
        private BindingSource imageLabelBindingSource;
        private ToolStripButton FittoViewButton;
        private Panel Parampanel;
        private Label Fontlabel;
        private ComboBox FontstylecomboBox;
        private ComboBox FontsizecomboBox;
        private Label Fontsizelabel;
        private Label Locationlabel;
        private Label Locationshowlabel;
        private Label Remarklabel;
        private TextBox RemarktextBox;
        private Panel Parampanel1;
        private DataGridViewTextBoxColumn Index;
        private DataGridViewTextBoxColumn Labeltext;
        private DataGridViewTextBoxColumn Group;
    }
}
