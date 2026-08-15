namespace FrmViews.Controls
{
    partial class CameraParameterSelector
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label positionLabel;
        private System.Windows.Forms.Label currentLabel;
        private System.Windows.Forms.ListBox parameterList;
        private System.Windows.Forms.TableLayoutPanel actionLayout;
        private System.Windows.Forms.Button switchButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.positionLabel = new System.Windows.Forms.Label();
            this.currentLabel = new System.Windows.Forms.Label();
            this.parameterList = new System.Windows.Forms.ListBox();
            this.actionLayout = new System.Windows.Forms.TableLayoutPanel();
            this.switchButton = new System.Windows.Forms.Button();
            this.rootLayout.SuspendLayout();
            this.actionLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.BackColor = UiTheme.SurfaceMuted;
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.titleLabel, 0, 0);
            this.rootLayout.Controls.Add(this.positionLabel, 0, 1);
            this.rootLayout.Controls.Add(this.currentLabel, 0, 2);
            this.rootLayout.Controls.Add(this.parameterList, 0, 3);
            this.rootLayout.Controls.Add(this.actionLayout, 0, 4);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Margin = System.Windows.Forms.Padding.Empty;
            this.rootLayout.Padding = new System.Windows.Forms.Padding(14, 10, 14, 12);
            this.rootLayout.RowCount = 5;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            //
            // titleLabel
            //
            this.titleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = UiTheme.Text;
            this.titleLabel.Text = "相机 1 参数型号";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // positionLabel
            //
            this.positionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.positionLabel.ForeColor = UiTheme.Muted;
            this.positionLabel.Text = "检测位置：检测1";
            this.positionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // currentLabel
            //
            this.currentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentLabel.AutoEllipsis = true;
            this.currentLabel.ForeColor = UiTheme.Primary;
            this.currentLabel.Text = "当前参数：66CC";
            this.currentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // parameterList
            //
            this.parameterList.BackColor = UiTheme.Surface;
            this.parameterList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.parameterList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parameterList.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this.parameterList.FormattingEnabled = true;
            this.parameterList.IntegralHeight = false;
            this.parameterList.Items.AddRange(new object[] { "66CC", "90CC" });
            this.parameterList.Margin = new System.Windows.Forms.Padding(0, 4, 0, 8);
            //
            // actionLayout
            //
            this.actionLayout.ColumnCount = 1;
            this.actionLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.actionLayout.Controls.Add(this.switchButton, 0, 0);
            this.actionLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionLayout.Margin = System.Windows.Forms.Padding.Empty;
            this.actionLayout.RowCount = 1;
            this.actionLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            //
            // switchButton
            //
            UiTheme.StyleCommandButton(this.switchButton, true);
            this.switchButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.switchButton.Margin = System.Windows.Forms.Padding.Empty;
            this.switchButton.Text = "切换到选中参数";
            this.switchButton.Width = 142;
            //
            // CameraParameterSelector
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = UiTheme.SurfaceMuted;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.rootLayout);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MinimumSize = new System.Drawing.Size(280, 250);
            this.Name = "CameraParameterSelector";
            this.Size = new System.Drawing.Size(330, 270);
            this.rootLayout.ResumeLayout(false);
            this.actionLayout.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
