namespace FrmVpComponents
{
    partial class frmToolBlock
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private System.Windows.Forms.TableLayoutPanel titleLayout;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btn_Save2;
        private System.Windows.Forms.FlowLayoutPanel panel2;
        private System.Windows.Forms.Button btnOpenImage;
        private System.Windows.Forms.Button btnOpenImageFile;
        private System.Windows.Forms.Button btnRunNextImage;
        private System.Windows.Forms.Button btn_连续运行;
        private System.Windows.Forms.Button btn_取消连续运行;
        private System.Windows.Forms.Button btn_拷贝当前图片;
        private System.Windows.Forms.Button btn_双图像;
        private System.Windows.Forms.Button btn_屏蔽主程序传递图片;
        private System.Windows.Forms.TableLayoutPanel contextLayout;
        private System.Windows.Forms.FlowLayoutPanel navigationPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCurrentImageIndex;
        private System.Windows.Forms.Label labCurrentImageIndex;
        private System.Windows.Forms.Panel navigationSeparator;
        private System.Windows.Forms.FlowLayoutPanel monitorPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_Result;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_比较内容;
        private System.Windows.Forms.Panel monitorSeparator;
        private System.Windows.Forms.TableLayoutPanel pathLayout;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lab_当前图片路径;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labfileName;
        private System.Windows.Forms.Panel panel1;
        private Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 cogToolBlockEditV21;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TableLayoutPanel dualImageLayout;
        private System.Windows.Forms.TableLayoutPanel dualImageHeaderLayout;
        private System.Windows.Forms.Label dualImageTitleLabel;
        private System.Windows.Forms.Label dualImageSubtitleLabel;
        private System.Windows.Forms.TableLayoutPanel dualImageGrid;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_图片1输入名称;
        private System.Windows.Forms.Button btn_选择图像1;
        private System.Windows.Forms.Button btnOpenImageFile1;
        private System.Windows.Forms.TextBox txt_文件夹1路径;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_图片2输入名称;
        private System.Windows.Forms.Button btn_选择图像2;
        private System.Windows.Forms.Button btnOpenImageFile2;
        private System.Windows.Forms.TextBox txt_文件夹2路径;
        private System.Windows.Forms.TableLayoutPanel syncLayout;
        private System.Windows.Forms.FlowLayoutPanel syncModePanel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton rdB_文件名完全一致;
        private System.Windows.Forms.RadioButton rdB_创建时间完全一致;
        private System.Windows.Forms.RadioButton rdB_文件2秒可以比文件1晚1S;
        private System.Windows.Forms.NumericUpDown nUD_允许时间差秒数;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.FlowLayoutPanel dualImageOptionsPanel;
        private System.Windows.Forms.CheckBox chb_启用双图像;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ToolTip toolTip1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.headerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.titleLayout = new System.Windows.Forms.TableLayoutPanel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btn_Save2 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOpenImage = new System.Windows.Forms.Button();
            this.btnOpenImageFile = new System.Windows.Forms.Button();
            this.btnRunNextImage = new System.Windows.Forms.Button();
            this.btn_连续运行 = new System.Windows.Forms.Button();
            this.btn_取消连续运行 = new System.Windows.Forms.Button();
            this.btn_拷贝当前图片 = new System.Windows.Forms.Button();
            this.btn_双图像 = new System.Windows.Forms.Button();
            this.btn_屏蔽主程序传递图片 = new System.Windows.Forms.Button();
            this.contextLayout = new System.Windows.Forms.TableLayoutPanel();
            this.navigationPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCurrentImageIndex = new System.Windows.Forms.TextBox();
            this.labCurrentImageIndex = new System.Windows.Forms.Label();
            this.navigationSeparator = new System.Windows.Forms.Panel();
            this.monitorPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_Result = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_比较内容 = new System.Windows.Forms.TextBox();
            this.monitorSeparator = new System.Windows.Forms.Panel();
            this.pathLayout = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.lab_当前图片路径 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labfileName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cogToolBlockEditV21 = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dualImageLayout = new System.Windows.Forms.TableLayoutPanel();
            this.dualImageHeaderLayout = new System.Windows.Forms.TableLayoutPanel();
            this.dualImageTitleLabel = new System.Windows.Forms.Label();
            this.dualImageSubtitleLabel = new System.Windows.Forms.Label();
            this.dualImageGrid = new System.Windows.Forms.TableLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_图片1输入名称 = new System.Windows.Forms.TextBox();
            this.btn_选择图像1 = new System.Windows.Forms.Button();
            this.btnOpenImageFile1 = new System.Windows.Forms.Button();
            this.txt_文件夹1路径 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_图片2输入名称 = new System.Windows.Forms.TextBox();
            this.btn_选择图像2 = new System.Windows.Forms.Button();
            this.btnOpenImageFile2 = new System.Windows.Forms.Button();
            this.txt_文件夹2路径 = new System.Windows.Forms.TextBox();
            this.syncLayout = new System.Windows.Forms.TableLayoutPanel();
            this.syncModePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.rdB_文件名完全一致 = new System.Windows.Forms.RadioButton();
            this.rdB_创建时间完全一致 = new System.Windows.Forms.RadioButton();
            this.rdB_文件2秒可以比文件1晚1S = new System.Windows.Forms.RadioButton();
            this.nUD_允许时间差秒数 = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.dualImageOptionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.chb_启用双图像 = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.headerLayout.SuspendLayout();
            this.titleLayout.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextLayout.SuspendLayout();
            this.navigationPanel.SuspendLayout();
            this.monitorPanel.SuspendLayout();
            this.pathLayout.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).BeginInit();
            this.panel3.SuspendLayout();
            this.dualImageLayout.SuspendLayout();
            this.dualImageHeaderLayout.SuspendLayout();
            this.dualImageGrid.SuspendLayout();
            this.syncLayout.SuspendLayout();
            this.syncModePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_允许时间差秒数)).BeginInit();
            this.dualImageOptionsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.headerLayout, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.contextLayout, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1280, 800);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // headerLayout
            // 
            this.headerLayout.BackColor = System.Drawing.Color.White;
            this.headerLayout.ColumnCount = 3;
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 124F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.headerLayout.Controls.Add(this.titleLayout, 0, 0);
            this.headerLayout.Controls.Add(this.btnSave, 1, 0);
            this.headerLayout.Controls.Add(this.btn_Save2, 2, 0);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayout.Location = new System.Drawing.Point(0, 0);
            this.headerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.headerLayout.RowCount = 1;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Size = new System.Drawing.Size(1280, 70);
            this.headerLayout.TabIndex = 0;
            // 
            // titleLayout
            // 
            this.titleLayout.ColumnCount = 1;
            this.titleLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.titleLayout.Controls.Add(this.titleLabel, 0, 0);
            this.titleLayout.Controls.Add(this.subtitleLabel, 0, 1);
            this.titleLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleLayout.Location = new System.Drawing.Point(20, 10);
            this.titleLayout.Margin = new System.Windows.Forms.Padding(0);
            this.titleLayout.Name = "titleLayout";
            this.titleLayout.RowCount = 2;
            this.titleLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.titleLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.titleLayout.Size = new System.Drawing.Size(976, 50);
            this.titleLayout.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoEllipsis = true;
            this.titleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            this.titleLabel.Location = new System.Drawing.Point(0, 0);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(976, 29);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "视觉工具调试";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.AutoEllipsis = true;
            this.subtitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subtitleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.5F);
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.subtitleLabel.Location = new System.Drawing.Point(0, 29);
            this.subtitleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(976, 21);
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "ToolBlock 参数编辑、离线图像调试与运行验证";
            this.subtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(24, 68, 190);
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(28, 78, 216);
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(1008, 17);
            this.btnSave.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(112, 36);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "保存工具";
            this.toolTip1.SetToolTip(this.btnSave, "将完整 ToolBlock 保存为 VPP 文件");
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btn_Save2
            // 
            this.btn_Save2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_Save2.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.btn_Save2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_Save2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(211, 218, 228);
            this.btn_Save2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(225, 232, 242);
            this.btn_Save2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(238, 243, 249);
            this.btn_Save2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Save2.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btn_Save2.ForeColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.btn_Save2.Location = new System.Drawing.Point(1132, 17);
            this.btn_Save2.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btn_Save2.Name = "btn_Save2";
            this.btn_Save2.Size = new System.Drawing.Size(128, 36);
            this.btn_Save2.TabIndex = 2;
            this.btn_Save2.Text = "精简保存";
            this.toolTip1.SetToolTip(this.btn_Save2, "保存为不带图像数据的 ToolBlock 文件");
            this.btn_Save2.UseVisualStyleBackColor = false;
            this.btn_Save2.Click += new System.EventHandler(this.btn_Save2_Click);
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.panel2.Controls.Add(this.btnOpenImage);
            this.panel2.Controls.Add(this.btnOpenImageFile);
            this.panel2.Controls.Add(this.btnRunNextImage);
            this.panel2.Controls.Add(this.btn_连续运行);
            this.panel2.Controls.Add(this.btn_取消连续运行);
            this.panel2.Controls.Add(this.btn_拷贝当前图片);
            this.panel2.Controls.Add(this.btn_双图像);
            this.panel2.Controls.Add(this.btn_屏蔽主程序传递图片);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 70);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.panel2.Size = new System.Drawing.Size(1280, 58);
            this.panel2.TabIndex = 1;
            this.panel2.WrapContents = false;
            // 
            // btnOpenImage
            // 
            this.btnOpenImage.BackColor = System.Drawing.Color.White;
            this.btnOpenImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenImage.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(211, 218, 228);
            this.btnOpenImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenImage.Location = new System.Drawing.Point(16, 10);
            this.btnOpenImage.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnOpenImage.Name = "btnOpenImage";
            this.btnOpenImage.Size = new System.Drawing.Size(96, 36);
            this.btnOpenImage.TabIndex = 0;
            this.btnOpenImage.Text = "选择图像";
            this.toolTip1.SetToolTip(this.btnOpenImage, "选择一张图像用于离线运行");
            this.btnOpenImage.UseVisualStyleBackColor = false;
            this.btnOpenImage.Click += new System.EventHandler(this.btnOpenImage_Click);
            // 
            // btnOpenImageFile
            // 
            this.btnOpenImageFile.BackColor = System.Drawing.Color.White;
            this.btnOpenImageFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenImageFile.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(211, 218, 228);
            this.btnOpenImageFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenImageFile.Location = new System.Drawing.Point(120, 10);
            this.btnOpenImageFile.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnOpenImageFile.Name = "btnOpenImageFile";
            this.btnOpenImageFile.Size = new System.Drawing.Size(108, 36);
            this.btnOpenImageFile.TabIndex = 1;
            this.btnOpenImageFile.Text = "选择文件夹";
            this.toolTip1.SetToolTip(this.btnOpenImageFile, "选择图像文件夹用于顺序调试");
            this.btnOpenImageFile.UseVisualStyleBackColor = false;
            this.btnOpenImageFile.Click += new System.EventHandler(this.btnOpenImageFile_Click);
            // 
            // btnRunNextImage
            // 
            this.btnRunNextImage.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnRunNextImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRunNextImage.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.btnRunNextImage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(21, 128, 61);
            this.btnRunNextImage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(22, 142, 65);
            this.btnRunNextImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRunNextImage.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRunNextImage.ForeColor = System.Drawing.Color.White;
            this.btnRunNextImage.Location = new System.Drawing.Point(236, 10);
            this.btnRunNextImage.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnRunNextImage.Name = "btnRunNextImage";
            this.btnRunNextImage.Size = new System.Drawing.Size(88, 36);
            this.btnRunNextImage.TabIndex = 2;
            this.btnRunNextImage.Text = "运行当前";
            this.btnRunNextImage.UseVisualStyleBackColor = false;
            this.btnRunNextImage.Click += new System.EventHandler(this.btnRunNextImage_Click);
            // 
            // btn_连续运行
            // 
            this.btn_连续运行.BackColor = System.Drawing.Color.White;
            this.btn_连续运行.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_连续运行.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(147, 197, 253);
            this.btn_连续运行.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_连续运行.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.btn_连续运行.Location = new System.Drawing.Point(332, 10);
            this.btn_连续运行.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btn_连续运行.Name = "btn_连续运行";
            this.btn_连续运行.Size = new System.Drawing.Size(104, 36);
            this.btn_连续运行.TabIndex = 3;
            this.btn_连续运行.Text = "连续运行";
            this.toolTip1.SetToolTip(this.btn_连续运行, "连续运行直到监控输出不等于指定结果");
            this.btn_连续运行.UseVisualStyleBackColor = false;
            this.btn_连续运行.Click += new System.EventHandler(this.btn_连续运行_Click);
            // 
            // btn_取消连续运行
            // 
            this.btn_取消连续运行.BackColor = System.Drawing.Color.White;
            this.btn_取消连续运行.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_取消连续运行.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(252, 165, 165);
            this.btn_取消连续运行.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_取消连续运行.ForeColor = System.Drawing.Color.FromArgb(185, 28, 28);
            this.btn_取消连续运行.Location = new System.Drawing.Point(444, 10);
            this.btn_取消连续运行.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btn_取消连续运行.Name = "btn_取消连续运行";
            this.btn_取消连续运行.Size = new System.Drawing.Size(104, 36);
            this.btn_取消连续运行.TabIndex = 4;
            this.btn_取消连续运行.Text = "停止连续";
            this.btn_取消连续运行.UseVisualStyleBackColor = false;
            this.btn_取消连续运行.Click += new System.EventHandler(this.btn_取消连续运行_Click);
            // 
            // btn_拷贝当前图片
            // 
            this.btn_拷贝当前图片.BackColor = System.Drawing.Color.White;
            this.btn_拷贝当前图片.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_拷贝当前图片.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(211, 218, 228);
            this.btn_拷贝当前图片.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_拷贝当前图片.Location = new System.Drawing.Point(556, 10);
            this.btn_拷贝当前图片.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btn_拷贝当前图片.Name = "btn_拷贝当前图片";
            this.btn_拷贝当前图片.Size = new System.Drawing.Size(120, 36);
            this.btn_拷贝当前图片.TabIndex = 5;
            this.btn_拷贝当前图片.Text = "复制当前图像";
            this.toolTip1.SetToolTip(this.btn_拷贝当前图片, "将当前图像复制到标注目录");
            this.btn_拷贝当前图片.UseVisualStyleBackColor = false;
            this.btn_拷贝当前图片.Click += new System.EventHandler(this.btn_拷贝当前图片_Click);
            // 
            // btn_双图像
            // 
            this.btn_双图像.BackColor = System.Drawing.Color.FromArgb(239, 246, 255);
            this.btn_双图像.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_双图像.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(147, 197, 253);
            this.btn_双图像.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_双图像.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.btn_双图像.Location = new System.Drawing.Point(684, 10);
            this.btn_双图像.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btn_双图像.Name = "btn_双图像";
            this.btn_双图像.Size = new System.Drawing.Size(112, 36);
            this.btn_双图像.TabIndex = 6;
            this.btn_双图像.Text = "双图像配置";
            this.btn_双图像.UseVisualStyleBackColor = false;
            this.btn_双图像.Click += new System.EventHandler(this.btn_双图像_Click);
            // 
            // btn_屏蔽主程序传递图片
            // 
            this.btn_屏蔽主程序传递图片.BackColor = System.Drawing.Color.White;
            this.btn_屏蔽主程序传递图片.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_屏蔽主程序传递图片.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(211, 218, 228);
            this.btn_屏蔽主程序传递图片.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_屏蔽主程序传递图片.Location = new System.Drawing.Point(804, 10);
            this.btn_屏蔽主程序传递图片.Margin = new System.Windows.Forms.Padding(0);
            this.btn_屏蔽主程序传递图片.Name = "btn_屏蔽主程序传递图片";
            this.btn_屏蔽主程序传递图片.Size = new System.Drawing.Size(160, 36);
            this.btn_屏蔽主程序传递图片.TabIndex = 7;
            this.btn_屏蔽主程序传递图片.Text = "独立调试副本";
            this.toolTip1.SetToolTip(this.btn_屏蔽主程序传递图片, "创建不接收主程序图像的独立调试副本");
            this.btn_屏蔽主程序传递图片.UseVisualStyleBackColor = false;
            this.btn_屏蔽主程序传递图片.Click += new System.EventHandler(this.btn_屏蔽主程序传递图片_Click);
            // 
            // contextLayout
            // 
            this.contextLayout.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.contextLayout.ColumnCount = 5;
            this.contextLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.contextLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.contextLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 340F));
            this.contextLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.contextLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contextLayout.Controls.Add(this.navigationPanel, 0, 0);
            this.contextLayout.Controls.Add(this.navigationSeparator, 1, 0);
            this.contextLayout.Controls.Add(this.monitorPanel, 2, 0);
            this.contextLayout.Controls.Add(this.monitorSeparator, 3, 0);
            this.contextLayout.Controls.Add(this.pathLayout, 4, 0);
            this.contextLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contextLayout.Location = new System.Drawing.Point(0, 128);
            this.contextLayout.Margin = new System.Windows.Forms.Padding(0);
            this.contextLayout.Name = "contextLayout";
            this.contextLayout.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.contextLayout.RowCount = 1;
            this.contextLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contextLayout.Size = new System.Drawing.Size(1280, 68);
            this.contextLayout.TabIndex = 2;
            // 
            // navigationPanel
            // 
            this.navigationPanel.Controls.Add(this.label1);
            this.navigationPanel.Controls.Add(this.txtCurrentImageIndex);
            this.navigationPanel.Controls.Add(this.labCurrentImageIndex);
            this.navigationPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationPanel.Location = new System.Drawing.Point(16, 8);
            this.navigationPanel.Margin = new System.Windows.Forms.Padding(0);
            this.navigationPanel.Name = "navigationPanel";
            this.navigationPanel.Padding = new System.Windows.Forms.Padding(0, 9, 0, 0);
            this.navigationPanel.Size = new System.Drawing.Size(240, 52);
            this.navigationPanel.TabIndex = 0;
            this.navigationPanel.WrapContents = false;
            // 
            // label1
            // 
            this.label1.AutoSize = false;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.label1.Location = new System.Drawing.Point(0, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "图像序号";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtCurrentImageIndex
            // 
            this.txtCurrentImageIndex.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.txtCurrentImageIndex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCurrentImageIndex.Location = new System.Drawing.Point(108, 9);
            this.txtCurrentImageIndex.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.txtCurrentImageIndex.Name = "txtCurrentImageIndex";
            this.txtCurrentImageIndex.Size = new System.Drawing.Size(54, 27);
            this.txtCurrentImageIndex.TabIndex = 1;
            this.txtCurrentImageIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtCurrentImageIndex, "输入序号后按 Enter 定位图像");
            this.txtCurrentImageIndex.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCurrentImageIndex_KeyPress);
            // 
            // labCurrentImageIndex
            // 
            this.labCurrentImageIndex.AutoSize = false;
            this.labCurrentImageIndex.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.labCurrentImageIndex.ForeColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.labCurrentImageIndex.Location = new System.Drawing.Point(168, 9);
            this.labCurrentImageIndex.Margin = new System.Windows.Forms.Padding(0);
            this.labCurrentImageIndex.Name = "labCurrentImageIndex";
            this.labCurrentImageIndex.Size = new System.Drawing.Size(64, 30);
            this.labCurrentImageIndex.TabIndex = 2;
            this.labCurrentImageIndex.Text = "/1";
            this.labCurrentImageIndex.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // navigationSeparator
            // 
            this.navigationSeparator.BackColor = System.Drawing.Color.FromArgb(218, 224, 232);
            this.navigationSeparator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationSeparator.Location = new System.Drawing.Point(256, 18);
            this.navigationSeparator.Margin = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.navigationSeparator.Name = "navigationSeparator";
            this.navigationSeparator.Size = new System.Drawing.Size(1, 32);
            this.navigationSeparator.TabIndex = 1;
            // 
            // monitorPanel
            // 
            this.monitorPanel.Controls.Add(this.label4);
            this.monitorPanel.Controls.Add(this.txt_Result);
            this.monitorPanel.Controls.Add(this.label5);
            this.monitorPanel.Controls.Add(this.txt_比较内容);
            this.monitorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monitorPanel.Location = new System.Drawing.Point(257, 8);
            this.monitorPanel.Margin = new System.Windows.Forms.Padding(0);
            this.monitorPanel.Name = "monitorPanel";
            this.monitorPanel.Padding = new System.Windows.Forms.Padding(0, 9, 0, 0);
            this.monitorPanel.Size = new System.Drawing.Size(340, 52);
            this.monitorPanel.TabIndex = 2;
            this.monitorPanel.WrapContents = false;
            // 
            // label4
            // 
            this.label4.AutoSize = false;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.label4.Location = new System.Drawing.Point(0, 9);
            this.label4.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 30);
            this.label4.TabIndex = 0;
            this.label4.Text = "监控输出";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txt_Result
            // 
            this.txt_Result.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.txt_Result.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_Result.Location = new System.Drawing.Point(108, 9);
            this.txt_Result.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.txt_Result.Name = "txt_Result";
            this.txt_Result.Size = new System.Drawing.Size(92, 27);
            this.txt_Result.TabIndex = 1;
            this.txt_Result.Text = "Result";
            this.toolTip1.SetToolTip(this.txt_Result, "ToolBlock 输出名称");
            // 
            // label5
            // 
            this.label5.AutoSize = false;
            this.label5.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.label5.Location = new System.Drawing.Point(208, 9);
            this.label5.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 30);
            this.label5.TabIndex = 2;
            this.label5.Text = "!=";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txt_比较内容
            // 
            this.txt_比较内容.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.txt_比较内容.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_比较内容.Location = new System.Drawing.Point(254, 9);
            this.txt_比较内容.Margin = new System.Windows.Forms.Padding(0);
            this.txt_比较内容.Name = "txt_比较内容";
            this.txt_比较内容.Size = new System.Drawing.Size(84, 27);
            this.txt_比较内容.TabIndex = 3;
            this.txt_比较内容.Text = "True";
            this.toolTip1.SetToolTip(this.txt_比较内容, "连续运行的停止比较值");
            // 
            // monitorSeparator
            // 
            this.monitorSeparator.BackColor = System.Drawing.Color.FromArgb(218, 224, 232);
            this.monitorSeparator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monitorSeparator.Location = new System.Drawing.Point(597, 18);
            this.monitorSeparator.Margin = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.monitorSeparator.Name = "monitorSeparator";
            this.monitorSeparator.Size = new System.Drawing.Size(1, 32);
            this.monitorSeparator.TabIndex = 3;
            // 
            // pathLayout
            // 
            this.pathLayout.ColumnCount = 2;
            this.pathLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.pathLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pathLayout.Controls.Add(this.label2, 0, 0);
            this.pathLayout.Controls.Add(this.lab_当前图片路径, 1, 0);
            this.pathLayout.Controls.Add(this.label3, 0, 1);
            this.pathLayout.Controls.Add(this.labfileName, 1, 1);
            this.pathLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathLayout.Location = new System.Drawing.Point(598, 8);
            this.pathLayout.Margin = new System.Windows.Forms.Padding(0);
            this.pathLayout.Name = "pathLayout";
            this.pathLayout.RowCount = 2;
            this.pathLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pathLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pathLayout.Size = new System.Drawing.Size(666, 52);
            this.pathLayout.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 26);
            this.label2.TabIndex = 0;
            this.label2.Text = "图像";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip1.SetToolTip(this.label2, "双击打开当前图像目录");
            this.label2.DoubleClick += new System.EventHandler(this.label2_DoubleClick);
            // 
            // lab_当前图片路径
            // 
            this.lab_当前图片路径.AutoEllipsis = true;
            this.lab_当前图片路径.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lab_当前图片路径.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.lab_当前图片路径.Location = new System.Drawing.Point(58, 0);
            this.lab_当前图片路径.Margin = new System.Windows.Forms.Padding(0);
            this.lab_当前图片路径.Name = "lab_当前图片路径";
            this.lab_当前图片路径.Size = new System.Drawing.Size(608, 26);
            this.lab_当前图片路径.TabIndex = 1;
            this.lab_当前图片路径.Text = "尚未选择图像";
            this.lab_当前图片路径.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.label3.Location = new System.Drawing.Point(0, 26);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 26);
            this.label3.TabIndex = 2;
            this.label3.Text = "工具";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip1.SetToolTip(this.label3, "双击打开 ToolBlock 文件目录");
            this.label3.DoubleClick += new System.EventHandler(this.label3_DoubleClick);
            // 
            // labfileName
            // 
            this.labfileName.AutoEllipsis = true;
            this.labfileName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labfileName.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.labfileName.Location = new System.Drawing.Point(58, 26);
            this.labfileName.Margin = new System.Windows.Forms.Padding(0);
            this.labfileName.Name = "labfileName";
            this.labfileName.Size = new System.Drawing.Size(608, 26);
            this.labfileName.TabIndex = 3;
            this.labfileName.Text = "ToolBlock1";
            this.labfileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(211, 218, 228);
            this.panel1.Controls.Add(this.cogToolBlockEditV21);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(16, 210);
            this.panel1.Margin = new System.Windows.Forms.Padding(16, 14, 16, 16);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(1);
            this.panel1.Size = new System.Drawing.Size(1248, 574);
            this.panel1.TabIndex = 3;
            // 
            // cogToolBlockEditV21
            // 
            this.cogToolBlockEditV21.AllowDrop = true;
            this.cogToolBlockEditV21.ContextMenuCustomizer = null;
            this.cogToolBlockEditV21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogToolBlockEditV21.Location = new System.Drawing.Point(1, 1);
            this.cogToolBlockEditV21.Margin = new System.Windows.Forms.Padding(0);
            this.cogToolBlockEditV21.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogToolBlockEditV21.Name = "cogToolBlockEditV21";
            this.cogToolBlockEditV21.ShowNodeToolTips = true;
            this.cogToolBlockEditV21.Size = new System.Drawing.Size(1246, 572);
            this.cogToolBlockEditV21.SuspendElectricRuns = false;
            this.cogToolBlockEditV21.TabIndex = 0;
            this.cogToolBlockEditV21.Load += new System.EventHandler(this.cogToolBlockEditV21_Load);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.dualImageLayout);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(1, 1);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(24);
            this.panel3.Size = new System.Drawing.Size(1246, 572);
            this.panel3.TabIndex = 1;
            this.panel3.Visible = false;
            // 
            // dualImageLayout
            // 
            this.dualImageLayout.ColumnCount = 1;
            this.dualImageLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dualImageLayout.Controls.Add(this.dualImageHeaderLayout, 0, 0);
            this.dualImageLayout.Controls.Add(this.dualImageGrid, 0, 1);
            this.dualImageLayout.Controls.Add(this.syncLayout, 0, 2);
            this.dualImageLayout.Controls.Add(this.label11, 0, 3);
            this.dualImageLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dualImageLayout.Location = new System.Drawing.Point(24, 24);
            this.dualImageLayout.Margin = new System.Windows.Forms.Padding(0);
            this.dualImageLayout.Name = "dualImageLayout";
            this.dualImageLayout.RowCount = 4;
            this.dualImageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.dualImageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.dualImageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.dualImageLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dualImageLayout.Size = new System.Drawing.Size(1198, 524);
            this.dualImageLayout.TabIndex = 0;
            // 
            // dualImageHeaderLayout
            // 
            this.dualImageHeaderLayout.ColumnCount = 1;
            this.dualImageHeaderLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dualImageHeaderLayout.Controls.Add(this.dualImageTitleLabel, 0, 0);
            this.dualImageHeaderLayout.Controls.Add(this.dualImageSubtitleLabel, 0, 1);
            this.dualImageHeaderLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dualImageHeaderLayout.Location = new System.Drawing.Point(0, 0);
            this.dualImageHeaderLayout.Margin = new System.Windows.Forms.Padding(0);
            this.dualImageHeaderLayout.Name = "dualImageHeaderLayout";
            this.dualImageHeaderLayout.RowCount = 2;
            this.dualImageHeaderLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.dualImageHeaderLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dualImageHeaderLayout.Size = new System.Drawing.Size(1198, 62);
            this.dualImageHeaderLayout.TabIndex = 0;
            // 
            // dualImageTitleLabel
            // 
            this.dualImageTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dualImageTitleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold);
            this.dualImageTitleLabel.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            this.dualImageTitleLabel.Location = new System.Drawing.Point(0, 0);
            this.dualImageTitleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.dualImageTitleLabel.Name = "dualImageTitleLabel";
            this.dualImageTitleLabel.Size = new System.Drawing.Size(1198, 32);
            this.dualImageTitleLabel.TabIndex = 0;
            this.dualImageTitleLabel.Text = "双图像输入配置";
            this.dualImageTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dualImageSubtitleLabel
            // 
            this.dualImageSubtitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dualImageSubtitleLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.dualImageSubtitleLabel.Location = new System.Drawing.Point(0, 32);
            this.dualImageSubtitleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.dualImageSubtitleLabel.Name = "dualImageSubtitleLabel";
            this.dualImageSubtitleLabel.Size = new System.Drawing.Size(1198, 30);
            this.dualImageSubtitleLabel.TabIndex = 1;
            this.dualImageSubtitleLabel.Text = "配置两个 ToolBlock 图像输入及离线图像匹配方式";
            this.dualImageSubtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dualImageGrid
            // 
            this.dualImageGrid.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.dualImageGrid.ColumnCount = 5;
            this.dualImageGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 132F));
            this.dualImageGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 174F));
            this.dualImageGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.dualImageGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.dualImageGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.dualImageGrid.Controls.Add(this.label6, 0, 0);
            this.dualImageGrid.Controls.Add(this.txt_图片1输入名称, 1, 0);
            this.dualImageGrid.Controls.Add(this.btn_选择图像1, 2, 0);
            this.dualImageGrid.Controls.Add(this.btnOpenImageFile1, 3, 0);
            this.dualImageGrid.Controls.Add(this.txt_文件夹1路径, 4, 0);
            this.dualImageGrid.Controls.Add(this.label7, 0, 1);
            this.dualImageGrid.Controls.Add(this.txt_图片2输入名称, 1, 1);
            this.dualImageGrid.Controls.Add(this.btn_选择图像2, 2, 1);
            this.dualImageGrid.Controls.Add(this.btnOpenImageFile2, 3, 1);
            this.dualImageGrid.Controls.Add(this.txt_文件夹2路径, 4, 1);
            this.dualImageGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dualImageGrid.Location = new System.Drawing.Point(0, 62);
            this.dualImageGrid.Margin = new System.Windows.Forms.Padding(0);
            this.dualImageGrid.Name = "dualImageGrid";
            this.dualImageGrid.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.dualImageGrid.RowCount = 2;
            this.dualImageGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dualImageGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dualImageGrid.Size = new System.Drawing.Size(1198, 112);
            this.dualImageGrid.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.label6.Location = new System.Drawing.Point(12, 8);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(132, 48);
            this.label6.TabIndex = 0;
            this.label6.Text = "图像 1 输入名称";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txt_图片1输入名称
            // 
            this.txt_图片1输入名称.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_图片1输入名称.BackColor = System.Drawing.Color.White;
            this.txt_图片1输入名称.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_图片1输入名称.Location = new System.Drawing.Point(144, 18);
            this.txt_图片1输入名称.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.txt_图片1输入名称.Name = "txt_图片1输入名称";
            this.txt_图片1输入名称.Size = new System.Drawing.Size(162, 27);
            this.txt_图片1输入名称.TabIndex = 1;
            this.txt_图片1输入名称.Text = "InputImage1";
            this.toolTip1.SetToolTip(this.txt_图片1输入名称, "ToolBlock 的第一个图像输入名称");
            this.txt_图片1输入名称.TextChanged += new System.EventHandler(this.txt_图片1输入名称_TextChanged);
            // 
            // btn_选择图像1
            // 
            this.btn_选择图像1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_选择图像1.BackColor = System.Drawing.Color.White;
            this.btn_选择图像1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_选择图像1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(211, 218, 228);
            this.btn_选择图像1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_选择图像1.Location = new System.Drawing.Point(316, 14);
            this.btn_选择图像1.Margin = new System.Windows.Forms.Padding(0);
            this.btn_选择图像1.Name = "btn_选择图像1";
            this.btn_选择图像1.Size = new System.Drawing.Size(92, 32);
            this.btn_选择图像1.TabIndex = 2;
            this.btn_选择图像1.Text = "选择图像";
            this.btn_选择图像1.UseVisualStyleBackColor = false;
            this.btn_选择图像1.Click += new System.EventHandler(this.btnOpenImage_Click);
            // 
            // btnOpenImageFile1
            // 
            this.btnOpenImageFile1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOpenImageFile1.BackColor = System.Drawing.Color.White;
            this.btnOpenImageFile1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenImageFile1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(211, 218, 228);
            this.btnOpenImageFile1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenImageFile1.Location = new System.Drawing.Point(428, 14);
            this.btnOpenImageFile1.Margin = new System.Windows.Forms.Padding(0);
            this.btnOpenImageFile1.Name = "btnOpenImageFile1";
            this.btnOpenImageFile1.Size = new System.Drawing.Size(102, 32);
            this.btnOpenImageFile1.TabIndex = 3;
            this.btnOpenImageFile1.Text = "选择文件夹";
            this.btnOpenImageFile1.UseVisualStyleBackColor = false;
            this.btnOpenImageFile1.Click += new System.EventHandler(this.btnOpenImageFile1_Click);
            // 
            // txt_文件夹1路径
            // 
            this.txt_文件夹1路径.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_文件夹1路径.BackColor = System.Drawing.Color.White;
            this.txt_文件夹1路径.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_文件夹1路径.Location = new System.Drawing.Point(552, 18);
            this.txt_文件夹1路径.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.txt_文件夹1路径.Name = "txt_文件夹1路径";
            this.txt_文件夹1路径.Size = new System.Drawing.Size(622, 27);
            this.txt_文件夹1路径.TabIndex = 4;
            this.toolTip1.SetToolTip(this.txt_文件夹1路径, "修改后按 Enter 加载目录");
            this.txt_文件夹1路径.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_文件夹1路径_KeyPress);
            // 
            // label7
            // 
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.label7.Location = new System.Drawing.Point(12, 56);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(132, 48);
            this.label7.TabIndex = 5;
            this.label7.Text = "图像 2 输入名称";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txt_图片2输入名称
            // 
            this.txt_图片2输入名称.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_图片2输入名称.BackColor = System.Drawing.Color.White;
            this.txt_图片2输入名称.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_图片2输入名称.Location = new System.Drawing.Point(144, 66);
            this.txt_图片2输入名称.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.txt_图片2输入名称.Name = "txt_图片2输入名称";
            this.txt_图片2输入名称.Size = new System.Drawing.Size(162, 27);
            this.txt_图片2输入名称.TabIndex = 6;
            this.txt_图片2输入名称.Text = "InputImage2";
            this.toolTip1.SetToolTip(this.txt_图片2输入名称, "ToolBlock 的第二个图像输入名称");
            this.txt_图片2输入名称.TextChanged += new System.EventHandler(this.txt_图片2输入名称_TextChanged);
            // 
            // btn_选择图像2
            // 
            this.btn_选择图像2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btn_选择图像2.BackColor = System.Drawing.Color.White;
            this.btn_选择图像2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_选择图像2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(211, 218, 228);
            this.btn_选择图像2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_选择图像2.Location = new System.Drawing.Point(316, 62);
            this.btn_选择图像2.Margin = new System.Windows.Forms.Padding(0);
            this.btn_选择图像2.Name = "btn_选择图像2";
            this.btn_选择图像2.Size = new System.Drawing.Size(92, 32);
            this.btn_选择图像2.TabIndex = 7;
            this.btn_选择图像2.Text = "选择图像";
            this.btn_选择图像2.UseVisualStyleBackColor = false;
            this.btn_选择图像2.Click += new System.EventHandler(this.btnOpenImage2_Click);
            // 
            // btnOpenImageFile2
            // 
            this.btnOpenImageFile2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOpenImageFile2.BackColor = System.Drawing.Color.White;
            this.btnOpenImageFile2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenImageFile2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(211, 218, 228);
            this.btnOpenImageFile2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenImageFile2.Location = new System.Drawing.Point(428, 62);
            this.btnOpenImageFile2.Margin = new System.Windows.Forms.Padding(0);
            this.btnOpenImageFile2.Name = "btnOpenImageFile2";
            this.btnOpenImageFile2.Size = new System.Drawing.Size(102, 32);
            this.btnOpenImageFile2.TabIndex = 8;
            this.btnOpenImageFile2.Text = "选择文件夹";
            this.btnOpenImageFile2.UseVisualStyleBackColor = false;
            this.btnOpenImageFile2.Click += new System.EventHandler(this.btnOpenImageFile2_Click);
            // 
            // txt_文件夹2路径
            // 
            this.txt_文件夹2路径.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_文件夹2路径.BackColor = System.Drawing.Color.White;
            this.txt_文件夹2路径.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_文件夹2路径.Location = new System.Drawing.Point(552, 66);
            this.txt_文件夹2路径.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.txt_文件夹2路径.Name = "txt_文件夹2路径";
            this.txt_文件夹2路径.Size = new System.Drawing.Size(622, 27);
            this.txt_文件夹2路径.TabIndex = 9;
            this.toolTip1.SetToolTip(this.txt_文件夹2路径, "修改后按 Enter 加载目录");
            this.txt_文件夹2路径.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_文件夹2路径_KeyPress);
            // 
            // syncLayout
            // 
            this.syncLayout.ColumnCount = 1;
            this.syncLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.syncLayout.Controls.Add(this.syncModePanel, 0, 0);
            this.syncLayout.Controls.Add(this.dualImageOptionsPanel, 0, 1);
            this.syncLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.syncLayout.Location = new System.Drawing.Point(0, 174);
            this.syncLayout.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.syncLayout.Name = "syncLayout";
            this.syncLayout.RowCount = 2;
            this.syncLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.syncLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.syncLayout.Size = new System.Drawing.Size(1198, 102);
            this.syncLayout.TabIndex = 2;
            // 
            // syncModePanel
            // 
            this.syncModePanel.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.syncModePanel.Controls.Add(this.label8);
            this.syncModePanel.Controls.Add(this.rdB_文件名完全一致);
            this.syncModePanel.Controls.Add(this.rdB_创建时间完全一致);
            this.syncModePanel.Controls.Add(this.rdB_文件2秒可以比文件1晚1S);
            this.syncModePanel.Controls.Add(this.nUD_允许时间差秒数);
            this.syncModePanel.Controls.Add(this.label10);
            this.syncModePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.syncModePanel.Location = new System.Drawing.Point(0, 0);
            this.syncModePanel.Margin = new System.Windows.Forms.Padding(0);
            this.syncModePanel.Name = "syncModePanel";
            this.syncModePanel.Padding = new System.Windows.Forms.Padding(12, 10, 12, 8);
            this.syncModePanel.Size = new System.Drawing.Size(1198, 52);
            this.syncModePanel.TabIndex = 0;
            this.syncModePanel.WrapContents = false;
            // 
            // label8
            // 
            this.label8.AutoSize = false;
            this.label8.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.label8.Location = new System.Drawing.Point(12, 10);
            this.label8.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 30);
            this.label8.TabIndex = 0;
            this.label8.Text = "图像同步方式";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rdB_文件名完全一致
            // 
            this.rdB_文件名完全一致.AutoSize = true;
            this.rdB_文件名完全一致.Checked = true;
            this.rdB_文件名完全一致.Location = new System.Drawing.Point(120, 15);
            this.rdB_文件名完全一致.Margin = new System.Windows.Forms.Padding(0, 5, 20, 0);
            this.rdB_文件名完全一致.Name = "rdB_文件名完全一致";
            this.rdB_文件名完全一致.Size = new System.Drawing.Size(124, 21);
            this.rdB_文件名完全一致.TabIndex = 1;
            this.rdB_文件名完全一致.TabStop = true;
            this.rdB_文件名完全一致.Text = "文件名完全一致";
            this.rdB_文件名完全一致.UseVisualStyleBackColor = true;
            // 
            // rdB_创建时间完全一致
            // 
            this.rdB_创建时间完全一致.AutoSize = true;
            this.rdB_创建时间完全一致.Location = new System.Drawing.Point(264, 15);
            this.rdB_创建时间完全一致.Margin = new System.Windows.Forms.Padding(0, 5, 20, 0);
            this.rdB_创建时间完全一致.Name = "rdB_创建时间完全一致";
            this.rdB_创建时间完全一致.Size = new System.Drawing.Size(136, 21);
            this.rdB_创建时间完全一致.TabIndex = 2;
            this.rdB_创建时间完全一致.Text = "创建时间完全一致";
            this.rdB_创建时间完全一致.UseVisualStyleBackColor = true;
            // 
            // rdB_文件2秒可以比文件1晚1S
            // 
            this.rdB_文件2秒可以比文件1晚1S.AutoSize = true;
            this.rdB_文件2秒可以比文件1晚1S.Location = new System.Drawing.Point(420, 15);
            this.rdB_文件2秒可以比文件1晚1S.Margin = new System.Windows.Forms.Padding(0, 5, 8, 0);
            this.rdB_文件2秒可以比文件1晚1S.Name = "rdB_文件2秒可以比文件1晚1S";
            this.rdB_文件2秒可以比文件1晚1S.Size = new System.Drawing.Size(196, 21);
            this.rdB_文件2秒可以比文件1晚1S.TabIndex = 3;
            this.rdB_文件2秒可以比文件1晚1S.Text = "图像 2 允许晚于图像 1";
            this.toolTip1.SetToolTip(this.rdB_文件2秒可以比文件1晚1S, "适用于以采集时间匹配图像的场景");
            this.rdB_文件2秒可以比文件1晚1S.UseVisualStyleBackColor = true;
            // 
            // nUD_允许时间差秒数
            // 
            this.nUD_允许时间差秒数.Location = new System.Drawing.Point(624, 12);
            this.nUD_允许时间差秒数.Margin = new System.Windows.Forms.Padding(0, 2, 4, 0);
            this.nUD_允许时间差秒数.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            this.nUD_允许时间差秒数.Name = "nUD_允许时间差秒数";
            this.nUD_允许时间差秒数.Size = new System.Drawing.Size(56, 27);
            this.nUD_允许时间差秒数.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = false;
            this.label10.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.label10.Location = new System.Drawing.Point(684, 10);
            this.label10.Margin = new System.Windows.Forms.Padding(0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(28, 30);
            this.label10.TabIndex = 5;
            this.label10.Text = "秒";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dualImageOptionsPanel
            // 
            this.dualImageOptionsPanel.Controls.Add(this.chb_启用双图像);
            this.dualImageOptionsPanel.Controls.Add(this.label9);
            this.dualImageOptionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dualImageOptionsPanel.Location = new System.Drawing.Point(0, 52);
            this.dualImageOptionsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.dualImageOptionsPanel.Name = "dualImageOptionsPanel";
            this.dualImageOptionsPanel.Padding = new System.Windows.Forms.Padding(12, 12, 12, 8);
            this.dualImageOptionsPanel.Size = new System.Drawing.Size(1198, 50);
            this.dualImageOptionsPanel.TabIndex = 1;
            this.dualImageOptionsPanel.WrapContents = false;
            // 
            // chb_启用双图像
            // 
            this.chb_启用双图像.AutoSize = true;
            this.chb_启用双图像.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.chb_启用双图像.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.chb_启用双图像.Location = new System.Drawing.Point(12, 12);
            this.chb_启用双图像.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.chb_启用双图像.Name = "chb_启用双图像";
            this.chb_启用双图像.Size = new System.Drawing.Size(123, 21);
            this.chb_启用双图像.TabIndex = 0;
            this.chb_启用双图像.Text = "启用双图像输入";
            this.chb_启用双图像.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = false;
            this.label9.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.label9.Location = new System.Drawing.Point(155, 12);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(420, 24);
            this.label9.TabIndex = 1;
            this.label9.Text = "再次点击“双图像配置”返回 ToolBlock 编辑界面";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.label11.Location = new System.Drawing.Point(0, 286);
            this.label11.Margin = new System.Windows.Forms.Padding(0, 14, 0, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(1198, 238);
            this.label11.TabIndex = 3;
            this.label11.Text = "直接编辑目录路径后请按 Enter 重新加载图像列表。";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 4000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // frmToolBlock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1000, 680);
            this.Name = "frmToolBlock";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "视觉工具调试";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.headerLayout.ResumeLayout(false);
            this.titleLayout.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.contextLayout.ResumeLayout(false);
            this.navigationPanel.ResumeLayout(false);
            this.navigationPanel.PerformLayout();
            this.monitorPanel.ResumeLayout(false);
            this.monitorPanel.PerformLayout();
            this.pathLayout.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).EndInit();
            this.panel3.ResumeLayout(false);
            this.dualImageLayout.ResumeLayout(false);
            this.dualImageHeaderLayout.ResumeLayout(false);
            this.dualImageGrid.ResumeLayout(false);
            this.dualImageGrid.PerformLayout();
            this.syncLayout.ResumeLayout(false);
            this.syncModePanel.ResumeLayout(false);
            this.syncModePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_允许时间差秒数)).EndInit();
            this.dualImageOptionsPanel.ResumeLayout(false);
            this.dualImageOptionsPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
