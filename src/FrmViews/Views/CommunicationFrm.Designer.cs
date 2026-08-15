using System.ComponentModel;

namespace FrmViews.Views
{
    partial class CommunicationFrm
    {
        private IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.FlowLayoutPanel addButtonPanel;
        private System.Windows.Forms.Button addCameraButton;
        private System.Windows.Forms.Button addPlcButton;
        private System.Windows.Forms.Button addLightButton;
        private System.Windows.Forms.Button toolbarDeleteButton;
        private System.Windows.Forms.SplitContainer workspaceSplitContainer;
        private System.Windows.Forms.Panel topologyPanel;
        private System.Windows.Forms.Label topologyTitleLabel;
        private FrmViews.Controls.CommunicationTopologyControl topologyControl;
        private System.Windows.Forms.TableLayoutPanel detailsLayout;
        private System.Windows.Forms.Panel detailsHeaderPanel;
        private System.Windows.Forms.Label detailsTitleLabel;
        private System.Windows.Forms.Label detailsTypeLabel;
        private System.Windows.Forms.TableLayoutPanel detailsContentPanel;
        private System.Windows.Forms.TableLayoutPanel commonFieldsLayout;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.CheckBox enabledCheckBox;
        private FrmViews.Controls.ModernTabControl settingsTabControl;
        private System.Windows.Forms.TabPage cameraTabPage;
        private System.Windows.Forms.TabPage plcTabPage;
        private System.Windows.Forms.TabPage lightTabPage;
        private System.Windows.Forms.TableLayoutPanel cameraFieldsLayout;
        private System.Windows.Forms.Label cameraProtocolLabel;
        private System.Windows.Forms.Label cameraProtocolValueLabel;
        private System.Windows.Forms.Label cameraHostLabel;
        private System.Windows.Forms.TextBox cameraHostTextBox;
        private System.Windows.Forms.Label cameraTimeoutLabel;
        private System.Windows.Forms.NumericUpDown cameraTimeoutNumeric;
        private System.Windows.Forms.TableLayoutPanel plcFieldsLayout;
        private System.Windows.Forms.Label plcProtocolLabel;
        private System.Windows.Forms.ComboBox plcProtocolComboBox;
        private System.Windows.Forms.Label plcConnectionModeLabel;
        private System.Windows.Forms.ComboBox plcConnectionModeComboBox;
        private System.Windows.Forms.TableLayoutPanel plcEndpointLayout;
        private System.Windows.Forms.TableLayoutPanel plcNetworkPanel;
        private System.Windows.Forms.Label plcNetworkTitleLabel;
        private System.Windows.Forms.Label plcHostLabel;
        private System.Windows.Forms.TextBox plcHostTextBox;
        private System.Windows.Forms.Label plcPortLabel;
        private System.Windows.Forms.NumericUpDown plcPortNumeric;
        private System.Windows.Forms.TableLayoutPanel plcSerialPanel;
        private System.Windows.Forms.Label plcSerialTitleLabel;
        private System.Windows.Forms.Label plcSerialPortLabel;
        private System.Windows.Forms.ComboBox plcSerialPortComboBox;
        private System.Windows.Forms.Label plcBaudLabel;
        private System.Windows.Forms.ComboBox plcBaudComboBox;
        private System.Windows.Forms.Label plcStationLabel;
        private System.Windows.Forms.NumericUpDown plcStationNumeric;
        private System.Windows.Forms.TableLayoutPanel lightFieldsLayout;
        private System.Windows.Forms.Label lightConnectionModeLabel;
        private System.Windows.Forms.ComboBox lightConnectionModeComboBox;
        private System.Windows.Forms.TableLayoutPanel lightEndpointLayout;
        private System.Windows.Forms.TableLayoutPanel lightNetworkPanel;
        private System.Windows.Forms.Label lightNetworkTitleLabel;
        private System.Windows.Forms.Label lightHostLabel;
        private System.Windows.Forms.TextBox lightHostTextBox;
        private System.Windows.Forms.Label lightPortLabel;
        private System.Windows.Forms.NumericUpDown lightPortNumeric;
        private System.Windows.Forms.TableLayoutPanel lightSerialPanel;
        private System.Windows.Forms.Label lightSerialTitleLabel;
        private System.Windows.Forms.Label lightSerialPortLabel;
        private System.Windows.Forms.ComboBox lightSerialPortComboBox;
        private System.Windows.Forms.Label lightBaudLabel;
        private System.Windows.Forms.ComboBox lightBaudComboBox;
        private System.Windows.Forms.Label lightDataBitsLabel;
        private System.Windows.Forms.ComboBox lightDataBitsComboBox;
        private System.Windows.Forms.Label lightParityLabel;
        private System.Windows.Forms.ComboBox lightParityComboBox;
        private System.Windows.Forms.Label lightStopBitsLabel;
        private System.Windows.Forms.ComboBox lightStopBitsComboBox;
        private System.Windows.Forms.TableLayoutPanel footerLayout;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label emptyStateLabel;
        private System.Windows.Forms.BindingSource deviceBindingSource;
        private System.Windows.Forms.ErrorProvider errorProvider;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.addButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.addCameraButton = new System.Windows.Forms.Button();
            this.addPlcButton = new System.Windows.Forms.Button();
            this.addLightButton = new System.Windows.Forms.Button();
            this.toolbarDeleteButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.workspaceSplitContainer = new System.Windows.Forms.SplitContainer();
            this.topologyPanel = new System.Windows.Forms.Panel();
            this.topologyControl = new FrmViews.Controls.CommunicationTopologyControl();
            this.topologyTitleLabel = new System.Windows.Forms.Label();
            this.detailsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.detailsHeaderPanel = new System.Windows.Forms.Panel();
            this.detailsTitleLabel = new System.Windows.Forms.Label();
            this.detailsTypeLabel = new System.Windows.Forms.Label();
            this.detailsContentPanel = new System.Windows.Forms.TableLayoutPanel();
            this.commonFieldsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.nameLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.enabledCheckBox = new System.Windows.Forms.CheckBox();
            this.settingsTabControl = new FrmViews.Controls.ModernTabControl();
            this.plcTabPage = new System.Windows.Forms.TabPage();
            this.plcFieldsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.plcProtocolLabel = new System.Windows.Forms.Label();
            this.plcProtocolComboBox = new System.Windows.Forms.ComboBox();
            this.plcConnectionModeLabel = new System.Windows.Forms.Label();
            this.plcConnectionModeComboBox = new System.Windows.Forms.ComboBox();
            this.plcEndpointLayout = new System.Windows.Forms.TableLayoutPanel();
            this.plcNetworkPanel = new System.Windows.Forms.TableLayoutPanel();
            this.plcNetworkTitleLabel = new System.Windows.Forms.Label();
            this.plcHostLabel = new System.Windows.Forms.Label();
            this.plcHostTextBox = new System.Windows.Forms.TextBox();
            this.plcPortLabel = new System.Windows.Forms.Label();
            this.plcPortNumeric = new System.Windows.Forms.NumericUpDown();
            this.plcSerialPanel = new System.Windows.Forms.TableLayoutPanel();
            this.plcSerialTitleLabel = new System.Windows.Forms.Label();
            this.plcSerialPortLabel = new System.Windows.Forms.Label();
            this.plcSerialPortComboBox = new System.Windows.Forms.ComboBox();
            this.plcBaudLabel = new System.Windows.Forms.Label();
            this.plcBaudComboBox = new System.Windows.Forms.ComboBox();
            this.plcStationLabel = new System.Windows.Forms.Label();
            this.plcStationNumeric = new System.Windows.Forms.NumericUpDown();
            this.cameraTabPage = new System.Windows.Forms.TabPage();
            this.cameraFieldsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.cameraProtocolLabel = new System.Windows.Forms.Label();
            this.cameraProtocolValueLabel = new System.Windows.Forms.Label();
            this.cameraHostLabel = new System.Windows.Forms.Label();
            this.cameraHostTextBox = new System.Windows.Forms.TextBox();
            this.cameraTimeoutLabel = new System.Windows.Forms.Label();
            this.cameraTimeoutNumeric = new System.Windows.Forms.NumericUpDown();
            this.lightTabPage = new System.Windows.Forms.TabPage();
            this.lightFieldsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lightConnectionModeLabel = new System.Windows.Forms.Label();
            this.lightConnectionModeComboBox = new System.Windows.Forms.ComboBox();
            this.lightEndpointLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lightNetworkPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lightNetworkTitleLabel = new System.Windows.Forms.Label();
            this.lightHostLabel = new System.Windows.Forms.Label();
            this.lightHostTextBox = new System.Windows.Forms.TextBox();
            this.lightPortLabel = new System.Windows.Forms.Label();
            this.lightPortNumeric = new System.Windows.Forms.NumericUpDown();
            this.lightSerialPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lightSerialTitleLabel = new System.Windows.Forms.Label();
            this.lightSerialPortLabel = new System.Windows.Forms.Label();
            this.lightSerialPortComboBox = new System.Windows.Forms.ComboBox();
            this.lightBaudLabel = new System.Windows.Forms.Label();
            this.lightBaudComboBox = new System.Windows.Forms.ComboBox();
            this.lightDataBitsLabel = new System.Windows.Forms.Label();
            this.lightDataBitsComboBox = new System.Windows.Forms.ComboBox();
            this.lightParityLabel = new System.Windows.Forms.Label();
            this.lightParityComboBox = new System.Windows.Forms.ComboBox();
            this.lightStopBitsLabel = new System.Windows.Forms.Label();
            this.lightStopBitsComboBox = new System.Windows.Forms.ComboBox();
            this.emptyStateLabel = new System.Windows.Forms.Label();
            this.footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.closeButton = new System.Windows.Forms.Button();
            this.deviceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.rootLayout.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.addButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.workspaceSplitContainer)).BeginInit();
            this.workspaceSplitContainer.Panel1.SuspendLayout();
            this.workspaceSplitContainer.Panel2.SuspendLayout();
            this.workspaceSplitContainer.SuspendLayout();
            this.topologyPanel.SuspendLayout();
            this.detailsLayout.SuspendLayout();
            this.detailsHeaderPanel.SuspendLayout();
            this.detailsContentPanel.SuspendLayout();
            this.commonFieldsLayout.SuspendLayout();
            this.settingsTabControl.SuspendLayout();
            this.plcTabPage.SuspendLayout();
            this.plcFieldsLayout.SuspendLayout();
            this.plcEndpointLayout.SuspendLayout();
            this.plcNetworkPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plcPortNumeric)).BeginInit();
            this.plcSerialPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plcStationNumeric)).BeginInit();
            this.cameraTabPage.SuspendLayout();
            this.cameraFieldsLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cameraTimeoutNumeric)).BeginInit();
            this.lightTabPage.SuspendLayout();
            this.lightFieldsLayout.SuspendLayout();
            this.lightEndpointLayout.SuspendLayout();
            this.lightNetworkPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lightPortNumeric)).BeginInit();
            this.lightSerialPanel.SuspendLayout();
            this.footerLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.headerPanel, 0, 0);
            this.rootLayout.Controls.Add(this.workspaceSplitContainer, 0, 1);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 2;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 109F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(1180, 929);
            this.rootLayout.TabIndex = 0;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.Controls.Add(this.titleLabel);
            this.headerPanel.Controls.Add(this.subtitleLabel);
            this.headerPanel.Controls.Add(this.addButtonPanel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerPanel.Location = new System.Drawing.Point(3, 3);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(24, 14, 24, 12);
            this.headerPanel.Size = new System.Drawing.Size(1174, 103);
            this.headerPanel.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 13F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(32)))), ((int)(((byte)(45)))));
            this.titleLabel.Location = new System.Drawing.Point(24, 14);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(145, 30);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "通讯连接配置";
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.AutoSize = true;
            this.subtitleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.5F);
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(112)))), ((int)(((byte)(128)))));
            this.subtitleLabel.Location = new System.Drawing.Point(26, 48);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(190, 20);
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "管理相机、PLC 与光源连接";
            // 
            // addButtonPanel
            // 
            this.addButtonPanel.Controls.Add(this.addCameraButton);
            this.addButtonPanel.Controls.Add(this.addPlcButton);
            this.addButtonPanel.Controls.Add(this.addLightButton);
            this.addButtonPanel.Controls.Add(this.toolbarDeleteButton);
            this.addButtonPanel.Controls.Add(this.saveButton);
            this.addButtonPanel.Controls.Add(this.statusLabel);
            this.addButtonPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.addButtonPanel.Location = new System.Drawing.Point(340, 14);
            this.addButtonPanel.Name = "addButtonPanel";
            this.addButtonPanel.Padding = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.addButtonPanel.Size = new System.Drawing.Size(810, 77);
            this.addButtonPanel.TabIndex = 2;
            // 
            // addCameraButton
            // 
            this.addCameraButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.addCameraButton.FlatAppearance.BorderSize = 0;
            this.addCameraButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addCameraButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.addCameraButton.ForeColor = System.Drawing.Color.White;
            this.addCameraButton.Location = new System.Drawing.Point(8, 20);
            this.addCameraButton.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.addCameraButton.Name = "addCameraButton";
            this.addCameraButton.Size = new System.Drawing.Size(132, 38);
            this.addCameraButton.TabIndex = 0;
            this.addCameraButton.Text = "+ 添加相机";
            this.addCameraButton.UseVisualStyleBackColor = false;
            this.addCameraButton.Click += new System.EventHandler(this.AddCameraButtonOnClick);
            // 
            // addPlcButton
            // 
            this.addPlcButton.BackColor = System.Drawing.Color.White;
            this.addPlcButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(225)))), ((int)(((byte)(232)))));
            this.addPlcButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addPlcButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.addPlcButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(32)))), ((int)(((byte)(45)))));
            this.addPlcButton.Location = new System.Drawing.Point(148, 20);
            this.addPlcButton.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.addPlcButton.Name = "addPlcButton";
            this.addPlcButton.Size = new System.Drawing.Size(132, 38);
            this.addPlcButton.TabIndex = 1;
            this.addPlcButton.Text = "+ 添加 PLC";
            this.addPlcButton.UseVisualStyleBackColor = false;
            this.addPlcButton.Click += new System.EventHandler(this.AddPlcButtonOnClick);
            // 
            // addLightButton
            // 
            this.addLightButton.BackColor = System.Drawing.Color.White;
            this.addLightButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(225)))), ((int)(((byte)(232)))));
            this.addLightButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addLightButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.addLightButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(32)))), ((int)(((byte)(45)))));
            this.addLightButton.Location = new System.Drawing.Point(288, 20);
            this.addLightButton.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.addLightButton.Name = "addLightButton";
            this.addLightButton.Size = new System.Drawing.Size(132, 38);
            this.addLightButton.TabIndex = 2;
            this.addLightButton.Text = "+ 添加光源";
            this.addLightButton.UseVisualStyleBackColor = false;
            this.addLightButton.Click += new System.EventHandler(this.AddLightButtonOnClick);
            // 
            // toolbarDeleteButton
            // 
            this.toolbarDeleteButton.BackColor = System.Drawing.Color.White;
            this.toolbarDeleteButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(225)))), ((int)(((byte)(232)))));
            this.toolbarDeleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolbarDeleteButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.toolbarDeleteButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.toolbarDeleteButton.Location = new System.Drawing.Point(428, 20);
            this.toolbarDeleteButton.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.toolbarDeleteButton.Name = "toolbarDeleteButton";
            this.toolbarDeleteButton.Size = new System.Drawing.Size(118, 38);
            this.toolbarDeleteButton.TabIndex = 3;
            this.toolbarDeleteButton.Text = "删除设备";
            this.toolbarDeleteButton.UseVisualStyleBackColor = false;
            this.toolbarDeleteButton.Click += new System.EventHandler(this.DeleteButtonOnClick);
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.White;
            this.saveButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(225)))), ((int)(((byte)(232)))));
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.saveButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(32)))), ((int)(((byte)(45)))));
            this.saveButton.Location = new System.Drawing.Point(554, 20);
            this.saveButton.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(132, 38);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "保存配置";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.SaveButtonOnClick);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoEllipsis = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F);
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(112)))), ((int)(((byte)(128)))));
            this.statusLabel.Location = new System.Drawing.Point(698, 20);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(98, 38);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "待保存";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // workspaceSplitContainer
            // 
            this.workspaceSplitContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(213)))), ((int)(((byte)(221)))));
            this.workspaceSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workspaceSplitContainer.Location = new System.Drawing.Point(18, 127);
            this.workspaceSplitContainer.Margin = new System.Windows.Forms.Padding(18);
            this.workspaceSplitContainer.Name = "workspaceSplitContainer";
            this.workspaceSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // workspaceSplitContainer.Panel1
            // 
            this.workspaceSplitContainer.Panel1.BackColor = System.Drawing.Color.White;
            this.workspaceSplitContainer.Panel1.Controls.Add(this.topologyPanel);
            this.workspaceSplitContainer.Panel1MinSize = 130;
            // 
            // workspaceSplitContainer.Panel2
            // 
            this.workspaceSplitContainer.Panel2.BackColor = System.Drawing.Color.White;
            this.workspaceSplitContainer.Panel2.Controls.Add(this.detailsLayout);
            this.workspaceSplitContainer.Panel2MinSize = 180;
            this.workspaceSplitContainer.Size = new System.Drawing.Size(1144, 784);
            this.workspaceSplitContainer.SplitterDistance = 296;
            this.workspaceSplitContainer.SplitterWidth = 5;
            this.workspaceSplitContainer.TabIndex = 1;
            // 
            // topologyPanel
            // 
            this.topologyPanel.Controls.Add(this.topologyControl);
            this.topologyPanel.Controls.Add(this.topologyTitleLabel);
            this.topologyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topologyPanel.Location = new System.Drawing.Point(0, 0);
            this.topologyPanel.Name = "topologyPanel";
            this.topologyPanel.Padding = new System.Windows.Forms.Padding(16, 0, 16, 12);
            this.topologyPanel.Size = new System.Drawing.Size(1144, 296);
            this.topologyPanel.TabIndex = 0;
            // 
            // topologyControl
            // 
            this.topologyControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(249)))), ((int)(((byte)(251)))));
            this.topologyControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.topologyControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topologyControl.Location = new System.Drawing.Point(16, 44);
            this.topologyControl.MinimumSize = new System.Drawing.Size(540, 100);
            this.topologyControl.Name = "topologyControl";
            this.topologyControl.Size = new System.Drawing.Size(1112, 240);
            this.topologyControl.TabIndex = 0;
            this.topologyControl.DeviceSelected += new System.EventHandler<FrmViews.Controls.CommunicationDeviceSelectedEventArgs>(this.TopologyControlOnDeviceSelected);
            // 
            // topologyTitleLabel
            // 
            this.topologyTitleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topologyTitleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.topologyTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(32)))), ((int)(((byte)(45)))));
            this.topologyTitleLabel.Location = new System.Drawing.Point(16, 0);
            this.topologyTitleLabel.Name = "topologyTitleLabel";
            this.topologyTitleLabel.Size = new System.Drawing.Size(1112, 44);
            this.topologyTitleLabel.TabIndex = 1;
            this.topologyTitleLabel.Text = "设备与网络组态";
            this.topologyTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // detailsLayout
            // 
            this.detailsLayout.ColumnCount = 1;
            this.detailsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailsLayout.Controls.Add(this.detailsHeaderPanel, 0, 0);
            this.detailsLayout.Controls.Add(this.detailsContentPanel, 0, 1);
            this.detailsLayout.Controls.Add(this.emptyStateLabel, 0, 1);
            this.detailsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsLayout.Location = new System.Drawing.Point(0, 0);
            this.detailsLayout.Name = "detailsLayout";
            this.detailsLayout.Padding = new System.Windows.Forms.Padding(20, 10, 20, 12);
            this.detailsLayout.RowCount = 2;
            this.detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.detailsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailsLayout.Size = new System.Drawing.Size(1144, 483);
            this.detailsLayout.TabIndex = 0;
            // 
            // detailsHeaderPanel
            // 
            this.detailsHeaderPanel.Controls.Add(this.detailsTitleLabel);
            this.detailsHeaderPanel.Controls.Add(this.detailsTypeLabel);
            this.detailsHeaderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsHeaderPanel.Location = new System.Drawing.Point(23, 13);
            this.detailsHeaderPanel.Name = "detailsHeaderPanel";
            this.detailsHeaderPanel.Size = new System.Drawing.Size(1098, 42);
            this.detailsHeaderPanel.TabIndex = 0;
            // 
            // detailsTitleLabel
            // 
            this.detailsTitleLabel.AutoSize = true;
            this.detailsTitleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.detailsTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(32)))), ((int)(((byte)(45)))));
            this.detailsTitleLabel.Location = new System.Drawing.Point(0, 12);
            this.detailsTitleLabel.Name = "detailsTitleLabel";
            this.detailsTitleLabel.Size = new System.Drawing.Size(78, 24);
            this.detailsTitleLabel.TabIndex = 0;
            this.detailsTitleLabel.Text = "连接详情";
            // 
            // detailsTypeLabel
            // 
            this.detailsTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsTypeLabel.AutoSize = true;
            this.detailsTypeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(255)))));
            this.detailsTypeLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.5F);
            this.detailsTypeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.detailsTypeLabel.Location = new System.Drawing.Point(1488, 10);
            this.detailsTypeLabel.Name = "detailsTypeLabel";
            this.detailsTypeLabel.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.detailsTypeLabel.Size = new System.Drawing.Size(100, 28);
            this.detailsTypeLabel.TabIndex = 1;
            this.detailsTypeLabel.Text = "未选择设备";
            // 
            // detailsContentPanel
            // 
            this.detailsContentPanel.ColumnCount = 1;
            this.detailsContentPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailsContentPanel.Controls.Add(this.commonFieldsLayout, 0, 0);
            this.detailsContentPanel.Controls.Add(this.settingsTabControl, 0, 1);
            this.detailsContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsContentPanel.Location = new System.Drawing.Point(23, 418);
            this.detailsContentPanel.Name = "detailsContentPanel";
            this.detailsContentPanel.RowCount = 2;
            this.detailsContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.detailsContentPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailsContentPanel.Size = new System.Drawing.Size(1098, 50);
            this.detailsContentPanel.TabIndex = 1;
            // 
            // commonFieldsLayout
            // 
            this.commonFieldsLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.commonFieldsLayout.ColumnCount = 4;
            this.commonFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.commonFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.commonFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.commonFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.commonFieldsLayout.Controls.Add(this.nameLabel, 0, 0);
            this.commonFieldsLayout.Controls.Add(this.nameTextBox, 1, 0);
            this.commonFieldsLayout.Controls.Add(this.enabledCheckBox, 3, 0);
            this.commonFieldsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commonFieldsLayout.Location = new System.Drawing.Point(3, 3);
            this.commonFieldsLayout.Name = "commonFieldsLayout";
            this.commonFieldsLayout.Padding = new System.Windows.Forms.Padding(14, 13, 14, 10);
            this.commonFieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.commonFieldsLayout.Size = new System.Drawing.Size(1092, 64);
            this.commonFieldsLayout.TabIndex = 0;
            // 
            // nameLabel
            // 
            this.nameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.nameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(112)))), ((int)(((byte)(128)))));
            this.nameLabel.Location = new System.Drawing.Point(17, 13);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(86, 41);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "设备名称";
            this.nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nameTextBox
            // 
            this.nameTextBox.BackColor = System.Drawing.Color.White;
            this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this.nameTextBox.Location = new System.Drawing.Point(106, 18);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(844, 28);
            this.nameTextBox.TabIndex = 1;
            // 
            // enabledCheckBox
            // 
            this.enabledCheckBox.AutoSize = true;
            this.enabledCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.enabledCheckBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.enabledCheckBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(32)))), ((int)(((byte)(45)))));
            this.enabledCheckBox.Location = new System.Drawing.Point(977, 16);
            this.enabledCheckBox.Name = "enabledCheckBox";
            this.enabledCheckBox.Size = new System.Drawing.Size(98, 35);
            this.enabledCheckBox.TabIndex = 2;
            this.enabledCheckBox.Text = "启用连接";
            // 
            // settingsTabControl
            // 
            this.settingsTabControl.Controls.Add(this.plcTabPage);
            this.settingsTabControl.Controls.Add(this.cameraTabPage);
            this.settingsTabControl.Controls.Add(this.lightTabPage);
            this.settingsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.settingsTabControl.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.settingsTabControl.ItemSize = new System.Drawing.Size(116, 42);
            this.settingsTabControl.Location = new System.Drawing.Point(0, 82);
            this.settingsTabControl.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.settingsTabControl.Name = "settingsTabControl";
            this.settingsTabControl.Padding = new System.Drawing.Point(16, 4);
            this.settingsTabControl.SelectedIndex = 0;
            this.settingsTabControl.Size = new System.Drawing.Size(1098, 1);
            this.settingsTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.settingsTabControl.TabIndex = 1;
            this.settingsTabControl.SelectedIndexChanged += new System.EventHandler(this.SettingsTabControlOnSelectedIndexChanged);
            // 
            // plcTabPage
            // 
            this.plcTabPage.AutoScroll = true;
            this.plcTabPage.BackColor = System.Drawing.Color.White;
            this.plcTabPage.Controls.Add(this.plcFieldsLayout);
            this.plcTabPage.Location = new System.Drawing.Point(4, 46);
            this.plcTabPage.Name = "plcTabPage";
            this.plcTabPage.Size = new System.Drawing.Size(1090, 0);
            this.plcTabPage.TabIndex = 0;
            this.plcTabPage.Text = "PLC";
            // 
            // plcFieldsLayout
            // 
            this.plcFieldsLayout.ColumnCount = 2;
            this.plcFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.plcFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.plcFieldsLayout.Controls.Add(this.plcProtocolLabel, 0, 0);
            this.plcFieldsLayout.Controls.Add(this.plcProtocolComboBox, 1, 0);
            this.plcFieldsLayout.Controls.Add(this.plcConnectionModeLabel, 0, 1);
            this.plcFieldsLayout.Controls.Add(this.plcConnectionModeComboBox, 1, 1);
            this.plcFieldsLayout.Controls.Add(this.plcEndpointLayout, 0, 2);
            this.plcFieldsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plcFieldsLayout.Location = new System.Drawing.Point(0, 0);
            this.plcFieldsLayout.Name = "plcFieldsLayout";
            this.plcFieldsLayout.Padding = new System.Windows.Forms.Padding(18, 14, 18, 10);
            this.plcFieldsLayout.RowCount = 3;
            this.plcFieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.plcFieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.plcFieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.plcFieldsLayout.Size = new System.Drawing.Size(1090, 0);
            this.plcFieldsLayout.TabIndex = 0;
            // 
            // plcProtocolLabel
            // 
            this.plcProtocolLabel.Location = new System.Drawing.Point(21, 14);
            this.plcProtocolLabel.Name = "plcProtocolLabel";
            this.plcProtocolLabel.Size = new System.Drawing.Size(100, 23);
            this.plcProtocolLabel.TabIndex = 0;
            this.plcProtocolLabel.Text = "通讯协议";
            // 
            // plcProtocolComboBox
            // 
            this.plcProtocolComboBox.Location = new System.Drawing.Point(171, 17);
            this.plcProtocolComboBox.Name = "plcProtocolComboBox";
            this.plcProtocolComboBox.Size = new System.Drawing.Size(121, 28);
            this.plcProtocolComboBox.TabIndex = 1;
            this.plcProtocolComboBox.SelectedIndexChanged += new System.EventHandler(this.PlcProtocolComboBoxOnSelectedIndexChanged);
            // 
            // plcConnectionModeLabel
            // 
            this.plcConnectionModeLabel.Location = new System.Drawing.Point(21, 64);
            this.plcConnectionModeLabel.Name = "plcConnectionModeLabel";
            this.plcConnectionModeLabel.Size = new System.Drawing.Size(100, 23);
            this.plcConnectionModeLabel.TabIndex = 2;
            this.plcConnectionModeLabel.Text = "通讯方式";
            // 
            // plcConnectionModeComboBox
            // 
            this.plcConnectionModeComboBox.Items.AddRange(new object[] {
            "网口",
            "串口"});
            this.plcConnectionModeComboBox.Location = new System.Drawing.Point(171, 67);
            this.plcConnectionModeComboBox.Name = "plcConnectionModeComboBox";
            this.plcConnectionModeComboBox.Size = new System.Drawing.Size(121, 28);
            this.plcConnectionModeComboBox.TabIndex = 3;
            this.plcConnectionModeComboBox.SelectedIndexChanged += new System.EventHandler(this.PlcConnectionModeOnSelectedIndexChanged);
            // 
            // plcEndpointLayout
            // 
            this.plcEndpointLayout.ColumnCount = 2;
            this.plcFieldsLayout.SetColumnSpan(this.plcEndpointLayout, 2);
            this.plcEndpointLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.plcEndpointLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.plcEndpointLayout.Controls.Add(this.plcNetworkPanel, 0, 0);
            this.plcEndpointLayout.Controls.Add(this.plcSerialPanel, 1, 0);
            this.plcEndpointLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plcEndpointLayout.Location = new System.Drawing.Point(18, 122);
            this.plcEndpointLayout.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.plcEndpointLayout.Name = "plcEndpointLayout";
            this.plcEndpointLayout.Size = new System.Drawing.Size(1054, 1);
            this.plcEndpointLayout.TabIndex = 4;
            // 
            // plcNetworkPanel
            // 
            this.plcNetworkPanel.Controls.Add(this.plcNetworkTitleLabel, 0, 0);
            this.plcNetworkPanel.Controls.Add(this.plcHostLabel, 0, 1);
            this.plcNetworkPanel.Controls.Add(this.plcHostTextBox, 1, 1);
            this.plcNetworkPanel.Controls.Add(this.plcPortLabel, 0, 2);
            this.plcNetworkPanel.Controls.Add(this.plcPortNumeric, 1, 2);
            this.plcNetworkPanel.Location = new System.Drawing.Point(3, 3);
            this.plcNetworkPanel.Name = "plcNetworkPanel";
            this.plcNetworkPanel.Size = new System.Drawing.Size(200, 100);
            this.plcNetworkPanel.TabIndex = 0;
            // 
            // plcNetworkTitleLabel
            // 
            this.plcNetworkTitleLabel.Location = new System.Drawing.Point(3, 0);
            this.plcNetworkTitleLabel.Name = "plcNetworkTitleLabel";
            this.plcNetworkTitleLabel.Size = new System.Drawing.Size(100, 23);
            this.plcNetworkTitleLabel.TabIndex = 0;
            this.plcNetworkTitleLabel.Text = "网口参数";
            // 
            // plcHostLabel
            // 
            this.plcHostLabel.Location = new System.Drawing.Point(3, 23);
            this.plcHostLabel.Name = "plcHostLabel";
            this.plcHostLabel.Size = new System.Drawing.Size(100, 23);
            this.plcHostLabel.TabIndex = 1;
            this.plcHostLabel.Text = "主机";
            // 
            // plcHostTextBox
            // 
            this.plcHostTextBox.Location = new System.Drawing.Point(109, 26);
            this.plcHostTextBox.Name = "plcHostTextBox";
            this.plcHostTextBox.Size = new System.Drawing.Size(100, 27);
            this.plcHostTextBox.TabIndex = 2;
            // 
            // plcPortLabel
            // 
            this.plcPortLabel.Location = new System.Drawing.Point(3, 56);
            this.plcPortLabel.Name = "plcPortLabel";
            this.plcPortLabel.Size = new System.Drawing.Size(100, 23);
            this.plcPortLabel.TabIndex = 3;
            this.plcPortLabel.Text = "端口";
            // 
            // plcPortNumeric
            // 
            this.plcPortNumeric.Location = new System.Drawing.Point(109, 59);
            this.plcPortNumeric.Name = "plcPortNumeric";
            this.plcPortNumeric.Size = new System.Drawing.Size(120, 27);
            this.plcPortNumeric.TabIndex = 4;
            // 
            // plcSerialPanel
            // 
            this.plcSerialPanel.Controls.Add(this.plcSerialTitleLabel, 0, 0);
            this.plcSerialPanel.Controls.Add(this.plcSerialPortLabel, 0, 1);
            this.plcSerialPanel.Controls.Add(this.plcSerialPortComboBox, 1, 1);
            this.plcSerialPanel.Controls.Add(this.plcBaudLabel, 0, 2);
            this.plcSerialPanel.Controls.Add(this.plcBaudComboBox, 1, 2);
            this.plcSerialPanel.Controls.Add(this.plcStationLabel, 0, 3);
            this.plcSerialPanel.Controls.Add(this.plcStationNumeric, 1, 3);
            this.plcSerialPanel.Location = new System.Drawing.Point(535, 0);
            this.plcSerialPanel.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.plcSerialPanel.Name = "plcSerialPanel";
            this.plcSerialPanel.Size = new System.Drawing.Size(200, 100);
            this.plcSerialPanel.TabIndex = 1;
            // 
            // plcSerialTitleLabel
            // 
            this.plcSerialTitleLabel.Location = new System.Drawing.Point(3, 0);
            this.plcSerialTitleLabel.Name = "plcSerialTitleLabel";
            this.plcSerialTitleLabel.Size = new System.Drawing.Size(100, 23);
            this.plcSerialTitleLabel.TabIndex = 0;
            this.plcSerialTitleLabel.Text = "串口参数";
            // 
            // plcSerialPortLabel
            // 
            this.plcSerialPortLabel.Location = new System.Drawing.Point(3, 23);
            this.plcSerialPortLabel.Name = "plcSerialPortLabel";
            this.plcSerialPortLabel.Size = new System.Drawing.Size(100, 23);
            this.plcSerialPortLabel.TabIndex = 1;
            this.plcSerialPortLabel.Text = "串口";
            // 
            // plcSerialPortComboBox
            // 
            this.plcSerialPortComboBox.Location = new System.Drawing.Point(109, 26);
            this.plcSerialPortComboBox.Name = "plcSerialPortComboBox";
            this.plcSerialPortComboBox.Size = new System.Drawing.Size(121, 28);
            this.plcSerialPortComboBox.TabIndex = 2;
            // 
            // plcBaudLabel
            // 
            this.plcBaudLabel.Location = new System.Drawing.Point(3, 57);
            this.plcBaudLabel.Name = "plcBaudLabel";
            this.plcBaudLabel.Size = new System.Drawing.Size(100, 23);
            this.plcBaudLabel.TabIndex = 3;
            this.plcBaudLabel.Text = "波特率";
            // 
            // plcBaudComboBox
            // 
            this.plcBaudComboBox.Items.AddRange(new object[] {
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.plcBaudComboBox.Location = new System.Drawing.Point(109, 60);
            this.plcBaudComboBox.Name = "plcBaudComboBox";
            this.plcBaudComboBox.Size = new System.Drawing.Size(121, 28);
            this.plcBaudComboBox.TabIndex = 4;
            // 
            // plcStationLabel
            // 
            this.plcStationLabel.Location = new System.Drawing.Point(3, 91);
            this.plcStationLabel.Name = "plcStationLabel";
            this.plcStationLabel.Size = new System.Drawing.Size(100, 23);
            this.plcStationLabel.TabIndex = 5;
            this.plcStationLabel.Text = "站号";
            // 
            // plcStationNumeric
            // 
            this.plcStationNumeric.Location = new System.Drawing.Point(109, 94);
            this.plcStationNumeric.Name = "plcStationNumeric";
            this.plcStationNumeric.Size = new System.Drawing.Size(120, 27);
            this.plcStationNumeric.TabIndex = 6;
            // 
            // cameraTabPage
            // 
            this.cameraTabPage.AutoScroll = true;
            this.cameraTabPage.BackColor = System.Drawing.Color.White;
            this.cameraTabPage.Controls.Add(this.cameraFieldsLayout);
            this.cameraTabPage.Location = new System.Drawing.Point(4, 46);
            this.cameraTabPage.Name = "cameraTabPage";
            this.cameraTabPage.Size = new System.Drawing.Size(1090, 0);
            this.cameraTabPage.TabIndex = 1;
            this.cameraTabPage.Text = "相机";
            // 
            // cameraFieldsLayout
            // 
            this.cameraFieldsLayout.AutoSize = true;
            this.cameraFieldsLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cameraFieldsLayout.ColumnCount = 2;
            this.cameraFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.cameraFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cameraFieldsLayout.Controls.Add(this.cameraProtocolLabel, 0, 0);
            this.cameraFieldsLayout.Controls.Add(this.cameraProtocolValueLabel, 1, 0);
            this.cameraFieldsLayout.Controls.Add(this.cameraHostLabel, 0, 1);
            this.cameraFieldsLayout.Controls.Add(this.cameraHostTextBox, 1, 1);
            this.cameraFieldsLayout.Controls.Add(this.cameraTimeoutLabel, 0, 2);
            this.cameraFieldsLayout.Controls.Add(this.cameraTimeoutNumeric, 1, 2);
            this.cameraFieldsLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.cameraFieldsLayout.Location = new System.Drawing.Point(0, 0);
            this.cameraFieldsLayout.Name = "cameraFieldsLayout";
            this.cameraFieldsLayout.Padding = new System.Windows.Forms.Padding(18, 18, 18, 12);
            this.cameraFieldsLayout.RowCount = 3;
            this.cameraFieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.cameraFieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.cameraFieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.cameraFieldsLayout.Size = new System.Drawing.Size(1090, 186);
            this.cameraFieldsLayout.TabIndex = 0;
            // 
            // cameraProtocolLabel
            // 
            this.cameraProtocolLabel.Location = new System.Drawing.Point(21, 18);
            this.cameraProtocolLabel.Name = "cameraProtocolLabel";
            this.cameraProtocolLabel.Size = new System.Drawing.Size(100, 23);
            this.cameraProtocolLabel.TabIndex = 0;
            this.cameraProtocolLabel.Text = "检测方式";
            // 
            // cameraProtocolValueLabel
            // 
            this.cameraProtocolValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cameraProtocolValueLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.cameraProtocolValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.cameraProtocolValueLabel.Location = new System.Drawing.Point(171, 18);
            this.cameraProtocolValueLabel.Name = "cameraProtocolValueLabel";
            this.cameraProtocolValueLabel.Size = new System.Drawing.Size(898, 52);
            this.cameraProtocolValueLabel.TabIndex = 1;
            this.cameraProtocolValueLabel.Text = "ICMP Ping";
            this.cameraProtocolValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cameraHostLabel
            // 
            this.cameraHostLabel.Location = new System.Drawing.Point(21, 70);
            this.cameraHostLabel.Name = "cameraHostLabel";
            this.cameraHostLabel.Size = new System.Drawing.Size(100, 23);
            this.cameraHostLabel.TabIndex = 2;
            this.cameraHostLabel.Text = "IP 地址 / 主机名";
            // 
            // cameraHostTextBox
            // 
            this.cameraHostTextBox.Location = new System.Drawing.Point(171, 73);
            this.cameraHostTextBox.Name = "cameraHostTextBox";
            this.cameraHostTextBox.Size = new System.Drawing.Size(100, 27);
            this.cameraHostTextBox.TabIndex = 3;
            // 
            // cameraTimeoutLabel
            // 
            this.cameraTimeoutLabel.Location = new System.Drawing.Point(21, 122);
            this.cameraTimeoutLabel.Name = "cameraTimeoutLabel";
            this.cameraTimeoutLabel.Size = new System.Drawing.Size(100, 23);
            this.cameraTimeoutLabel.TabIndex = 4;
            this.cameraTimeoutLabel.Text = "超时时间（ms）";
            // 
            // cameraTimeoutNumeric
            // 
            this.cameraTimeoutNumeric.Location = new System.Drawing.Point(171, 125);
            this.cameraTimeoutNumeric.Name = "cameraTimeoutNumeric";
            this.cameraTimeoutNumeric.Size = new System.Drawing.Size(120, 27);
            this.cameraTimeoutNumeric.TabIndex = 5;
            // 
            // lightTabPage
            // 
            this.lightTabPage.AutoScroll = true;
            this.lightTabPage.BackColor = System.Drawing.Color.White;
            this.lightTabPage.Controls.Add(this.lightFieldsLayout);
            this.lightTabPage.Location = new System.Drawing.Point(4, 46);
            this.lightTabPage.Name = "lightTabPage";
            this.lightTabPage.Size = new System.Drawing.Size(1090, 0);
            this.lightTabPage.TabIndex = 2;
            this.lightTabPage.Text = "光源";
            // 
            // lightFieldsLayout
            // 
            this.lightFieldsLayout.ColumnCount = 2;
            this.lightFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.lightFieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lightFieldsLayout.Controls.Add(this.lightConnectionModeLabel, 0, 0);
            this.lightFieldsLayout.Controls.Add(this.lightConnectionModeComboBox, 1, 0);
            this.lightFieldsLayout.Controls.Add(this.lightEndpointLayout, 0, 1);
            this.lightFieldsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lightFieldsLayout.Location = new System.Drawing.Point(0, 0);
            this.lightFieldsLayout.Name = "lightFieldsLayout";
            this.lightFieldsLayout.Padding = new System.Windows.Forms.Padding(18, 14, 18, 10);
            this.lightFieldsLayout.RowCount = 2;
            this.lightFieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.lightFieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.lightFieldsLayout.Size = new System.Drawing.Size(1090, 0);
            this.lightFieldsLayout.TabIndex = 0;
            // 
            // lightConnectionModeLabel
            // 
            this.lightConnectionModeLabel.Location = new System.Drawing.Point(21, 14);
            this.lightConnectionModeLabel.Name = "lightConnectionModeLabel";
            this.lightConnectionModeLabel.Size = new System.Drawing.Size(100, 23);
            this.lightConnectionModeLabel.TabIndex = 0;
            this.lightConnectionModeLabel.Text = "通讯方式";
            // 
            // lightConnectionModeComboBox
            // 
            this.lightConnectionModeComboBox.Items.AddRange(new object[] {
            "网口",
            "串口"});
            this.lightConnectionModeComboBox.Location = new System.Drawing.Point(171, 17);
            this.lightConnectionModeComboBox.Name = "lightConnectionModeComboBox";
            this.lightConnectionModeComboBox.Size = new System.Drawing.Size(121, 28);
            this.lightConnectionModeComboBox.TabIndex = 1;
            this.lightConnectionModeComboBox.SelectedIndexChanged += new System.EventHandler(this.LightConnectionModeOnSelectedIndexChanged);
            // 
            // lightEndpointLayout
            // 
            this.lightEndpointLayout.ColumnCount = 2;
            this.lightFieldsLayout.SetColumnSpan(this.lightEndpointLayout, 2);
            this.lightEndpointLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44F));
            this.lightEndpointLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 56F));
            this.lightEndpointLayout.Controls.Add(this.lightNetworkPanel, 0, 0);
            this.lightEndpointLayout.Controls.Add(this.lightSerialPanel, 1, 0);
            this.lightEndpointLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lightEndpointLayout.Location = new System.Drawing.Point(18, 72);
            this.lightEndpointLayout.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.lightEndpointLayout.Name = "lightEndpointLayout";
            this.lightEndpointLayout.Size = new System.Drawing.Size(1054, 1);
            this.lightEndpointLayout.TabIndex = 2;
            // 
            // lightNetworkPanel
            // 
            this.lightNetworkPanel.Controls.Add(this.lightNetworkTitleLabel, 0, 0);
            this.lightNetworkPanel.Controls.Add(this.lightHostLabel, 0, 1);
            this.lightNetworkPanel.Controls.Add(this.lightHostTextBox, 1, 1);
            this.lightNetworkPanel.Controls.Add(this.lightPortLabel, 0, 2);
            this.lightNetworkPanel.Controls.Add(this.lightPortNumeric, 1, 2);
            this.lightNetworkPanel.Location = new System.Drawing.Point(3, 3);
            this.lightNetworkPanel.Name = "lightNetworkPanel";
            this.lightNetworkPanel.Size = new System.Drawing.Size(200, 100);
            this.lightNetworkPanel.TabIndex = 0;
            // 
            // lightNetworkTitleLabel
            // 
            this.lightNetworkTitleLabel.Location = new System.Drawing.Point(3, 0);
            this.lightNetworkTitleLabel.Name = "lightNetworkTitleLabel";
            this.lightNetworkTitleLabel.Size = new System.Drawing.Size(100, 23);
            this.lightNetworkTitleLabel.TabIndex = 0;
            this.lightNetworkTitleLabel.Text = "网口参数";
            // 
            // lightHostLabel
            // 
            this.lightHostLabel.Location = new System.Drawing.Point(3, 23);
            this.lightHostLabel.Name = "lightHostLabel";
            this.lightHostLabel.Size = new System.Drawing.Size(100, 23);
            this.lightHostLabel.TabIndex = 1;
            this.lightHostLabel.Text = "主机";
            // 
            // lightHostTextBox
            // 
            this.lightHostTextBox.Location = new System.Drawing.Point(109, 26);
            this.lightHostTextBox.Name = "lightHostTextBox";
            this.lightHostTextBox.Size = new System.Drawing.Size(100, 27);
            this.lightHostTextBox.TabIndex = 2;
            // 
            // lightPortLabel
            // 
            this.lightPortLabel.Location = new System.Drawing.Point(3, 56);
            this.lightPortLabel.Name = "lightPortLabel";
            this.lightPortLabel.Size = new System.Drawing.Size(100, 23);
            this.lightPortLabel.TabIndex = 3;
            this.lightPortLabel.Text = "端口";
            // 
            // lightPortNumeric
            // 
            this.lightPortNumeric.Location = new System.Drawing.Point(109, 59);
            this.lightPortNumeric.Name = "lightPortNumeric";
            this.lightPortNumeric.Size = new System.Drawing.Size(120, 27);
            this.lightPortNumeric.TabIndex = 4;
            // 
            // lightSerialPanel
            // 
            this.lightSerialPanel.Controls.Add(this.lightSerialTitleLabel, 0, 0);
            this.lightSerialPanel.Controls.Add(this.lightSerialPortLabel, 0, 1);
            this.lightSerialPanel.Controls.Add(this.lightSerialPortComboBox, 1, 1);
            this.lightSerialPanel.Controls.Add(this.lightBaudLabel, 0, 2);
            this.lightSerialPanel.Controls.Add(this.lightBaudComboBox, 1, 2);
            this.lightSerialPanel.Controls.Add(this.lightDataBitsLabel, 0, 3);
            this.lightSerialPanel.Controls.Add(this.lightDataBitsComboBox, 1, 3);
            this.lightSerialPanel.Controls.Add(this.lightParityLabel, 0, 4);
            this.lightSerialPanel.Controls.Add(this.lightParityComboBox, 1, 4);
            this.lightSerialPanel.Controls.Add(this.lightStopBitsLabel, 0, 5);
            this.lightSerialPanel.Controls.Add(this.lightStopBitsComboBox, 1, 5);
            this.lightSerialPanel.Location = new System.Drawing.Point(471, 0);
            this.lightSerialPanel.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lightSerialPanel.Name = "lightSerialPanel";
            this.lightSerialPanel.Size = new System.Drawing.Size(200, 100);
            this.lightSerialPanel.TabIndex = 1;
            // 
            // lightSerialTitleLabel
            // 
            this.lightSerialTitleLabel.Location = new System.Drawing.Point(3, 0);
            this.lightSerialTitleLabel.Name = "lightSerialTitleLabel";
            this.lightSerialTitleLabel.Size = new System.Drawing.Size(100, 23);
            this.lightSerialTitleLabel.TabIndex = 0;
            this.lightSerialTitleLabel.Text = "串口参数";
            // 
            // lightSerialPortLabel
            // 
            this.lightSerialPortLabel.Location = new System.Drawing.Point(3, 23);
            this.lightSerialPortLabel.Name = "lightSerialPortLabel";
            this.lightSerialPortLabel.Size = new System.Drawing.Size(100, 23);
            this.lightSerialPortLabel.TabIndex = 1;
            this.lightSerialPortLabel.Text = "串口";
            // 
            // lightSerialPortComboBox
            // 
            this.lightSerialPortComboBox.Location = new System.Drawing.Point(109, 26);
            this.lightSerialPortComboBox.Name = "lightSerialPortComboBox";
            this.lightSerialPortComboBox.Size = new System.Drawing.Size(121, 28);
            this.lightSerialPortComboBox.TabIndex = 2;
            // 
            // lightBaudLabel
            // 
            this.lightBaudLabel.Location = new System.Drawing.Point(3, 57);
            this.lightBaudLabel.Name = "lightBaudLabel";
            this.lightBaudLabel.Size = new System.Drawing.Size(100, 23);
            this.lightBaudLabel.TabIndex = 3;
            this.lightBaudLabel.Text = "波特率";
            // 
            // lightBaudComboBox
            // 
            this.lightBaudComboBox.Items.AddRange(new object[] {
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.lightBaudComboBox.Location = new System.Drawing.Point(109, 60);
            this.lightBaudComboBox.Name = "lightBaudComboBox";
            this.lightBaudComboBox.Size = new System.Drawing.Size(121, 28);
            this.lightBaudComboBox.TabIndex = 4;
            // 
            // lightDataBitsLabel
            // 
            this.lightDataBitsLabel.Location = new System.Drawing.Point(3, 91);
            this.lightDataBitsLabel.Name = "lightDataBitsLabel";
            this.lightDataBitsLabel.Size = new System.Drawing.Size(100, 23);
            this.lightDataBitsLabel.TabIndex = 5;
            this.lightDataBitsLabel.Text = "数据位";
            // 
            // lightDataBitsComboBox
            // 
            this.lightDataBitsComboBox.Items.AddRange(new object[] {
            "7",
            "8"});
            this.lightDataBitsComboBox.Location = new System.Drawing.Point(109, 94);
            this.lightDataBitsComboBox.Name = "lightDataBitsComboBox";
            this.lightDataBitsComboBox.Size = new System.Drawing.Size(121, 28);
            this.lightDataBitsComboBox.TabIndex = 6;
            // 
            // lightParityLabel
            // 
            this.lightParityLabel.Location = new System.Drawing.Point(3, 120);
            this.lightParityLabel.Name = "lightParityLabel";
            this.lightParityLabel.Size = new System.Drawing.Size(100, 23);
            this.lightParityLabel.TabIndex = 7;
            this.lightParityLabel.Text = "校验位";
            // 
            // lightParityComboBox
            // 
            this.lightParityComboBox.Items.AddRange(new object[] {
            "None",
            "Odd",
            "Even",
            "Mark",
            "Space"});
            this.lightParityComboBox.Location = new System.Drawing.Point(109, 123);
            this.lightParityComboBox.Name = "lightParityComboBox";
            this.lightParityComboBox.Size = new System.Drawing.Size(121, 28);
            this.lightParityComboBox.TabIndex = 8;
            // 
            // lightStopBitsLabel
            // 
            this.lightStopBitsLabel.Location = new System.Drawing.Point(3, 149);
            this.lightStopBitsLabel.Name = "lightStopBitsLabel";
            this.lightStopBitsLabel.Size = new System.Drawing.Size(100, 23);
            this.lightStopBitsLabel.TabIndex = 9;
            this.lightStopBitsLabel.Text = "停止位";
            // 
            // lightStopBitsComboBox
            // 
            this.lightStopBitsComboBox.Items.AddRange(new object[] {
            "One",
            "OnePointFive",
            "Two"});
            this.lightStopBitsComboBox.Location = new System.Drawing.Point(109, 152);
            this.lightStopBitsComboBox.Name = "lightStopBitsComboBox";
            this.lightStopBitsComboBox.Size = new System.Drawing.Size(121, 28);
            this.lightStopBitsComboBox.TabIndex = 10;
            // 
            // emptyStateLabel
            // 
            this.emptyStateLabel.BackColor = System.Drawing.Color.White;
            this.emptyStateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.emptyStateLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.emptyStateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(112)))), ((int)(((byte)(128)))));
            this.emptyStateLabel.Location = new System.Drawing.Point(23, 58);
            this.emptyStateLabel.Name = "emptyStateLabel";
            this.emptyStateLabel.Size = new System.Drawing.Size(1098, 357);
            this.emptyStateLabel.TabIndex = 3;
            this.emptyStateLabel.Text = "尚未选择设备";
            this.emptyStateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // footerLayout
            // 
            this.footerLayout.ColumnCount = 3;
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.footerLayout.Controls.Add(this.closeButton, 2, 0);
            this.footerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerLayout.Location = new System.Drawing.Point(23, 454);
            this.footerLayout.Name = "footerLayout";
            this.footerLayout.Size = new System.Drawing.Size(1098, 14);
            this.footerLayout.TabIndex = 2;
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.Color.White;
            this.closeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.closeButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(225)))), ((int)(((byte)(232)))));
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.closeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(32)))), ((int)(((byte)(45)))));
            this.closeButton.Location = new System.Drawing.Point(990, 9);
            this.closeButton.Margin = new System.Windows.Forms.Padding(4, 9, 0, 9);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(108, 82);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "关闭";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonOnClick);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // CommunicationFrm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1180, 929);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.MinimumSize = new System.Drawing.Size(820, 560);
            this.Name = "CommunicationFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "通讯连接配置";
            this.rootLayout.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.addButtonPanel.ResumeLayout(false);
            this.workspaceSplitContainer.Panel1.ResumeLayout(false);
            this.workspaceSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.workspaceSplitContainer)).EndInit();
            this.workspaceSplitContainer.ResumeLayout(false);
            this.topologyPanel.ResumeLayout(false);
            this.detailsLayout.ResumeLayout(false);
            this.detailsHeaderPanel.ResumeLayout(false);
            this.detailsHeaderPanel.PerformLayout();
            this.detailsContentPanel.ResumeLayout(false);
            this.commonFieldsLayout.ResumeLayout(false);
            this.commonFieldsLayout.PerformLayout();
            this.settingsTabControl.ResumeLayout(false);
            this.plcTabPage.ResumeLayout(false);
            this.plcFieldsLayout.ResumeLayout(false);
            this.plcEndpointLayout.ResumeLayout(false);
            this.plcNetworkPanel.ResumeLayout(false);
            this.plcNetworkPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plcPortNumeric)).EndInit();
            this.plcSerialPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.plcStationNumeric)).EndInit();
            this.cameraTabPage.ResumeLayout(false);
            this.cameraTabPage.PerformLayout();
            this.cameraFieldsLayout.ResumeLayout(false);
            this.cameraFieldsLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cameraTimeoutNumeric)).EndInit();
            this.lightTabPage.ResumeLayout(false);
            this.lightFieldsLayout.ResumeLayout(false);
            this.lightEndpointLayout.ResumeLayout(false);
            this.lightNetworkPanel.ResumeLayout(false);
            this.lightNetworkPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lightPortNumeric)).EndInit();
            this.lightSerialPanel.ResumeLayout(false);
            this.footerLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.deviceBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        private static void ConfigureFieldLabel(System.Windows.Forms.Label label)
        {
            label.Dock = System.Windows.Forms.DockStyle.Fill;
            label.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            label.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }

        private static void ConfigureTextBox(System.Windows.Forms.TextBox textBox)
        {
            textBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            textBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            textBox.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
        }

        private static void ConfigureComboBox(System.Windows.Forms.ComboBox comboBox)
        {
            comboBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            comboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            comboBox.Margin = new System.Windows.Forms.Padding(0, 7, 0, 7);
        }

        private static void ConfigureNumeric(System.Windows.Forms.NumericUpDown numeric,
            decimal minimum, decimal maximum, decimal value)
        {
            numeric.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            numeric.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            numeric.Dock = System.Windows.Forms.DockStyle.Fill;
            numeric.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            numeric.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            numeric.Minimum = minimum;
            numeric.Maximum = maximum;
            numeric.Value = value;
        }

        private static void ConfigureEndpointPanel(System.Windows.Forms.TableLayoutPanel panel)
        {
            panel.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            panel.ColumnCount = 2;
            panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 86F));
            panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            panel.Dock = System.Windows.Forms.DockStyle.Fill;
            panel.Padding = new System.Windows.Forms.Padding(12, 6, 12, 8);
            panel.RowCount = 6;
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
        }

        private static void ConfigureSectionTitle(System.Windows.Forms.Label label,
            System.Windows.Forms.TableLayoutPanel owner)
        {
            owner.SetColumnSpan(label, 2);
            label.Dock = System.Windows.Forms.DockStyle.Fill;
            label.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            label.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        }
    }
}
