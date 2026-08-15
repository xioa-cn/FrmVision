namespace FrmViews.Views
{
    partial class HslAuthorizationFrm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.TableLayoutPanel editorLayout;
        private System.Windows.Forms.Label authorizationCodeLabel;
        private System.Windows.Forms.TextBox authorizationCodeTextBox;
        private System.Windows.Forms.CheckBox showAuthorizationCodeCheckBox;
        private System.Windows.Forms.TableLayoutPanel statusLayout;
        private System.Windows.Forms.Panel statusAccentPanel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.FlowLayoutPanel footerPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button closeButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.editorLayout = new System.Windows.Forms.TableLayoutPanel();
            this.authorizationCodeLabel = new System.Windows.Forms.Label();
            this.authorizationCodeTextBox = new System.Windows.Forms.TextBox();
            this.showAuthorizationCodeCheckBox = new System.Windows.Forms.CheckBox();
            this.statusLayout = new System.Windows.Forms.TableLayoutPanel();
            this.statusAccentPanel = new System.Windows.Forms.Panel();
            this.statusLabel = new System.Windows.Forms.Label();
            this.footerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.saveButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.rootLayout.SuspendLayout();
            this.headerLayout.SuspendLayout();
            this.editorLayout.SuspendLayout();
            this.statusLayout.SuspendLayout();
            this.footerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.headerLayout, 0, 0);
            this.rootLayout.Controls.Add(this.editorLayout, 0, 1);
            this.rootLayout.Controls.Add(this.statusLayout, 0, 2);
            this.rootLayout.Controls.Add(this.footerPanel, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Margin = new System.Windows.Forms.Padding(0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(24, 20, 24, 18);
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 118F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.rootLayout.Size = new System.Drawing.Size(640, 360);
            this.rootLayout.TabIndex = 0;
            // 
            // headerLayout
            // 
            this.headerLayout.ColumnCount = 1;
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Controls.Add(this.titleLabel, 0, 0);
            this.headerLayout.Controls.Add(this.subtitleLabel, 0, 1);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayout.Location = new System.Drawing.Point(24, 20);
            this.headerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.RowCount = 2;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Size = new System.Drawing.Size(592, 72);
            this.headerLayout.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            this.titleLabel.Location = new System.Drawing.Point(0, 0);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(592, 36);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "通讯秘钥";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.subtitleLabel.Location = new System.Drawing.Point(0, 36);
            this.subtitleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(592, 36);
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "HSLCommunication 授权秘钥将加密保存到当前 Windows 用户";
            this.subtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // editorLayout
            // 
            this.editorLayout.BackColor = System.Drawing.Color.White;
            this.editorLayout.ColumnCount = 2;
            this.editorLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.editorLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.editorLayout.Controls.Add(this.authorizationCodeLabel, 0, 0);
            this.editorLayout.Controls.Add(this.authorizationCodeTextBox, 0, 1);
            this.editorLayout.Controls.Add(this.showAuthorizationCodeCheckBox, 1, 1);
            this.editorLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorLayout.Location = new System.Drawing.Point(24, 92);
            this.editorLayout.Margin = new System.Windows.Forms.Padding(0);
            this.editorLayout.Name = "editorLayout";
            this.editorLayout.Padding = new System.Windows.Forms.Padding(16, 10, 16, 12);
            this.editorLayout.RowCount = 2;
            this.editorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.editorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.editorLayout.Size = new System.Drawing.Size(592, 118);
            this.editorLayout.TabIndex = 1;
            // 
            // authorizationCodeLabel
            // 
            this.editorLayout.SetColumnSpan(this.authorizationCodeLabel, 2);
            this.authorizationCodeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.authorizationCodeLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.authorizationCodeLabel.Location = new System.Drawing.Point(16, 10);
            this.authorizationCodeLabel.Margin = new System.Windows.Forms.Padding(0);
            this.authorizationCodeLabel.Name = "authorizationCodeLabel";
            this.authorizationCodeLabel.Size = new System.Drawing.Size(560, 34);
            this.authorizationCodeLabel.TabIndex = 0;
            this.authorizationCodeLabel.Text = "HSL 授权秘钥";
            this.authorizationCodeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // authorizationCodeTextBox
            // 
            this.authorizationCodeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.authorizationCodeTextBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.authorizationCodeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.authorizationCodeTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this.authorizationCodeTextBox.Location = new System.Drawing.Point(16, 55);
            this.authorizationCodeTextBox.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.authorizationCodeTextBox.Name = "authorizationCodeTextBox";
            this.authorizationCodeTextBox.Size = new System.Drawing.Size(444, 28);
            this.authorizationCodeTextBox.TabIndex = 1;
            this.authorizationCodeTextBox.UseSystemPasswordChar = true;
            // 
            // showAuthorizationCodeCheckBox
            // 
            this.showAuthorizationCodeCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.showAuthorizationCodeCheckBox.AutoSize = true;
            this.showAuthorizationCodeCheckBox.Location = new System.Drawing.Point(472, 59);
            this.showAuthorizationCodeCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.showAuthorizationCodeCheckBox.Name = "showAuthorizationCodeCheckBox";
            this.showAuthorizationCodeCheckBox.Size = new System.Drawing.Size(75, 21);
            this.showAuthorizationCodeCheckBox.TabIndex = 2;
            this.showAuthorizationCodeCheckBox.Text = "显示秘钥";
            this.showAuthorizationCodeCheckBox.UseVisualStyleBackColor = true;
            this.showAuthorizationCodeCheckBox.CheckedChanged += new System.EventHandler(this.ShowAuthorizationCodeOnCheckedChanged);
            // 
            // statusLayout
            // 
            this.statusLayout.BackColor = System.Drawing.Color.White;
            this.statusLayout.ColumnCount = 2;
            this.statusLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.statusLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusLayout.Controls.Add(this.statusAccentPanel, 0, 0);
            this.statusLayout.Controls.Add(this.statusLabel, 1, 0);
            this.statusLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLayout.Location = new System.Drawing.Point(24, 222);
            this.statusLayout.Margin = new System.Windows.Forms.Padding(0, 12, 0, 12);
            this.statusLayout.Name = "statusLayout";
            this.statusLayout.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.statusLayout.RowCount = 1;
            this.statusLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statusLayout.Size = new System.Drawing.Size(592, 62);
            this.statusLayout.TabIndex = 2;
            // 
            // statusAccentPanel
            // 
            this.statusAccentPanel.BackColor = System.Drawing.Color.FromArgb(217, 119, 6);
            this.statusAccentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusAccentPanel.Location = new System.Drawing.Point(12, 12);
            this.statusAccentPanel.Margin = new System.Windows.Forms.Padding(0, 2, 12, 2);
            this.statusAccentPanel.Name = "statusAccentPanel";
            this.statusAccentPanel.Size = new System.Drawing.Size(6, 38);
            this.statusAccentPanel.TabIndex = 0;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoEllipsis = true;
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(180, 83, 9);
            this.statusLabel.Location = new System.Drawing.Point(30, 10);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(550, 42);
            this.statusLabel.TabIndex = 1;
            this.statusLabel.Text = "当前状态：尚未配置通讯秘钥。";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // footerPanel
            // 
            this.footerPanel.Controls.Add(this.saveButton);
            this.footerPanel.Controls.Add(this.closeButton);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.footerPanel.Location = new System.Drawing.Point(24, 296);
            this.footerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(592, 46);
            this.footerPanel.TabIndex = 3;
            this.footerPanel.WrapContents = false;
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.saveButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.saveButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(24, 68, 190);
            this.saveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(28, 78, 216);
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.ForeColor = System.Drawing.Color.White;
            this.saveButton.Location = new System.Drawing.Point(464, 5);
            this.saveButton.Margin = new System.Windows.Forms.Padding(8, 5, 0, 5);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(128, 36);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "保存并应用";
            this.saveButton.UseVisualStyleBackColor = false;
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 225, 232);
            this.closeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(231, 235, 241);
            this.closeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(241, 244, 248);
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            this.closeButton.Location = new System.Drawing.Point(364, 5);
            this.closeButton.Margin = new System.Windows.Forms.Padding(8, 5, 0, 5);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(92, 36);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "关闭";
            this.closeButton.UseVisualStyleBackColor = false;
            // 
            // HslAuthorizationFrm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(640, 360);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HslAuthorizationFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "通讯秘钥";
            this.rootLayout.ResumeLayout(false);
            this.headerLayout.ResumeLayout(false);
            this.editorLayout.ResumeLayout(false);
            this.editorLayout.PerformLayout();
            this.statusLayout.ResumeLayout(false);
            this.footerPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
