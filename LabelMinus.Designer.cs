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
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LabelMinusForm));
            PicView = new PictureBox();
            Menu = new MenuStrip();
            FileMenu = new ToolStripMenuItem();
            NewTranslation = new ToolStripMenuItem();
            OpenTranslation = new ToolStripMenuItem();
            SaveTranslation = new ToolStripMenuItem();
            SaveAsTranslation = new ToolStripMenuItem();
            LabelView = new DataGridView();
            LabelIndex = new DataGridViewTextBoxColumn();
            LabelText = new DataGridViewTextBoxColumn();
            LabelGroup = new DataGridViewTextBoxColumn();
            indexDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            textDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            groupDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            positionDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            fontSizeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            fontFamilyDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            remarkDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            LabelViewMenuStrip = new ContextMenuStrip(components);
            deleteLabelToolStripMenuItem = new ToolStripMenuItem();
            imageLabelBindingSource = new BindingSource(components);
            PicNameBindingSource = new BindingSource(components);
            Tool = new ToolStrip();
            StatusLabel = new ToolStripLabel();
            LabelMode = new ToolStripButton();
            TextReviewMode = new ToolStripButton();
            OCRMode = new ToolStripButton();
            OCRtoolStripLabel = new ToolStripLabel();
            OCRComboBox = new ToolStripComboBox();
            ImageReviewButton = new ToolStripButton();
            splitContainer1 = new SplitContainer();
            bottompanel = new Panel();
            PicNameLabel = new Label();
            PicName = new ComboBox();
            LPbutton = new Button();
            NPbutton = new Button();
            FittoViewbutton = new Button();
            Parampanel = new Panel();
            Indexlabel = new Label();
            Parampanel2 = new Panel();
            LabelTextBox = new TextBox();
            Parampanel1 = new Panel();
            Locationshowlabel = new Label();
            Locationlabel = new Label();
            GroupcomboBox = new ComboBox();
            Grouplabel = new Label();
            FontsizecomboBox = new ComboBox();
            Fontsizelabel = new Label();
            FontstylecomboBox = new ComboBox();
            Fontlabel = new Label();
            RemarktextBox = new TextBox();
            Remarklabel = new Label();
            TextReviewtoolTip = new ToolTip(components);
            hoverTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)PicView).BeginInit();
            Menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LabelView).BeginInit();
            LabelViewMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)imageLabelBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PicNameBindingSource).BeginInit();
            Tool.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            bottompanel.SuspendLayout();
            Parampanel.SuspendLayout();
            Parampanel2.SuspendLayout();
            Parampanel1.SuspendLayout();
            SuspendLayout();
            // 
            // PicView
            // 
            PicView.BackColor = Color.SeaShell;
            PicView.BorderStyle = BorderStyle.FixedSingle;
            PicView.Dock = DockStyle.Fill;
            PicView.Location = new Point(0, 0);
            PicView.Name = "PicView";
            PicView.Size = new Size(500, 482);
            PicView.TabIndex = 0;
            PicView.TabStop = false;
            PicView.Paint += PicView_Paint;
            PicView.MouseDown += PicView_MouseDown;
            PicView.MouseMove += PicView_MouseMove;
            PicView.MouseUp += PicView_MouseUp;
            PicView.MouseWheel += PicView_MouseWheel;
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
            FileMenu.DropDownItems.AddRange(new ToolStripItem[] { NewTranslation, OpenTranslation, SaveTranslation, SaveAsTranslation });
            FileMenu.Name = "FileMenu";
            FileMenu.Size = new Size(44, 21);
            FileMenu.Text = "文件";
            // 
            // NewTranslation
            // 
            NewTranslation.Name = "NewTranslation";
            NewTranslation.Size = new Size(136, 22);
            NewTranslation.Text = "新建翻译";
            NewTranslation.Click += NewTranslation_Click;
            // 
            // OpenTranslation
            // 
            OpenTranslation.Name = "OpenTranslation";
            OpenTranslation.Size = new Size(136, 22);
            OpenTranslation.Text = "打开翻译";
            OpenTranslation.Click += OpenTranslation_Click;
            // 
            // SaveTranslation
            // 
            SaveTranslation.Name = "SaveTranslation";
            SaveTranslation.Size = new Size(136, 22);
            SaveTranslation.Text = "保存翻译";
            SaveTranslation.Click += SaveTranslation_Click;
            // 
            // SaveAsTranslation
            // 
            SaveAsTranslation.Name = "SaveAsTranslation";
            SaveAsTranslation.Size = new Size(136, 22);
            SaveAsTranslation.Text = "另存为翻译";
            SaveAsTranslation.Click += SaveAsTranslation_Click;
            // 
            // LabelView
            // 
            LabelView.AllowUserToAddRows = false;
            LabelView.AllowUserToResizeColumns = false;
            LabelView.AllowUserToResizeRows = false;
            LabelView.AutoGenerateColumns = false;
            LabelView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            LabelView.BackgroundColor = SystemColors.Info;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            LabelView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            LabelView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            LabelView.Columns.AddRange(new DataGridViewColumn[] { LabelIndex, LabelText, LabelGroup, indexDataGridViewTextBoxColumn, textDataGridViewTextBoxColumn, groupDataGridViewTextBoxColumn, positionDataGridViewTextBoxColumn, fontSizeDataGridViewTextBoxColumn, fontFamilyDataGridViewTextBoxColumn, remarkDataGridViewTextBoxColumn });
            LabelView.ContextMenuStrip = LabelViewMenuStrip;
            LabelView.DataSource = imageLabelBindingSource;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(255, 192, 128);
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            LabelView.DefaultCellStyle = dataGridViewCellStyle4;
            LabelView.Dock = DockStyle.Fill;
            LabelView.GridColor = SystemColors.Menu;
            LabelView.Location = new Point(0, 0);
            LabelView.MultiSelect = false;
            LabelView.Name = "LabelView";
            LabelView.ReadOnly = true;
            LabelView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            LabelView.RowHeadersVisible = false;
            LabelView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            LabelView.Size = new Size(480, 118);
            LabelView.TabIndex = 2;
            LabelView.CellMouseDown += LabelView_CellMouseClick;
            // 
            // LabelIndex
            // 
            LabelIndex.DataPropertyName = "Index";
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LabelIndex.DefaultCellStyle = dataGridViewCellStyle2;
            LabelIndex.HeaderText = "序号";
            LabelIndex.MinimumWidth = 80;
            LabelIndex.Name = "LabelIndex";
            LabelIndex.ReadOnly = true;
            LabelIndex.SortMode = DataGridViewColumnSortMode.NotSortable;
            LabelIndex.Width = 80;
            // 
            // LabelText
            // 
            LabelText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            LabelText.DataPropertyName = "Text";
            LabelText.HeaderText = "文本";
            LabelText.Name = "LabelText";
            LabelText.ReadOnly = true;
            // 
            // LabelGroup
            // 
            LabelGroup.DataPropertyName = "Group";
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LabelGroup.DefaultCellStyle = dataGridViewCellStyle3;
            LabelGroup.HeaderText = "组别";
            LabelGroup.Name = "LabelGroup";
            LabelGroup.ReadOnly = true;
            LabelGroup.SortMode = DataGridViewColumnSortMode.NotSortable;
            LabelGroup.Width = 44;
            // 
            // indexDataGridViewTextBoxColumn
            // 
            indexDataGridViewTextBoxColumn.DataPropertyName = "Index";
            indexDataGridViewTextBoxColumn.HeaderText = "序号";
            indexDataGridViewTextBoxColumn.Name = "indexDataGridViewTextBoxColumn";
            indexDataGridViewTextBoxColumn.ReadOnly = true;
            indexDataGridViewTextBoxColumn.Visible = false;
            indexDataGridViewTextBoxColumn.Width = 44;
            // 
            // textDataGridViewTextBoxColumn
            // 
            textDataGridViewTextBoxColumn.DataPropertyName = "Text";
            textDataGridViewTextBoxColumn.HeaderText = "文本内容";
            textDataGridViewTextBoxColumn.Name = "textDataGridViewTextBoxColumn";
            textDataGridViewTextBoxColumn.ReadOnly = true;
            textDataGridViewTextBoxColumn.Visible = false;
            textDataGridViewTextBoxColumn.Width = 44;
            // 
            // groupDataGridViewTextBoxColumn
            // 
            groupDataGridViewTextBoxColumn.DataPropertyName = "Group";
            groupDataGridViewTextBoxColumn.HeaderText = "分组";
            groupDataGridViewTextBoxColumn.Name = "groupDataGridViewTextBoxColumn";
            groupDataGridViewTextBoxColumn.ReadOnly = true;
            groupDataGridViewTextBoxColumn.Visible = false;
            groupDataGridViewTextBoxColumn.Width = 45;
            // 
            // positionDataGridViewTextBoxColumn
            // 
            positionDataGridViewTextBoxColumn.DataPropertyName = "Position";
            positionDataGridViewTextBoxColumn.HeaderText = "位置";
            positionDataGridViewTextBoxColumn.Name = "positionDataGridViewTextBoxColumn";
            positionDataGridViewTextBoxColumn.ReadOnly = true;
            positionDataGridViewTextBoxColumn.Visible = false;
            positionDataGridViewTextBoxColumn.Width = 44;
            // 
            // fontSizeDataGridViewTextBoxColumn
            // 
            fontSizeDataGridViewTextBoxColumn.DataPropertyName = "FontSize";
            fontSizeDataGridViewTextBoxColumn.HeaderText = "字号";
            fontSizeDataGridViewTextBoxColumn.Name = "fontSizeDataGridViewTextBoxColumn";
            fontSizeDataGridViewTextBoxColumn.ReadOnly = true;
            fontSizeDataGridViewTextBoxColumn.Visible = false;
            fontSizeDataGridViewTextBoxColumn.Width = 44;
            // 
            // fontFamilyDataGridViewTextBoxColumn
            // 
            fontFamilyDataGridViewTextBoxColumn.DataPropertyName = "FontFamily";
            fontFamilyDataGridViewTextBoxColumn.HeaderText = "字体";
            fontFamilyDataGridViewTextBoxColumn.Name = "fontFamilyDataGridViewTextBoxColumn";
            fontFamilyDataGridViewTextBoxColumn.ReadOnly = true;
            fontFamilyDataGridViewTextBoxColumn.Visible = false;
            fontFamilyDataGridViewTextBoxColumn.Width = 44;
            // 
            // remarkDataGridViewTextBoxColumn
            // 
            remarkDataGridViewTextBoxColumn.DataPropertyName = "Remark";
            remarkDataGridViewTextBoxColumn.HeaderText = "备注";
            remarkDataGridViewTextBoxColumn.Name = "remarkDataGridViewTextBoxColumn";
            remarkDataGridViewTextBoxColumn.ReadOnly = true;
            remarkDataGridViewTextBoxColumn.Visible = false;
            remarkDataGridViewTextBoxColumn.Width = 44;
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
            // imageLabelBindingSource
            // 
            imageLabelBindingSource.DataMember = "Labels";
            imageLabelBindingSource.DataSource = PicNameBindingSource;
            imageLabelBindingSource.CurrentItemChanged += imageLabelBindingSource_CurrentItemChanged;
            imageLabelBindingSource.PositionChanged += imageLabelBindingSource_PositionChanged;
            // 
            // PicNameBindingSource
            // 
            PicNameBindingSource.DataSource = typeof(ImageInfo);
            PicNameBindingSource.CurrentItemChanged += PicNameBindingSource_CurrentItemChanged;
            // 
            // Tool
            // 
            Tool.AutoSize = false;
            Tool.Items.AddRange(new ToolStripItem[] { StatusLabel, LabelMode, TextReviewMode, OCRMode, OCRtoolStripLabel, OCRComboBox, ImageReviewButton });
            Tool.Location = new Point(0, 25);
            Tool.Name = "Tool";
            Tool.Size = new Size(984, 40);
            Tool.TabIndex = 4;
            Tool.Text = "toolStrip1";
            // 
            // StatusLabel
            // 
            StatusLabel.Alignment = ToolStripItemAlignment.Right;
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new Size(146, 37);
            StatusLabel.Text = "sponsored by No-Hifuu";
            // 
            // LabelMode
            // 
            LabelMode.Image = (Image)resources.GetObject("LabelMode.Image");
            LabelMode.ImageTransparentColor = Color.Magenta;
            LabelMode.Name = "LabelMode";
            LabelMode.Size = new Size(76, 37);
            LabelMode.Text = "标记模式";
            LabelMode.Click += LabelMode_Click;
            // 
            // TextReviewMode
            // 
            TextReviewMode.Image = LabelMinus.Properties.Resources.图标;
            TextReviewMode.ImageTransparentColor = Color.Magenta;
            TextReviewMode.Name = "TextReviewMode";
            TextReviewMode.Size = new Size(76, 37);
            TextReviewMode.Text = "文校模式";
            TextReviewMode.Click += TextReviewMode_Click;
            // 
            // OCRMode
            // 
            OCRMode.Image = (Image)resources.GetObject("OCRMode.Image");
            OCRMode.ImageTransparentColor = Color.Magenta;
            OCRMode.Name = "OCRMode";
            OCRMode.Size = new Size(76, 37);
            OCRMode.Text = "识别模式";
            OCRMode.Click += OCRMode_Click;
            // 
            // OCRtoolStripLabel
            // 
            OCRtoolStripLabel.Name = "OCRtoolStripLabel";
            OCRtoolStripLabel.Size = new Size(68, 37);
            OCRtoolStripLabel.Text = "识别网站：";
            // 
            // OCRComboBox
            // 
            OCRComboBox.Font = new Font("Microsoft YaHei UI", 11F);
            OCRComboBox.Name = "OCRComboBox";
            OCRComboBox.Overflow = ToolStripItemOverflow.Never;
            OCRComboBox.Size = new Size(240, 40);
            // 
            // ImageReviewButton
            // 
            ImageReviewButton.Image = (Image)resources.GetObject("ImageReviewButton.Image");
            ImageReviewButton.ImageTransparentColor = Color.Magenta;
            ImageReviewButton.Name = "ImageReviewButton";
            ImageReviewButton.Size = new Size(76, 37);
            ImageReviewButton.Text = "图校模式";
            ImageReviewButton.Click += ImageReviewButton_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 65);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(PicView);
            splitContainer1.Panel1.Controls.Add(bottompanel);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(LabelView);
            splitContainer1.Panel2.Controls.Add(Parampanel);
            splitContainer1.Panel2.Margin = new Padding(3);
            splitContainer1.Size = new Size(984, 518);
            splitContainer1.SplitterDistance = 500;
            splitContainer1.TabIndex = 3;
            // 
            // bottompanel
            // 
            bottompanel.Controls.Add(PicNameLabel);
            bottompanel.Controls.Add(PicName);
            bottompanel.Controls.Add(LPbutton);
            bottompanel.Controls.Add(NPbutton);
            bottompanel.Controls.Add(FittoViewbutton);
            bottompanel.Dock = DockStyle.Bottom;
            bottompanel.Location = new Point(0, 482);
            bottompanel.Name = "bottompanel";
            bottompanel.Size = new Size(500, 36);
            bottompanel.TabIndex = 2;
            // 
            // PicNameLabel
            // 
            PicNameLabel.Anchor = AnchorStyles.Right;
            PicNameLabel.AutoSize = true;
            PicNameLabel.Location = new Point(148, 10);
            PicNameLabel.Name = "PicNameLabel";
            PicNameLabel.Size = new Size(35, 17);
            PicNameLabel.TabIndex = 7;
            PicNameLabel.Text = "图集:";
            PicNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PicName
            // 
            PicName.Anchor = AnchorStyles.Right;
            PicName.DataSource = PicNameBindingSource;
            PicName.DisplayMember = "ImageName";
            PicName.Font = new Font("Microsoft YaHei UI", 12F);
            PicName.FormattingEnabled = true;
            PicName.Location = new Point(189, 3);
            PicName.Name = "PicName";
            PicName.Size = new Size(161, 29);
            PicName.TabIndex = 6;
            // 
            // LPbutton
            // 
            LPbutton.Dock = DockStyle.Right;
            LPbutton.Location = new Point(350, 0);
            LPbutton.Name = "LPbutton";
            LPbutton.Size = new Size(75, 36);
            LPbutton.TabIndex = 4;
            LPbutton.Text = "上一张";
            LPbutton.UseVisualStyleBackColor = true;
            LPbutton.Click += LP_Click;
            // 
            // NPbutton
            // 
            NPbutton.Dock = DockStyle.Right;
            NPbutton.Location = new Point(425, 0);
            NPbutton.Name = "NPbutton";
            NPbutton.Size = new Size(75, 36);
            NPbutton.TabIndex = 3;
            NPbutton.Text = "下一张";
            NPbutton.UseVisualStyleBackColor = true;
            NPbutton.Click += NP_Click;
            // 
            // FittoViewbutton
            // 
            FittoViewbutton.Dock = DockStyle.Left;
            FittoViewbutton.Location = new Point(0, 0);
            FittoViewbutton.Name = "FittoViewbutton";
            FittoViewbutton.Size = new Size(75, 36);
            FittoViewbutton.TabIndex = 0;
            FittoViewbutton.Text = "适应屏幕";
            FittoViewbutton.UseVisualStyleBackColor = true;
            FittoViewbutton.Click += FittoView;
            // 
            // Parampanel
            // 
            Parampanel.Controls.Add(Indexlabel);
            Parampanel.Controls.Add(Parampanel2);
            Parampanel.Controls.Add(Parampanel1);
            Parampanel.Dock = DockStyle.Bottom;
            Parampanel.Location = new Point(0, 118);
            Parampanel.Name = "Parampanel";
            Parampanel.Padding = new Padding(0, 30, 10, 10);
            Parampanel.Size = new Size(480, 400);
            Parampanel.TabIndex = 4;
            // 
            // Indexlabel
            // 
            Indexlabel.DataBindings.Add(new Binding("Text", imageLabelBindingSource, "Index", true));
            Indexlabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 134);
            Indexlabel.Location = new Point(3, 4);
            Indexlabel.Name = "Indexlabel";
            Indexlabel.Size = new Size(50, 24);
            Indexlabel.TabIndex = 10;
            Indexlabel.Text = "999";
            Indexlabel.TextAlign = ContentAlignment.MiddleCenter;
            Indexlabel.Paint += Indexlabel_Paint;
            // 
            // Parampanel2
            // 
            Parampanel2.Controls.Add(LabelTextBox);
            Parampanel2.Dock = DockStyle.Fill;
            Parampanel2.Location = new Point(0, 30);
            Parampanel2.Name = "Parampanel2";
            Parampanel2.Padding = new Padding(0, 3, 3, 3);
            Parampanel2.Size = new Size(309, 360);
            Parampanel2.TabIndex = 9;
            // 
            // LabelTextBox
            // 
            LabelTextBox.BackColor = Color.AntiqueWhite;
            LabelTextBox.BorderStyle = BorderStyle.FixedSingle;
            LabelTextBox.DataBindings.Add(new Binding("Text", imageLabelBindingSource, "Text", true, DataSourceUpdateMode.OnPropertyChanged));
            LabelTextBox.Dock = DockStyle.Fill;
            LabelTextBox.Font = new Font("Microsoft YaHei UI", 12F);
            LabelTextBox.ForeColor = Color.Gray;
            LabelTextBox.Location = new Point(0, 3);
            LabelTextBox.Multiline = true;
            LabelTextBox.Name = "LabelTextBox";
            LabelTextBox.Size = new Size(306, 354);
            LabelTextBox.TabIndex = 3;
            LabelTextBox.Enter += TextBox_Enter;
            LabelTextBox.Leave += TextBox_Leave;
            // 
            // Parampanel1
            // 
            Parampanel1.Controls.Add(Locationshowlabel);
            Parampanel1.Controls.Add(Locationlabel);
            Parampanel1.Controls.Add(GroupcomboBox);
            Parampanel1.Controls.Add(Grouplabel);
            Parampanel1.Controls.Add(FontsizecomboBox);
            Parampanel1.Controls.Add(Fontsizelabel);
            Parampanel1.Controls.Add(FontstylecomboBox);
            Parampanel1.Controls.Add(Fontlabel);
            Parampanel1.Controls.Add(RemarktextBox);
            Parampanel1.Controls.Add(Remarklabel);
            Parampanel1.Dock = DockStyle.Right;
            Parampanel1.Location = new Point(309, 30);
            Parampanel1.Name = "Parampanel1";
            Parampanel1.Padding = new Padding(3);
            Parampanel1.Size = new Size(161, 360);
            Parampanel1.TabIndex = 8;
            // 
            // Locationshowlabel
            // 
            Locationshowlabel.BorderStyle = BorderStyle.FixedSingle;
            Locationshowlabel.DataBindings.Add(new Binding("Text", imageLabelBindingSource, "Position", true));
            Locationshowlabel.Dock = DockStyle.Top;
            Locationshowlabel.Location = new Point(3, 162);
            Locationshowlabel.Margin = new Padding(3);
            Locationshowlabel.Name = "Locationshowlabel";
            Locationshowlabel.Size = new Size(155, 45);
            Locationshowlabel.TabIndex = 5;
            // 
            // Locationlabel
            // 
            Locationlabel.AutoSize = true;
            Locationlabel.Dock = DockStyle.Top;
            Locationlabel.Font = new Font("Microsoft YaHei UI", 10F);
            Locationlabel.Location = new Point(3, 142);
            Locationlabel.Name = "Locationlabel";
            Locationlabel.Size = new Size(37, 20);
            Locationlabel.TabIndex = 4;
            Locationlabel.Text = "位置";
            // 
            // GroupcomboBox
            // 
            GroupcomboBox.DataBindings.Add(new Binding("Text", imageLabelBindingSource, "Group", true, DataSourceUpdateMode.OnPropertyChanged));
            GroupcomboBox.Dock = DockStyle.Top;
            GroupcomboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            GroupcomboBox.FormattingEnabled = true;
            GroupcomboBox.Items.AddRange(new object[] { "框内", "框外" });
            GroupcomboBox.Location = new Point(3, 117);
            GroupcomboBox.Name = "GroupcomboBox";
            GroupcomboBox.Size = new Size(155, 25);
            GroupcomboBox.TabIndex = 9;
            // 
            // Grouplabel
            // 
            Grouplabel.AutoSize = true;
            Grouplabel.Dock = DockStyle.Top;
            Grouplabel.Font = new Font("Microsoft YaHei UI", 10F);
            Grouplabel.Location = new Point(3, 97);
            Grouplabel.Name = "Grouplabel";
            Grouplabel.Size = new Size(37, 20);
            Grouplabel.TabIndex = 8;
            Grouplabel.Text = "组别";
            // 
            // FontsizecomboBox
            // 
            FontsizecomboBox.DataBindings.Add(new Binding("Text", imageLabelBindingSource, "FontSize", true, DataSourceUpdateMode.OnPropertyChanged));
            FontsizecomboBox.Dock = DockStyle.Top;
            FontsizecomboBox.FormattingEnabled = true;
            FontsizecomboBox.Location = new Point(3, 72);
            FontsizecomboBox.Name = "FontsizecomboBox";
            FontsizecomboBox.Size = new Size(155, 25);
            FontsizecomboBox.TabIndex = 3;
            // 
            // Fontsizelabel
            // 
            Fontsizelabel.AutoSize = true;
            Fontsizelabel.Dock = DockStyle.Top;
            Fontsizelabel.Font = new Font("Microsoft YaHei UI", 10F);
            Fontsizelabel.Location = new Point(3, 52);
            Fontsizelabel.Margin = new Padding(3);
            Fontsizelabel.Name = "Fontsizelabel";
            Fontsizelabel.Size = new Size(37, 20);
            Fontsizelabel.TabIndex = 2;
            Fontsizelabel.Text = "字号";
            // 
            // FontstylecomboBox
            // 
            FontstylecomboBox.DataBindings.Add(new Binding("Text", imageLabelBindingSource, "FontFamily", true, DataSourceUpdateMode.OnPropertyChanged));
            FontstylecomboBox.Dock = DockStyle.Top;
            FontstylecomboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            FontstylecomboBox.Font = new Font("Microsoft YaHei UI", 12F);
            FontstylecomboBox.FormattingEnabled = true;
            FontstylecomboBox.ItemHeight = 21;
            FontstylecomboBox.Location = new Point(3, 23);
            FontstylecomboBox.Name = "FontstylecomboBox";
            FontstylecomboBox.Size = new Size(155, 29);
            FontstylecomboBox.TabIndex = 1;
            // 
            // Fontlabel
            // 
            Fontlabel.AutoSize = true;
            Fontlabel.Dock = DockStyle.Top;
            Fontlabel.Font = new Font("Microsoft YaHei UI", 10F);
            Fontlabel.Location = new Point(3, 3);
            Fontlabel.Margin = new Padding(3);
            Fontlabel.Name = "Fontlabel";
            Fontlabel.Size = new Size(37, 20);
            Fontlabel.TabIndex = 0;
            Fontlabel.Text = "字体";
            // 
            // RemarktextBox
            // 
            RemarktextBox.BackColor = Color.AntiqueWhite;
            RemarktextBox.BorderStyle = BorderStyle.FixedSingle;
            RemarktextBox.DataBindings.Add(new Binding("Text", imageLabelBindingSource, "Remark", true, DataSourceUpdateMode.OnPropertyChanged));
            RemarktextBox.Dock = DockStyle.Bottom;
            RemarktextBox.ForeColor = Color.Gray;
            RemarktextBox.Location = new Point(3, 273);
            RemarktextBox.Multiline = true;
            RemarktextBox.Name = "RemarktextBox";
            RemarktextBox.Size = new Size(155, 84);
            RemarktextBox.TabIndex = 7;
            RemarktextBox.Enter += RemarktextBox_Enter;
            RemarktextBox.Leave += RemarktextBox_Leave;
            // 
            // Remarklabel
            // 
            Remarklabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Remarklabel.AutoSize = true;
            Remarklabel.Location = new Point(6, 253);
            Remarklabel.Name = "Remarklabel";
            Remarklabel.Size = new Size(32, 17);
            Remarklabel.TabIndex = 6;
            Remarklabel.Text = "备注";
            // 
            // TextReviewtoolTip
            // 
            TextReviewtoolTip.AutoPopDelay = 5000;
            TextReviewtoolTip.InitialDelay = 0;
            TextReviewtoolTip.OwnerDraw = true;
            TextReviewtoolTip.ReshowDelay = 0;
            TextReviewtoolTip.UseFading = false;
            TextReviewtoolTip.Draw += TextReviewtoolTip_Draw;
            TextReviewtoolTip.Popup += TextReviewtoolTip_Popup;
            // 
            // hoverTimer
            // 
            hoverTimer.Interval = 500;
            hoverTimer.Tick += hoverTimer_Tick;
            // 
            // LabelMinusForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 583);
            Controls.Add(splitContainer1);
            Controls.Add(Tool);
            Controls.Add(Menu);
            Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = Menu;
            Name = "LabelMinusForm";
            Text = "LabelMinus";
            FormClosing += LabelMinusForm_FormClosing;
            FormClosed += LabelMinusForm_FormClosed;
            Load += LabelMinus_Load;
            KeyDown += LabelMinusForm_KeyDown;
            ((System.ComponentModel.ISupportInitialize)PicView).EndInit();
            Menu.ResumeLayout(false);
            Menu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)LabelView).EndInit();
            LabelViewMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)imageLabelBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)PicNameBindingSource).EndInit();
            Tool.ResumeLayout(false);
            Tool.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            bottompanel.ResumeLayout(false);
            bottompanel.PerformLayout();
            Parampanel.ResumeLayout(false);
            Parampanel2.ResumeLayout(false);
            Parampanel2.PerformLayout();
            Parampanel1.ResumeLayout(false);
            Parampanel1.PerformLayout();
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
        private ContextMenuStrip LabelViewMenuStrip;
        private ToolStripMenuItem deleteLabelToolStripMenuItem;
        private ToolStrip toolStrip1;
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
        private ToolStripLabel PicNametoolStripLabel;
        private ToolStripMenuItem SaveAsTranslation;
        private ToolStripLabel StatusLabel;
        private ToolStripComboBox OCRComboBox;
        private ToolStripButton OCRMode;
        private ToolStripButton LabelMode;
        private ToolStripLabel OCRtoolStripLabel;
        private ToolStripButton NP;
        private ToolStripButton LP;
        private ComboBox GroupcomboBox;
        private Label Grouplabel;
        private ToolStripButton ImageReviewButton;
        private ToolStripButton TextReviewMode;
        private ToolTip TextReviewtoolTip;
        private System.Windows.Forms.Timer hoverTimer;
        private BindingSource PicNameBindingSource;
        private Panel bottompanel;
        private Label PicNameLabel;
        private ComboBox PicName;
        private Button LPbutton;
        private Button NPbutton;
        private Button FittoViewbutton;
        private DataGridViewTextBoxColumn LabelIndex;
        private DataGridViewTextBoxColumn LabelText;
        private DataGridViewTextBoxColumn LabelGroup;
        private DataGridViewTextBoxColumn indexDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn textDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn groupDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn positionDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn fontSizeDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn fontFamilyDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn remarkDataGridViewTextBoxColumn;
        private TextBox LabelTextBox;
        private Panel Parampanel2;
        private Label Indexlabel;
    }
}
