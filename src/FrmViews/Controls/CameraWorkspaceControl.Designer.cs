namespace FrmViews.Controls
{
    partial class CameraWorkspaceControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel cameraLayout;
        private CameraViewportControl camera1;
        private CameraViewportControl camera2;
        private CameraViewportControl camera3;
        private CameraViewportControl camera4;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cameraLayout = new System.Windows.Forms.TableLayoutPanel();
            this.camera1 = new CameraViewportControl();
            this.camera2 = new CameraViewportControl();
            this.camera3 = new CameraViewportControl();
            this.camera4 = new CameraViewportControl();
            this.cameraLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // cameraLayout
            //
            this.cameraLayout.BackColor = UiTheme.Page;
            this.cameraLayout.ColumnCount = 2;
            this.cameraLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.cameraLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.cameraLayout.Controls.Add(this.camera1, 0, 0);
            this.cameraLayout.Controls.Add(this.camera2, 1, 0);
            this.cameraLayout.Controls.Add(this.camera3, 0, 1);
            this.cameraLayout.Controls.Add(this.camera4, 1, 1);
            this.cameraLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cameraLayout.Margin = System.Windows.Forms.Padding.Empty;
            this.cameraLayout.Padding = new System.Windows.Forms.Padding(6);
            this.cameraLayout.RowCount = 2;
            this.cameraLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.cameraLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));

            this.camera1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.camera2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.camera3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.camera4.Dock = System.Windows.Forms.DockStyle.Fill;
            //
            // CameraWorkspaceControl
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = UiTheme.Page;
            this.Controls.Add(this.cameraLayout);
            this.Margin = System.Windows.Forms.Padding.Empty;
            this.MinimumSize = new System.Drawing.Size(560, 420);
            this.Name = "CameraWorkspaceControl";
            this.Size = new System.Drawing.Size(900, 680);
            this.cameraLayout.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
