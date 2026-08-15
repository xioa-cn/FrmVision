namespace FrmViews.Controls
{
    partial class MainDashboardControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.SplitContainer splitContainer;
        private CameraWorkspaceControl cameraWorkspace;
        private FrmServices.LogServices.Log logControl;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.cameraWorkspace = new CameraWorkspaceControl();
            this.logControl = new FrmServices.LogServices.Log();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer
            //
            this.splitContainer.BackColor = UiTheme.Border;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Margin = System.Windows.Forms.Padding.Empty;
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Panel1.BackColor = UiTheme.Page;
            this.splitContainer.Panel1.Controls.Add(this.cameraWorkspace);
            this.splitContainer.Panel2.BackColor = UiTheme.Surface;
            this.splitContainer.Panel2.Controls.Add(this.logControl);
            this.splitContainer.Size = new System.Drawing.Size(1360, 720);
            this.splitContainer.SplitterDistance = 860;
            this.splitContainer.SplitterWidth = 6;
            this.splitContainer.TabIndex = 0;
            //
            // cameraWorkspace
            //
            this.cameraWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cameraWorkspace.Location = new System.Drawing.Point(0, 0);
            this.cameraWorkspace.Margin = System.Windows.Forms.Padding.Empty;
            this.cameraWorkspace.MinimumSize = new System.Drawing.Size(0, 0);
            this.cameraWorkspace.Name = "cameraWorkspace";
            this.cameraWorkspace.Size = new System.Drawing.Size(860, 720);
            //
            // logControl
            //
            this.logControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logControl.Location = new System.Drawing.Point(0, 0);
            this.logControl.Margin = System.Windows.Forms.Padding.Empty;
            this.logControl.MaximumHistoryEntries = 10000;
            this.logControl.MaximumLiveEntries = 5000;
            this.logControl.MinimumSize = new System.Drawing.Size(0, 0);
            this.logControl.Name = "logControl";
            this.logControl.Size = new System.Drawing.Size(494, 720);
            //
            // MainDashboardControl
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = UiTheme.Page;
            this.Controls.Add(this.splitContainer);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = System.Windows.Forms.Padding.Empty;
            this.Name = "MainDashboardControl";
            this.Size = new System.Drawing.Size(1360, 720);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
