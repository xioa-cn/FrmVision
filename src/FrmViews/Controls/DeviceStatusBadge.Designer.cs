namespace FrmViews.Controls
{
    partial class DeviceStatusBadge
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Panel statusDot;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label stateLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.statusDot = new System.Windows.Forms.Panel();
            this.nameLabel = new System.Windows.Forms.Label();
            this.stateLabel = new System.Windows.Forms.Label();
            this.rootLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 3;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.statusDot, 0, 0);
            this.rootLayout.Controls.Add(this.nameLabel, 1, 0);
            this.rootLayout.Controls.Add(this.stateLabel, 2, 0);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Margin = System.Windows.Forms.Padding.Empty;
            this.rootLayout.RowCount = 1;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this.statusDot.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.statusDot.BackColor = UiTheme.Danger;
            this.statusDot.Size = new System.Drawing.Size(6, 6);

            this.nameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameLabel.ForeColor = UiTheme.Text;
            this.nameLabel.Margin = System.Windows.Forms.Padding.Empty;
            this.nameLabel.Padding = System.Windows.Forms.Padding.Empty;
            this.nameLabel.Text = "PLC";
            this.nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.nameLabel.UseCompatibleTextRendering = false;

            this.stateLabel.AutoEllipsis = false;
            this.stateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateLabel.ForeColor = UiTheme.Danger;
            this.stateLabel.Margin = System.Windows.Forms.Padding.Empty;
            this.stateLabel.Padding = System.Windows.Forms.Padding.Empty;
            this.stateLabel.Text = "未连接";
            this.stateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.stateLabel.UseCompatibleTextRendering = false;
            //
            // DeviceStatusBadge
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = UiTheme.Surface;
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 8F);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.Name = "DeviceStatusBadge";
            this.Size = new System.Drawing.Size(92, 30);
            this.rootLayout.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
