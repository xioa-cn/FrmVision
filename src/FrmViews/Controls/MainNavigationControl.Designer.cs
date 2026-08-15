namespace FrmViews.Controls
{
    partial class MainNavigationControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel brandLayout;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.FlowLayoutPanel toolbarPanel;
        private System.Windows.Forms.Label brandMarkLabel;
        private System.Windows.Forms.Label brandTitleLabel;
        private System.Windows.Forms.Label brandSubtitleLabel;
        private System.Windows.Forms.Panel toolSeparator1;
        private System.Windows.Forms.Panel toolSeparator2;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem systemMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recordsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registrationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem communicationMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraToolsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualTriggerMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lightControlMenuItem;
        private System.Windows.Forms.ToolStripMenuItem light1MenuItem;
        private System.Windows.Forms.ToolStripMenuItem light2MenuItem;
        private System.Windows.Forms.ToolStripMenuItem light3MenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parametersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem liveLogsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem historyLogsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem storageSettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem camera1MenuItem;
        private System.Windows.Forms.ToolStripMenuItem camera2MenuItem;
        private System.Windows.Forms.ToolStripMenuItem camera3MenuItem;
        private System.Windows.Forms.ToolStripMenuItem camera4MenuItem;
        private System.Windows.Forms.Button homeToolButton;
        private System.Windows.Forms.Button stopToolButton;
        private System.Windows.Forms.Button refreshToolButton;
        private System.Windows.Forms.Button userToolButton;
        private System.Windows.Forms.Button logoutToolButton;
        private System.Windows.Forms.Button openConfigDirectoryToolButton;
        private System.Windows.Forms.Button settingsToolButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.brandLayout = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.toolbarPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.brandMarkLabel = new System.Windows.Forms.Label();
            this.brandTitleLabel = new System.Windows.Forms.Label();
            this.brandSubtitleLabel = new System.Windows.Forms.Label();
            this.toolSeparator1 = new System.Windows.Forms.Panel();
            this.toolSeparator2 = new System.Windows.Forms.Panel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.systemMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recordsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registrationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.communicationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraToolsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualTriggerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lightControlMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.light1MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.light2MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.light3MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parametersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.liveLogsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historyLogsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storageSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.camera1MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.camera2MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.camera3MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.camera4MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.homeToolButton = new System.Windows.Forms.Button();
            this.stopToolButton = new System.Windows.Forms.Button();
            this.refreshToolButton = new System.Windows.Forms.Button();
            this.userToolButton = new System.Windows.Forms.Button();
            this.logoutToolButton = new System.Windows.Forms.Button();
            this.openConfigDirectoryToolButton = new System.Windows.Forms.Button();
            this.settingsToolButton = new System.Windows.Forms.Button();
            this.rootLayout.SuspendLayout();
            this.brandLayout.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.toolbarPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.BackColor = UiTheme.Surface;
            this.rootLayout.ColumnCount = 2;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.brandLayout, 0, 0);
            this.rootLayout.Controls.Add(this.menuStrip, 1, 0);
            this.rootLayout.Controls.Add(this.toolbarPanel, 1, 1);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Margin = System.Windows.Forms.Padding.Empty;
            this.rootLayout.RowCount = 2;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.SetRowSpan(this.brandLayout, 2);
            //
            // brandLayout
            //
            this.brandLayout.BackColor = UiTheme.Surface;
            this.brandLayout.ColumnCount = 2;
            this.brandLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.brandLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.brandLayout.Controls.Add(this.brandMarkLabel, 0, 0);
            this.brandLayout.Controls.Add(this.brandTitleLabel, 1, 0);
            this.brandLayout.Controls.Add(this.brandSubtitleLabel, 1, 1);
            this.brandLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brandLayout.Margin = System.Windows.Forms.Padding.Empty;
            this.brandLayout.Padding = new System.Windows.Forms.Padding(16, 18, 8, 18);
            this.brandLayout.RowCount = 2;
            this.brandLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.brandLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.brandLayout.SetRowSpan(this.brandMarkLabel, 2);

            this.brandMarkLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.brandMarkLabel.BackColor = UiTheme.Primary;
            this.brandMarkLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.brandMarkLabel.ForeColor = System.Drawing.Color.White;
            this.brandMarkLabel.Size = new System.Drawing.Size(40, 40);
            this.brandMarkLabel.Text = "FV";
            this.brandMarkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.brandTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brandTitleLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.brandTitleLabel.ForeColor = UiTheme.Text;
            this.brandTitleLabel.Text = "FrmVision";
            this.brandTitleLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;

            this.brandSubtitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brandSubtitleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F);
            this.brandSubtitleLabel.ForeColor = UiTheme.Muted;
            this.brandSubtitleLabel.Text = "视觉检测系统";
            this.brandSubtitleLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // menuStrip
            //
            this.menuStrip.AutoSize = false;
            this.menuStrip.BackColor = UiTheme.Surface;
            this.menuStrip.CanOverflow = true;
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.5F);
            this.menuStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.systemMenuItem,
                this.recordsMenuItem,
                this.registrationMenuItem,
                this.communicationMenuItem,
                this.usersMenuItem,
                this.cameraToolsMenuItem,
                this.manualTriggerMenuItem,
                this.lightControlMenuItem,
                this.helpMenuItem
            });
            this.menuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip.Margin = System.Windows.Forms.Padding.Empty;
            this.menuStrip.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.menuStrip.Renderer = new ModernMenuRenderer();
            this.menuStrip.ShowItemToolTips = true;
            this.menuStrip.Stretch = true;

            ConfigureDesignMenuItem(this.systemMenuItem, "系统");
            ConfigureDesignMenuItem(this.recordsMenuItem, "查询记录");
            ConfigureDesignMenuItem(this.registrationMenuItem, "软件注册");
            ConfigureDesignMenuItem(this.communicationMenuItem, "通讯");
            ConfigureDesignMenuItem(this.usersMenuItem, "用户管理");
            ConfigureDesignMenuItem(this.cameraToolsMenuItem, "相机工具");
            ConfigureDesignMenuItem(this.manualTriggerMenuItem, "手动触发");
            ConfigureDesignMenuItem(this.lightControlMenuItem, "光源控制");
            ConfigureDesignMenuItem(this.light1MenuItem, "光源控制 1", false);
            ConfigureDesignMenuItem(this.light2MenuItem, "光源控制 2", false);
            ConfigureDesignMenuItem(this.light3MenuItem, "光源控制 3", false);
            ConfigureDesignMenuItem(this.helpMenuItem, "帮助");
            ConfigureDesignMenuItem(this.parametersMenuItem, "配方应用", false);
            ConfigureDesignMenuItem(this.liveLogsMenuItem, "实时日志", false);
            ConfigureDesignMenuItem(this.historyLogsMenuItem, "历史日志", false);
            ConfigureDesignMenuItem(this.storageSettingsMenuItem, "存储设置", false);
            ConfigureDesignMenuItem(this.camera1MenuItem, "相机 1", false);
            ConfigureDesignMenuItem(this.camera2MenuItem, "相机 2", false);
            ConfigureDesignMenuItem(this.camera3MenuItem, "相机 3", false);
            ConfigureDesignMenuItem(this.camera4MenuItem, "相机 4", false);

            this.systemMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.parametersMenuItem
            });
            this.recordsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.liveLogsMenuItem,
                this.historyLogsMenuItem,
                this.storageSettingsMenuItem
            });
            this.cameraToolsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.camera1MenuItem,
                this.camera2MenuItem,
                this.camera3MenuItem,
                this.camera4MenuItem
            });
            this.lightControlMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.light1MenuItem,
                this.light2MenuItem,
                this.light3MenuItem
            });
            ConfigureDesignDropDown(this.systemMenuItem);
            ConfigureDesignDropDown(this.recordsMenuItem);
            ConfigureDesignDropDown(this.cameraToolsMenuItem);
            ConfigureDesignDropDown(this.lightControlMenuItem);
            //
            // toolbarPanel
            //
            this.toolbarPanel.AutoScroll = true;
            this.toolbarPanel.BackColor = UiTheme.SurfaceMuted;
            this.toolbarPanel.Controls.Add(this.homeToolButton);
            this.toolbarPanel.Controls.Add(this.stopToolButton);
            this.toolbarPanel.Controls.Add(this.refreshToolButton);
            this.toolbarPanel.Controls.Add(this.toolSeparator1);
            this.toolbarPanel.Controls.Add(this.userToolButton);
            this.toolbarPanel.Controls.Add(this.toolSeparator2);
            this.toolbarPanel.Controls.Add(this.logoutToolButton);
            this.toolbarPanel.Controls.Add(this.openConfigDirectoryToolButton);
            this.toolbarPanel.Controls.Add(this.settingsToolButton);
            this.toolbarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolbarPanel.Margin = System.Windows.Forms.Padding.Empty;
            this.toolbarPanel.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.toolbarPanel.WrapContents = false;

            ConfigureToolSeparator(this.toolSeparator1);
            ConfigureToolSeparator(this.toolSeparator2);

            UiTheme.StyleIconButton(this.homeToolButton).Text = "\uE80F";
            UiTheme.StyleIconButton(this.stopToolButton).Text = "\uE71A";
            UiTheme.StyleIconButton(this.refreshToolButton).Text = "\uE72C";
            UiTheme.StyleIconButton(this.userToolButton).Text = "\uE77B";
            UiTheme.StyleIconButton(this.logoutToolButton).Text = "\uE8B5";
            UiTheme.StyleIconButton(this.openConfigDirectoryToolButton).Text = "\uE8B7";
            UiTheme.StyleIconButton(this.settingsToolButton).Text = "\uE713";
            this.homeToolButton.BackColor = UiTheme.Primary;
            this.homeToolButton.FlatAppearance.BorderColor = UiTheme.Primary;
            this.homeToolButton.ForeColor = System.Drawing.Color.White;

            this.toolTip.SetToolTip(this.homeToolButton, "主界面");
            this.toolTip.SetToolTip(this.stopToolButton, "停止");
            this.toolTip.SetToolTip(this.refreshToolButton, "刷新状态");
            this.toolTip.SetToolTip(this.userToolButton, "当前用户");
            this.toolTip.SetToolTip(this.logoutToolButton, "退出登录");
            this.toolTip.SetToolTip(this.openConfigDirectoryToolButton, "打开配置目录");
            this.toolTip.SetToolTip(this.settingsToolButton, "配方应用");
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            this.toolTip.AutoPopDelay = 3000;

            this.homeToolButton.Click += new System.EventHandler(this.RaiseHomeRequested);
            this.logoutToolButton.Click += new System.EventHandler(this.RaiseLogoutRequested);
            this.openConfigDirectoryToolButton.Click += new System.EventHandler(this.RaiseOpenConfigDirectoryRequested);
            this.settingsToolButton.Click += new System.EventHandler(this.RaiseParametersRequested);
            //
            // MainNavigationControl
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = UiTheme.Surface;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = System.Windows.Forms.Padding.Empty;
            this.Name = "MainNavigationControl";
            this.Size = new System.Drawing.Size(1280, 104);
            this.rootLayout.ResumeLayout(false);
            this.brandLayout.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolbarPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private static void ConfigureToolSeparator(System.Windows.Forms.Panel separator)
        {
            separator.Anchor = System.Windows.Forms.AnchorStyles.None;
            separator.BackColor = UiTheme.Border;
            separator.Margin = new System.Windows.Forms.Padding(6, 15, 10, 15);
            separator.Size = new System.Drawing.Size(1, 26);
        }

        private static void ConfigureDesignMenuItem(
            System.Windows.Forms.ToolStripMenuItem menuItem,
            string text,
            bool topLevel = true)
        {
            menuItem.AutoSize = true;
            menuItem.ForeColor = UiTheme.Text;
            menuItem.Padding = topLevel
                ? new System.Windows.Forms.Padding(10, 0, 10, 0)
                : new System.Windows.Forms.Padding(10, 0, 18, 0);
            menuItem.Text = text;
            menuItem.TextAlign = topLevel
                ? System.Drawing.ContentAlignment.MiddleCenter
                : System.Drawing.ContentAlignment.MiddleLeft;
        }

        private static void ConfigureDesignDropDown(
            System.Windows.Forms.ToolStripMenuItem ownerItem)
        {
            var preferredItemWidth = 0;
            foreach (System.Windows.Forms.ToolStripItem childItem in ownerItem.DropDownItems)
            {
                preferredItemWidth = System.Math.Max(
                    preferredItemWidth,
                    childItem.GetPreferredSize(System.Drawing.Size.Empty).Width);
            }

            ownerItem.DropDown.AutoSize = true;
            ownerItem.DropDown.LayoutStyle =
                System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            ownerItem.DropDown.Padding = new System.Windows.Forms.Padding(1, 2, 1, 2);
            ownerItem.DropDown.MinimumSize = new System.Drawing.Size(preferredItemWidth + 4, 0);
        }
    }
}
