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
            OpenNowFolder = new ToolStripMenuItem();
            ModifyMenu = new ToolStripMenuItem();
            ModifyGroup = new ToolStripMenuItem();
            LabelTextFontsize = new ToolStripMenuItem();
            ChangeOCRWeb = new ToolStripMenuItem();
            ExportText = new ToolStripMenuItem();
            ExportOriginal = new ToolStripMenuItem();
            ExportCurrent = new ToolStripMenuItem();
            ExportDiff = new ToolStripMenuItem();
            显示ToolStripMenuItem = new ToolStripMenuItem();
            BothShow = new ToolStripMenuItem();
            LabelViewOnly = new ToolStripMenuItem();
            LabelTextBoxOnly = new ToolStripMenuItem();
            ParamHide = new ToolStripMenuItem();
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
            originalTextDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            LabelViewMenuStrip = new ContextMenuStrip(components);
            deleteLabelToolStripMenuItem = new ToolStripMenuItem();
            imageLabelBindingSource = new BindingSource(components);
            PicNameBindingSource = new BindingSource(components);
            splitContainer1 = new SplitContainer();
            Picpanel = new Panel();
            bottompanel = new Panel();
            PicNameLabel = new Label();
            PicName = new ComboBox();
            LPbutton = new Button();
            NPbutton = new Button();
            FittoViewbutton = new Button();
            LabelViewpanel = new Panel();
            Parampanel = new Panel();
            GroupPanel = new Panel();
            flowGroups = new FlowLayoutPanel();
            Indexlabel = new Label();
            Textboxpanel = new Panel();
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
            Toolpanel = new Panel();
            DarkorWhiteMode = new Button();
            StatusLabel = new Label();
            ImageReviewButton = new Button();
            OCRComboBox = new ComboBox();
            OCRMode = new Button();
            TextReviewMode = new Button();
            LabelMode = new Button();
            ((System.ComponentModel.ISupportInitialize)PicView).BeginInit();
            Menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LabelView).BeginInit();
            LabelViewMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)imageLabelBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PicNameBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            Picpanel.SuspendLayout();
            bottompanel.SuspendLayout();
            LabelViewpanel.SuspendLayout();
            Parampanel.SuspendLayout();
            GroupPanel.SuspendLayout();
            Textboxpanel.SuspendLayout();
            Parampanel1.SuspendLayout();
            Toolpanel.SuspendLayout();
            SuspendLayout();
            // 
            // PicView
            // 
            PicView.BackColor = Color.SeaShell;
            PicView.BorderStyle = BorderStyle.FixedSingle;
            PicView.Dock = DockStyle.Fill;
            PicView.Location = new Point(0, 0);
            PicView.Name = "PicView";
            PicView.Size = new Size(500, 652);
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
            Menu.BackColor = Color.White;
            Menu.Items.AddRange(new ToolStripItem[] { FileMenu, ModifyMenu, ExportText, 显示ToolStripMenuItem });
            Menu.Location = new Point(0, 0);
            Menu.Name = "Menu";
            Menu.Size = new Size(984, 25);
            Menu.TabIndex = 1;
            Menu.Text = "menuStrip1";
            // 
            // FileMenu
            // 
            FileMenu.DropDownItems.AddRange(new ToolStripItem[] { NewTranslation, OpenTranslation, SaveTranslation, SaveAsTranslation, OpenNowFolder });
            FileMenu.Name = "FileMenu";
            FileMenu.Size = new Size(44, 21);
            FileMenu.Text = "文件";
            // 
            // NewTranslation
            // 
            NewTranslation.Name = "NewTranslation";
            NewTranslation.Size = new Size(180, 22);
            NewTranslation.Text = "新建翻译";
            NewTranslation.Click += NewTranslation_Click;
            // 
            // OpenTranslation
            // 
            OpenTranslation.Name = "OpenTranslation";
            OpenTranslation.Size = new Size(180, 22);
            OpenTranslation.Text = "打开翻译";
            OpenTranslation.Click += OpenTranslation_Click;
            // 
            // SaveTranslation
            // 
            SaveTranslation.Name = "SaveTranslation";
            SaveTranslation.Size = new Size(180, 22);
            SaveTranslation.Text = "保存翻译";
            SaveTranslation.Click += SaveTranslation_Click;
            // 
            // SaveAsTranslation
            // 
            SaveAsTranslation.Name = "SaveAsTranslation";
            SaveAsTranslation.Size = new Size(180, 22);
            SaveAsTranslation.Text = "另存为翻译";
            SaveAsTranslation.Click += SaveAsTranslation_Click;
            // 
            // OpenNowFolder
            // 
            OpenNowFolder.Name = "OpenNowFolder";
            OpenNowFolder.Size = new Size(180, 22);
            OpenNowFolder.Text = "打开工作文件夹";
            OpenNowFolder.Click += OpenNowFolder_Click;
            // 
            // ModifyMenu
            // 
            ModifyMenu.DropDownItems.AddRange(new ToolStripItem[] { ModifyGroup, LabelTextFontsize, ChangeOCRWeb });
            ModifyMenu.Name = "ModifyMenu";
            ModifyMenu.Size = new Size(44, 21);
            ModifyMenu.Text = "修改";
            // 
            // ModifyGroup
            // 
            ModifyGroup.Name = "ModifyGroup";
            ModifyGroup.Size = new Size(160, 22);
            ModifyGroup.Text = "修改组别";
            ModifyGroup.Click += ModifyGroup_Click;
            // 
            // LabelTextFontsize
            // 
            LabelTextFontsize.Name = "LabelTextFontsize";
            LabelTextFontsize.Size = new Size(160, 22);
            LabelTextFontsize.Text = "修改文本框字号";
            LabelTextFontsize.Click += LabelTextFontsize_Click;
            // 
            // ChangeOCRWeb
            // 
            ChangeOCRWeb.Name = "ChangeOCRWeb";
            ChangeOCRWeb.Size = new Size(160, 22);
            ChangeOCRWeb.Text = "修改OCR网站";
            ChangeOCRWeb.Click += ChangeOCRWeb_Click;
            // 
            // ExportText
            // 
            ExportText.DropDownItems.AddRange(new ToolStripItem[] { ExportOriginal, ExportCurrent, ExportDiff });
            ExportText.Name = "ExportText";
            ExportText.Size = new Size(44, 21);
            ExportText.Text = "导出";
            // 
            // ExportOriginal
            // 
            ExportOriginal.Name = "ExportOriginal";
            ExportOriginal.Size = new Size(148, 22);
            ExportOriginal.Text = "导出原翻译";
            ExportOriginal.Click += ExportOriginal_Click;
            // 
            // ExportCurrent
            // 
            ExportCurrent.Name = "ExportCurrent";
            ExportCurrent.Size = new Size(148, 22);
            ExportCurrent.Text = "导出现翻译";
            ExportCurrent.Click += ExportCurrent_Click;
            // 
            // ExportDiff
            // 
            ExportDiff.Name = "ExportDiff";
            ExportDiff.Size = new Size(148, 22);
            ExportDiff.Text = "导出修改文档";
            ExportDiff.Click += ExportDiff_Click;
            // 
            // 显示ToolStripMenuItem
            // 
            显示ToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { BothShow, LabelViewOnly, LabelTextBoxOnly, ParamHide });
            显示ToolStripMenuItem.Name = "显示ToolStripMenuItem";
            显示ToolStripMenuItem.Size = new Size(44, 21);
            显示ToolStripMenuItem.Text = "显示";
            // 
            // BothShow
            // 
            BothShow.Name = "BothShow";
            BothShow.Size = new Size(148, 22);
            BothShow.Text = "列表与文本框";
            // 
            // LabelViewOnly
            // 
            LabelViewOnly.Name = "LabelViewOnly";
            LabelViewOnly.Size = new Size(148, 22);
            LabelViewOnly.Text = "仅标记列表";
            // 
            // LabelTextBoxOnly
            // 
            LabelTextBoxOnly.Name = "LabelTextBoxOnly";
            LabelTextBoxOnly.Size = new Size(148, 22);
            LabelTextBoxOnly.Text = "仅文本框";
            // 
            // ParamHide
            // 
            ParamHide.Name = "ParamHide";
            ParamHide.Size = new Size(148, 22);
            ParamHide.Text = "次要参数";
            // 
            // LabelView
            // 
            LabelView.AllowUserToAddRows = false;
            LabelView.AllowUserToResizeColumns = false;
            LabelView.AllowUserToResizeRows = false;
            LabelView.AutoGenerateColumns = false;
            LabelView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            LabelView.BackgroundColor = Color.PeachPuff;
            LabelView.BorderStyle = BorderStyle.None;
            LabelView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            LabelView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            LabelView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            LabelView.Columns.AddRange(new DataGridViewColumn[] { LabelIndex, LabelText, LabelGroup, indexDataGridViewTextBoxColumn, textDataGridViewTextBoxColumn, groupDataGridViewTextBoxColumn, positionDataGridViewTextBoxColumn, fontSizeDataGridViewTextBoxColumn, fontFamilyDataGridViewTextBoxColumn, remarkDataGridViewTextBoxColumn, originalTextDataGridViewTextBoxColumn });
            LabelView.ContextMenuStrip = LabelViewMenuStrip;
            LabelView.DataSource = imageLabelBindingSource;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(255, 192, 128);
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            LabelView.DefaultCellStyle = dataGridViewCellStyle4;
            LabelView.Dock = DockStyle.Fill;
            LabelView.GridColor = SystemColors.GrayText;
            LabelView.Location = new Point(3, 3);
            LabelView.MultiSelect = false;
            LabelView.Name = "LabelView";
            LabelView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            LabelView.RowHeadersVisible = false;
            LabelView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            LabelView.Size = new Size(464, 188);
            LabelView.TabIndex = 2;
            LabelView.CellMouseDown += LabelView_CellMouseClick;
            // 
            // LabelIndex
            // 
            LabelIndex.DataPropertyName = "Index";
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LabelIndex.DefaultCellStyle = dataGridViewCellStyle2;
            LabelIndex.HeaderText = "序号";
            LabelIndex.MinimumWidth = 60;
            LabelIndex.Name = "LabelIndex";
            LabelIndex.ReadOnly = true;
            LabelIndex.SortMode = DataGridViewColumnSortMode.NotSortable;
            LabelIndex.Width = 60;
            // 
            // LabelText
            // 
            LabelText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            LabelText.DataPropertyName = "Text";
            LabelText.HeaderText = "文本";
            LabelText.Name = "LabelText";
            // 
            // LabelGroup
            // 
            LabelGroup.DataPropertyName = "Group";
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            LabelGroup.DefaultCellStyle = dataGridViewCellStyle3;
            LabelGroup.HeaderText = "组别";
            LabelGroup.Name = "LabelGroup";
            LabelGroup.SortMode = DataGridViewColumnSortMode.NotSortable;
            LabelGroup.Width = 60;
            // 
            // indexDataGridViewTextBoxColumn
            // 
            indexDataGridViewTextBoxColumn.DataPropertyName = "Index";
            indexDataGridViewTextBoxColumn.HeaderText = "序号";
            indexDataGridViewTextBoxColumn.Name = "indexDataGridViewTextBoxColumn";
            indexDataGridViewTextBoxColumn.Visible = false;
            // 
            // textDataGridViewTextBoxColumn
            // 
            textDataGridViewTextBoxColumn.DataPropertyName = "Text";
            textDataGridViewTextBoxColumn.HeaderText = "文本内容";
            textDataGridViewTextBoxColumn.Name = "textDataGridViewTextBoxColumn";
            textDataGridViewTextBoxColumn.Visible = false;
            // 
            // groupDataGridViewTextBoxColumn
            // 
            groupDataGridViewTextBoxColumn.DataPropertyName = "Group";
            groupDataGridViewTextBoxColumn.HeaderText = "分组";
            groupDataGridViewTextBoxColumn.Name = "groupDataGridViewTextBoxColumn";
            groupDataGridViewTextBoxColumn.Visible = false;
            // 
            // positionDataGridViewTextBoxColumn
            // 
            positionDataGridViewTextBoxColumn.DataPropertyName = "Position";
            positionDataGridViewTextBoxColumn.HeaderText = "位置";
            positionDataGridViewTextBoxColumn.Name = "positionDataGridViewTextBoxColumn";
            positionDataGridViewTextBoxColumn.Visible = false;
            // 
            // fontSizeDataGridViewTextBoxColumn
            // 
            fontSizeDataGridViewTextBoxColumn.DataPropertyName = "FontSize";
            fontSizeDataGridViewTextBoxColumn.HeaderText = "字号";
            fontSizeDataGridViewTextBoxColumn.Name = "fontSizeDataGridViewTextBoxColumn";
            fontSizeDataGridViewTextBoxColumn.Visible = false;
            // 
            // fontFamilyDataGridViewTextBoxColumn
            // 
            fontFamilyDataGridViewTextBoxColumn.DataPropertyName = "FontFamily";
            fontFamilyDataGridViewTextBoxColumn.HeaderText = "字体";
            fontFamilyDataGridViewTextBoxColumn.Name = "fontFamilyDataGridViewTextBoxColumn";
            fontFamilyDataGridViewTextBoxColumn.Visible = false;
            // 
            // remarkDataGridViewTextBoxColumn
            // 
            remarkDataGridViewTextBoxColumn.DataPropertyName = "Remark";
            remarkDataGridViewTextBoxColumn.HeaderText = "备注";
            remarkDataGridViewTextBoxColumn.Name = "remarkDataGridViewTextBoxColumn";
            remarkDataGridViewTextBoxColumn.Visible = false;
            // 
            // originalTextDataGridViewTextBoxColumn
            // 
            originalTextDataGridViewTextBoxColumn.DataPropertyName = "OriginalText";
            originalTextDataGridViewTextBoxColumn.HeaderText = "OriginalText";
            originalTextDataGridViewTextBoxColumn.Name = "originalTextDataGridViewTextBoxColumn";
            originalTextDataGridViewTextBoxColumn.ReadOnly = true;
            originalTextDataGridViewTextBoxColumn.Visible = false;
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
            // splitContainer1
            // 
            splitContainer1.BackColor = Color.PeachPuff;
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 67);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(Picpanel);
            splitContainer1.Panel1.Controls.Add(bottompanel);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.BackColor = Color.PeachPuff;
            splitContainer1.Panel2.Controls.Add(LabelViewpanel);
            splitContainer1.Panel2.Controls.Add(Parampanel);
            splitContainer1.Panel2.Margin = new Padding(3);
            splitContainer1.Panel2.Padding = new Padding(0, 0, 10, 0);
            splitContainer1.Size = new Size(984, 694);
            splitContainer1.SplitterDistance = 500;
            splitContainer1.TabIndex = 3;
            // 
            // Picpanel
            // 
            Picpanel.Controls.Add(PicView);
            Picpanel.Dock = DockStyle.Fill;
            Picpanel.Location = new Point(0, 0);
            Picpanel.Name = "Picpanel";
            Picpanel.Size = new Size(500, 652);
            Picpanel.TabIndex = 3;
            // 
            // bottompanel
            // 
            bottompanel.BackColor = Color.PeachPuff;
            bottompanel.Controls.Add(PicNameLabel);
            bottompanel.Controls.Add(PicName);
            bottompanel.Controls.Add(LPbutton);
            bottompanel.Controls.Add(NPbutton);
            bottompanel.Controls.Add(FittoViewbutton);
            bottompanel.Dock = DockStyle.Bottom;
            bottompanel.Location = new Point(0, 652);
            bottompanel.Name = "bottompanel";
            bottompanel.Padding = new Padding(3);
            bottompanel.Size = new Size(500, 42);
            bottompanel.TabIndex = 2;
            // 
            // PicNameLabel
            // 
            PicNameLabel.Anchor = AnchorStyles.Right;
            PicNameLabel.AutoSize = true;
            PicNameLabel.Location = new Point(145, 13);
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
            PicName.DropDownStyle = ComboBoxStyle.DropDownList;
            PicName.Font = new Font("Microsoft YaHei UI", 12F);
            PicName.FormattingEnabled = true;
            PicName.Location = new Point(184, 6);
            PicName.Name = "PicName";
            PicName.Size = new Size(161, 29);
            PicName.TabIndex = 6;
            // 
            // LPbutton
            // 
            LPbutton.Dock = DockStyle.Right;
            LPbutton.Location = new Point(347, 3);
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
            NPbutton.Location = new Point(422, 3);
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
            FittoViewbutton.Location = new Point(3, 3);
            FittoViewbutton.Name = "FittoViewbutton";
            FittoViewbutton.Size = new Size(75, 36);
            FittoViewbutton.TabIndex = 0;
            FittoViewbutton.Text = "适应屏幕";
            FittoViewbutton.UseVisualStyleBackColor = true;
            FittoViewbutton.Click += FittoView;
            // 
            // LabelViewpanel
            // 
            LabelViewpanel.BackColor = Color.Orange;
            LabelViewpanel.Controls.Add(LabelView);
            LabelViewpanel.Dock = DockStyle.Fill;
            LabelViewpanel.Location = new Point(0, 0);
            LabelViewpanel.Name = "LabelViewpanel";
            LabelViewpanel.Padding = new Padding(3);
            LabelViewpanel.Size = new Size(470, 194);
            LabelViewpanel.TabIndex = 5;
            // 
            // Parampanel
            // 
            Parampanel.BackColor = Color.PeachPuff;
            Parampanel.Controls.Add(GroupPanel);
            Parampanel.Controls.Add(Indexlabel);
            Parampanel.Controls.Add(Textboxpanel);
            Parampanel.Controls.Add(Parampanel1);
            Parampanel.Dock = DockStyle.Bottom;
            Parampanel.Location = new Point(0, 194);
            Parampanel.Name = "Parampanel";
            Parampanel.Padding = new Padding(0, 40, 0, 10);
            Parampanel.Size = new Size(470, 500);
            Parampanel.TabIndex = 4;
            // 
            // GroupPanel
            // 
            GroupPanel.Controls.Add(flowGroups);
            GroupPanel.Location = new Point(59, 0);
            GroupPanel.Name = "GroupPanel";
            GroupPanel.Size = new Size(411, 40);
            GroupPanel.TabIndex = 11;
            // 
            // flowGroups
            // 
            flowGroups.AutoScroll = true;
            flowGroups.Dock = DockStyle.Fill;
            flowGroups.Location = new Point(0, 0);
            flowGroups.Name = "flowGroups";
            flowGroups.Size = new Size(411, 40);
            flowGroups.TabIndex = 0;
            // 
            // Indexlabel
            // 
            Indexlabel.DataBindings.Add(new Binding("Text", imageLabelBindingSource, "Index", true));
            Indexlabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 134);
            Indexlabel.Location = new Point(7, 9);
            Indexlabel.Name = "Indexlabel";
            Indexlabel.Size = new Size(50, 24);
            Indexlabel.TabIndex = 10;
            Indexlabel.Text = "999";
            Indexlabel.TextAlign = ContentAlignment.MiddleCenter;
            Indexlabel.Paint += Indexlabel_Paint;
            // 
            // Textboxpanel
            // 
            Textboxpanel.BackColor = Color.Orange;
            Textboxpanel.Controls.Add(LabelTextBox);
            Textboxpanel.Dock = DockStyle.Fill;
            Textboxpanel.Location = new Point(0, 40);
            Textboxpanel.Name = "Textboxpanel";
            Textboxpanel.Padding = new Padding(3);
            Textboxpanel.Size = new Size(309, 450);
            Textboxpanel.TabIndex = 9;
            // 
            // LabelTextBox
            // 
            LabelTextBox.BackColor = Color.AntiqueWhite;
            LabelTextBox.BorderStyle = BorderStyle.None;
            LabelTextBox.DataBindings.Add(new Binding("Text", imageLabelBindingSource, "Text", true, DataSourceUpdateMode.OnPropertyChanged));
            LabelTextBox.Dock = DockStyle.Fill;
            LabelTextBox.Font = new Font("Microsoft YaHei UI", 12F);
            LabelTextBox.ForeColor = Color.Gray;
            LabelTextBox.Location = new Point(3, 3);
            LabelTextBox.Multiline = true;
            LabelTextBox.Name = "LabelTextBox";
            LabelTextBox.Size = new Size(303, 444);
            LabelTextBox.TabIndex = 3;
            LabelTextBox.Enter += TextBox_FocusChanged;
            LabelTextBox.Leave += TextBox_FocusChanged;
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
            Parampanel1.Location = new Point(309, 40);
            Parampanel1.Name = "Parampanel1";
            Parampanel1.Padding = new Padding(3);
            Parampanel1.Size = new Size(161, 450);
            Parampanel1.TabIndex = 8;
            Parampanel1.Visible = false;
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
            RemarktextBox.Location = new Point(3, 363);
            RemarktextBox.Multiline = true;
            RemarktextBox.Name = "RemarktextBox";
            RemarktextBox.Size = new Size(155, 84);
            RemarktextBox.TabIndex = 7;
            RemarktextBox.Enter += TextBox_FocusChanged;
            RemarktextBox.Leave += TextBox_FocusChanged;
            // 
            // Remarklabel
            // 
            Remarklabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Remarklabel.AutoSize = true;
            Remarklabel.Location = new Point(6, 343);
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
            // Toolpanel
            // 
            Toolpanel.Controls.Add(DarkorWhiteMode);
            Toolpanel.Controls.Add(StatusLabel);
            Toolpanel.Controls.Add(ImageReviewButton);
            Toolpanel.Controls.Add(OCRComboBox);
            Toolpanel.Controls.Add(OCRMode);
            Toolpanel.Controls.Add(TextReviewMode);
            Toolpanel.Controls.Add(LabelMode);
            Toolpanel.Dock = DockStyle.Top;
            Toolpanel.Location = new Point(0, 25);
            Toolpanel.Name = "Toolpanel";
            Toolpanel.Padding = new Padding(3);
            Toolpanel.Size = new Size(984, 42);
            Toolpanel.TabIndex = 5;
            // 
            // DarkorWhiteMode
            // 
            DarkorWhiteMode.Dock = DockStyle.Right;
            DarkorWhiteMode.Location = new Point(785, 3);
            DarkorWhiteMode.Name = "DarkorWhiteMode";
            DarkorWhiteMode.Size = new Size(50, 36);
            DarkorWhiteMode.TabIndex = 7;
            DarkorWhiteMode.Text = "黑白";
            DarkorWhiteMode.UseVisualStyleBackColor = true;
            DarkorWhiteMode.Click += DarkorWhiteMode_Click;
            // 
            // StatusLabel
            // 
            StatusLabel.Dock = DockStyle.Right;
            StatusLabel.Location = new Point(835, 3);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new Size(146, 36);
            StatusLabel.TabIndex = 6;
            StatusLabel.Text = "sponsored by No-Hifuu";
            StatusLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ImageReviewButton
            // 
            ImageReviewButton.Anchor = AnchorStyles.Left;
            ImageReviewButton.Location = new Point(442, 6);
            ImageReviewButton.Name = "ImageReviewButton";
            ImageReviewButton.Size = new Size(75, 33);
            ImageReviewButton.TabIndex = 5;
            ImageReviewButton.Text = "图校模式";
            ImageReviewButton.UseVisualStyleBackColor = true;
            ImageReviewButton.Click += ImageReviewButton_Click;
            // 
            // OCRComboBox
            // 
            OCRComboBox.Anchor = AnchorStyles.Left;
            OCRComboBox.Font = new Font("Microsoft YaHei UI", 10F);
            OCRComboBox.FormattingEnabled = true;
            OCRComboBox.Location = new Point(234, 9);
            OCRComboBox.Name = "OCRComboBox";
            OCRComboBox.Size = new Size(202, 27);
            OCRComboBox.TabIndex = 4;
            // 
            // OCRMode
            // 
            OCRMode.Dock = DockStyle.Left;
            OCRMode.Location = new Point(153, 3);
            OCRMode.Name = "OCRMode";
            OCRMode.Size = new Size(75, 36);
            OCRMode.TabIndex = 2;
            OCRMode.Text = "识别模式";
            OCRMode.UseVisualStyleBackColor = true;
            OCRMode.Click += OCRMode_Click;
            // 
            // TextReviewMode
            // 
            TextReviewMode.Dock = DockStyle.Left;
            TextReviewMode.Location = new Point(78, 3);
            TextReviewMode.Name = "TextReviewMode";
            TextReviewMode.Size = new Size(75, 36);
            TextReviewMode.TabIndex = 1;
            TextReviewMode.Text = "文校模式";
            TextReviewMode.UseVisualStyleBackColor = true;
            TextReviewMode.Click += TextReviewMode_Click;
            // 
            // LabelMode
            // 
            LabelMode.Dock = DockStyle.Left;
            LabelMode.Location = new Point(3, 3);
            LabelMode.Name = "LabelMode";
            LabelMode.Size = new Size(75, 36);
            LabelMode.TabIndex = 0;
            LabelMode.Text = "标记模式";
            LabelMode.UseVisualStyleBackColor = true;
            LabelMode.Click += LabelMode_Click;
            // 
            // LabelMinusForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 761);
            Controls.Add(splitContainer1);
            Controls.Add(Toolpanel);
            Controls.Add(Menu);
            Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = Menu;
            Name = "LabelMinusForm";
            Text = "LabelMinus";
            FormClosing += LabelMinusForm_FormClosing;
            FormClosed += LabelMinusForm_FormClosed;
            Load += LabelMinus_Load;
            ((System.ComponentModel.ISupportInitialize)PicView).EndInit();
            Menu.ResumeLayout(false);
            Menu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)LabelView).EndInit();
            LabelViewMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)imageLabelBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)PicNameBindingSource).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            Picpanel.ResumeLayout(false);
            bottompanel.ResumeLayout(false);
            bottompanel.PerformLayout();
            LabelViewpanel.ResumeLayout(false);
            Parampanel.ResumeLayout(false);
            GroupPanel.ResumeLayout(false);
            Textboxpanel.ResumeLayout(false);
            Textboxpanel.PerformLayout();
            Parampanel1.ResumeLayout(false);
            Parampanel1.PerformLayout();
            Toolpanel.ResumeLayout(false);
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
        private ToolStripMenuItem SaveAsTranslation;
        private DataGridView LabelView;
        private SplitContainer splitContainer1;
        private ContextMenuStrip LabelViewMenuStrip;
        private ToolStripMenuItem deleteLabelToolStripMenuItem;
        private ToolStrip toolStrip1;
        private BindingSource imageLabelBindingSource;
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
        private ComboBox GroupcomboBox;
        private Label Grouplabel;
        private ToolTip TextReviewtoolTip;
        private System.Windows.Forms.Timer hoverTimer;
        private BindingSource PicNameBindingSource;
        private Panel bottompanel;
        private Label PicNameLabel;
        private ComboBox PicName;
        private Button LPbutton;
        private Button NPbutton;
        private Button FittoViewbutton;
        private TextBox LabelTextBox;
        private Panel Textboxpanel;
        private Label Indexlabel;
        private Panel LabelViewpanel;
        private Panel Toolpanel;
        private Button TextReviewMode;
        private Button LabelMode;
        private Button DarkorWhiteMode;
        private Label StatusLabel;
        private Button ImageReviewButton;
        private ComboBox OCRComboBox;
        private Button OCRMode;
        private ToolStripMenuItem ModifyMenu;
        private ToolStripMenuItem ModifyGroup;
        private ToolStripMenuItem LabelTextFontsize;
        private Panel Picpanel;
        private ToolStripMenuItem ExportText;
        private ToolStripMenuItem ExportOriginal;
        private ToolStripMenuItem ExportCurrent;
        private ToolStripMenuItem ExportDiff;
        private ToolStripMenuItem OpenNowFolder;
        private Panel GroupPanel;
        private FlowLayoutPanel flowGroups;
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
        private DataGridViewTextBoxColumn originalTextDataGridViewTextBoxColumn;
        private ToolStripMenuItem 显示ToolStripMenuItem;
        private ToolStripMenuItem ChangeOCRWeb;
        private ToolStripMenuItem BothShow;
        private ToolStripMenuItem LabelViewOnly;
        private ToolStripMenuItem LabelTextBoxOnly;
        private ToolStripMenuItem ParamHide;
    }
}
