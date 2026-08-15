namespace FrmViews
{
    partial class MainFrm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private FrmViews.Controls.MainNavigationControl navigationControl;
        private FrmViews.Controls.ModernTabControl workspaceTabs;
        private System.Windows.Forms.TabPage mainTabPage;
        private System.Windows.Forms.TabPage parametersTabPage;
        private FrmViews.Controls.MainDashboardControl dashboardControl;
        private FrmViews.Controls.ParameterWorkspaceControl parameterControl;
        private FrmViews.Controls.MachineStatusBarControl statusBarControl;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.navigationControl = new FrmViews.Controls.MainNavigationControl();
            this.workspaceTabs = new FrmViews.Controls.ModernTabControl();
            this.mainTabPage = new System.Windows.Forms.TabPage();
            this.parametersTabPage = new System.Windows.Forms.TabPage();
            this.dashboardControl = new FrmViews.Controls.MainDashboardControl();
            this.parameterControl = new FrmViews.Controls.ParameterWorkspaceControl();
            this.statusBarControl = new FrmViews.Controls.MachineStatusBarControl();
            this.rootLayout.SuspendLayout();
            this.workspaceTabs.SuspendLayout();
            this.mainTabPage.SuspendLayout();
            this.parametersTabPage.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.BackColor = FrmViews.Controls.UiTheme.Surface;
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.navigationControl, 0, 0);
            this.rootLayout.Controls.Add(this.workspaceTabs, 0, 1);
            this.rootLayout.Controls.Add(this.statusBarControl, 0, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Margin = System.Windows.Forms.Padding.Empty;
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            //
            // navigationControl
            //
            this.navigationControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navigationControl.Location = new System.Drawing.Point(0, 0);
            this.navigationControl.Margin = System.Windows.Forms.Padding.Empty;
            this.navigationControl.Name = "navigationControl";
            this.navigationControl.Size = new System.Drawing.Size(1440, 104);
            this.navigationControl.HomeRequested += new System.EventHandler(this.NavigationOnHomeRequested);
            this.navigationControl.RecordsRequested += new System.EventHandler(this.NavigationOnRecordsRequested);
            this.navigationControl.ParametersRequested += new System.EventHandler(this.NavigationOnParametersRequested);
            //
            // workspaceTabs
            //
            this.workspaceTabs.Controls.Add(this.mainTabPage);
            this.workspaceTabs.Controls.Add(this.parametersTabPage);
            this.workspaceTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workspaceTabs.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.workspaceTabs.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.workspaceTabs.ItemSize = new System.Drawing.Size(116, 42);
            this.workspaceTabs.Location = new System.Drawing.Point(0, 104);
            this.workspaceTabs.Margin = System.Windows.Forms.Padding.Empty;
            this.workspaceTabs.Name = "workspaceTabs";
            this.workspaceTabs.Padding = new System.Drawing.Point(16, 4);
            this.workspaceTabs.SelectedIndex = 0;
            this.workspaceTabs.Size = new System.Drawing.Size(1440, 760);
            this.workspaceTabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.workspaceTabs.TabIndex = 1;
            //
            // mainTabPage
            //
            this.mainTabPage.BackColor = FrmViews.Controls.UiTheme.Page;
            this.mainTabPage.Controls.Add(this.dashboardControl);
            this.mainTabPage.Location = new System.Drawing.Point(4, 42);
            this.mainTabPage.Margin = System.Windows.Forms.Padding.Empty;
            this.mainTabPage.Name = "mainTabPage";
            this.mainTabPage.Padding = new System.Windows.Forms.Padding(0);
            this.mainTabPage.Size = new System.Drawing.Size(1432, 714);
            this.mainTabPage.TabIndex = 0;
            this.mainTabPage.Text = "主界面";
            //
            // parametersTabPage
            //
            this.parametersTabPage.BackColor = FrmViews.Controls.UiTheme.Page;
            this.parametersTabPage.Controls.Add(this.parameterControl);
            this.parametersTabPage.Location = new System.Drawing.Point(4, 42);
            this.parametersTabPage.Margin = System.Windows.Forms.Padding.Empty;
            this.parametersTabPage.Name = "parametersTabPage";
            this.parametersTabPage.Padding = new System.Windows.Forms.Padding(0);
            this.parametersTabPage.Size = new System.Drawing.Size(1432, 714);
            this.parametersTabPage.TabIndex = 1;
            this.parametersTabPage.Text = "参数";
            //
            // dashboardControl
            //
            this.dashboardControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dashboardControl.Location = new System.Drawing.Point(0, 0);
            this.dashboardControl.Margin = System.Windows.Forms.Padding.Empty;
            this.dashboardControl.Name = "dashboardControl";
            this.dashboardControl.Size = new System.Drawing.Size(1432, 714);
            //
            // parameterControl
            //
            this.parameterControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parameterControl.Location = new System.Drawing.Point(0, 0);
            this.parameterControl.Margin = System.Windows.Forms.Padding.Empty;
            this.parameterControl.Name = "parameterControl";
            this.parameterControl.Size = new System.Drawing.Size(1432, 714);
            //
            // statusBarControl
            //
            this.statusBarControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusBarControl.Location = new System.Drawing.Point(0, 864);
            this.statusBarControl.Margin = System.Windows.Forms.Padding.Empty;
            this.statusBarControl.Name = "statusBarControl";
            this.statusBarControl.Size = new System.Drawing.Size(1440, 36);
            //
            // MainFrm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = FrmViews.Controls.UiTheme.Page;
            this.ClientSize = new System.Drawing.Size(1440, 900);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(1100, 720);
            this.Name = "MainFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmVision 视觉检测系统";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.rootLayout.ResumeLayout(false);
            this.workspaceTabs.ResumeLayout(false);
            this.mainTabPage.ResumeLayout(false);
            this.parametersTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
