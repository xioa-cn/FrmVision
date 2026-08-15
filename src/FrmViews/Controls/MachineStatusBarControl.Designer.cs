namespace FrmViews.Controls
{
    partial class MachineStatusBarControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Panel topBorderPanel;
        private System.Windows.Forms.FlowLayoutPanel identityPanel;
        private System.Windows.Forms.FlowLayoutPanel devicePanel;
        private System.Windows.Forms.FlowLayoutPanel modePanel;
        private System.Windows.Forms.Label userCaptionLabel;
        private System.Windows.Forms.Label userValueLabel;
        //private System.Windows.Forms.Label productCaptionLabel;
        //private System.Windows.Forms.Label productValueLabel;
        private System.Windows.Forms.Label modeCaptionLabel;
        private System.Windows.Forms.Label modeValueLabel;
        private System.Windows.Forms.Label messageLabel;
        private DeviceStatusBadge plcStatus;
        private DeviceStatusBadge camera1Status;
        private DeviceStatusBadge camera2Status;
        private DeviceStatusBadge camera3Status;
        private DeviceStatusBadge light1Status;
        private DeviceStatusBadge light2Status;
        private DeviceStatusBadge light3Status;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.topBorderPanel = new System.Windows.Forms.Panel();
            this.identityPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.devicePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.modePanel = new System.Windows.Forms.FlowLayoutPanel();
            this.userCaptionLabel = new System.Windows.Forms.Label();
            this.userValueLabel = new System.Windows.Forms.Label();
            //this.productCaptionLabel = new System.Windows.Forms.Label();
            //this.productValueLabel = new System.Windows.Forms.Label();
            this.modeCaptionLabel = new System.Windows.Forms.Label();
            this.modeValueLabel = new System.Windows.Forms.Label();
            this.messageLabel = new System.Windows.Forms.Label();
            this.plcStatus = new DeviceStatusBadge();
            this.camera1Status = new DeviceStatusBadge();
            this.camera2Status = new DeviceStatusBadge();
            this.camera3Status = new DeviceStatusBadge();
            this.light1Status = new DeviceStatusBadge();
            this.light2Status = new DeviceStatusBadge();
            this.light3Status = new DeviceStatusBadge();
            this.rootLayout.SuspendLayout();
            this.identityPanel.SuspendLayout();
            this.devicePanel.SuspendLayout();
            this.modePanel.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.BackColor = UiTheme.Surface;
            this.rootLayout.ColumnCount = 4;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 124F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.rootLayout.Controls.Add(this.identityPanel, 0, 0);
            this.rootLayout.Controls.Add(this.devicePanel, 1, 0);
            this.rootLayout.Controls.Add(this.modePanel, 2, 0);
            this.rootLayout.Controls.Add(this.messageLabel, 3, 0);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Padding = new System.Windows.Forms.Padding(16, 2, 16, 2);
            this.rootLayout.RowCount = 1;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this.identityPanel.Controls.Add(this.userCaptionLabel);
            this.identityPanel.Controls.Add(this.userValueLabel);
            //this.identityPanel.Controls.Add(this.productCaptionLabel);
            //this.identityPanel.Controls.Add(this.productValueLabel);
            this.identityPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.identityPanel.Margin = System.Windows.Forms.Padding.Empty;
            this.identityPanel.WrapContents = false;

            this.devicePanel.AutoScroll = false;
            this.devicePanel.Controls.Add(this.plcStatus);
            this.devicePanel.Controls.Add(this.camera1Status);
            this.devicePanel.Controls.Add(this.camera2Status);
            this.devicePanel.Controls.Add(this.camera3Status);
            this.devicePanel.Controls.Add(this.light1Status);
            this.devicePanel.Controls.Add(this.light2Status);
            this.devicePanel.Controls.Add(this.light3Status);
            this.devicePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.devicePanel.Margin = System.Windows.Forms.Padding.Empty;
            this.devicePanel.WrapContents = false;

            this.modePanel.Controls.Add(this.modeCaptionLabel);
            this.modePanel.Controls.Add(this.modeValueLabel);
            this.modePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modePanel.Margin = System.Windows.Forms.Padding.Empty;
            this.modePanel.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.modePanel.WrapContents = false;

            ConfigureInlineLabel(this.userCaptionLabel, "当前用户", UiTheme.Muted, 64);
            ConfigureInlineLabel(this.userValueLabel, "未登录", UiTheme.Text, 156);
            this.userValueLabel.AutoEllipsis = true;
            //ConfigureInlineLabel(this.productCaptionLabel, "产品", UiTheme.Muted, 36);
            //ConfigureInlineLabel(this.productValueLabel, "46CC", UiTheme.Primary, 48);
            ConfigureInlineLabel(this.modeCaptionLabel, "模式", UiTheme.Muted, 34);
            ConfigureInlineLabel(this.modeValueLabel, "联机模式", UiTheme.Text, 76);

            this.messageLabel.AutoEllipsis = true;
            this.messageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageLabel.ForeColor = UiTheme.Muted;
            this.messageLabel.Text = "系统就绪";
            this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // topBorderPanel
            //
            this.topBorderPanel.BackColor = UiTheme.Border;
            this.topBorderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topBorderPanel.Height = 1;
            //
            // MachineStatusBarControl
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = UiTheme.Surface;
            this.Controls.Add(this.rootLayout);
            this.Controls.Add(this.topBorderPanel);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F);
            this.Margin = System.Windows.Forms.Padding.Empty;
            this.Name = "MachineStatusBarControl";
            this.Size = new System.Drawing.Size(1440, 36);
            this.rootLayout.ResumeLayout(false);
            this.identityPanel.ResumeLayout(false);
            this.devicePanel.ResumeLayout(false);
            this.modePanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private static void ConfigureInlineLabel(
            System.Windows.Forms.Label label, string text, System.Drawing.Color color, int width)
        {
            label.ForeColor = color;
            label.Margin = System.Windows.Forms.Padding.Empty;
            label.Padding = System.Windows.Forms.Padding.Empty;
            label.Size = new System.Drawing.Size(width, 30);
            label.Text = text;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            label.UseCompatibleTextRendering = false;
        }
    }
}
