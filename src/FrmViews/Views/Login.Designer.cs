namespace FrmViews.Views
{
    partial class Login
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.TableLayoutPanel fieldsLayout;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.FlowLayoutPanel optionsPanel;
        private System.Windows.Forms.CheckBox showPasswordCheckBox;
        private System.Windows.Forms.CheckBox rememberPasswordCheckBox;
        private System.Windows.Forms.CheckBox autoLoginCheckBox;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.FlowLayoutPanel footerPanel;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ErrorProvider errorProvider;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
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
            this.fieldsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.optionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.showPasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.rememberPasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.autoLoginCheckBox = new System.Windows.Forms.CheckBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.footerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.loginButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.rootLayout.SuspendLayout();
            this.headerLayout.SuspendLayout();
            this.fieldsLayout.SuspendLayout();
            this.optionsPanel.SuspendLayout();
            this.footerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.headerLayout, 0, 0);
            this.rootLayout.Controls.Add(this.fieldsLayout, 0, 1);
            this.rootLayout.Controls.Add(this.statusLabel, 0, 2);
            this.rootLayout.Controls.Add(this.footerPanel, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(28, 22, 28, 18);
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 154F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.rootLayout.Size = new System.Drawing.Size(500, 370);
            this.rootLayout.TabIndex = 0;
            // 
            // headerLayout
            // 
            this.headerLayout.ColumnCount = 1;
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Controls.Add(this.titleLabel, 0, 0);
            this.headerLayout.Controls.Add(this.subtitleLabel, 0, 1);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayout.Location = new System.Drawing.Point(28, 22);
            this.headerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.RowCount = 2;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Size = new System.Drawing.Size(444, 72);
            this.headerLayout.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            this.titleLabel.Location = new System.Drawing.Point(0, 0);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(444, 38);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "用户登录";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.subtitleLabel.Location = new System.Drawing.Point(0, 38);
            this.subtitleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(444, 34);
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "使用本机账户进入视觉系统";
            this.subtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fieldsLayout
            // 
            this.fieldsLayout.BackColor = System.Drawing.Color.White;
            this.fieldsLayout.ColumnCount = 2;
            this.fieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.fieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fieldsLayout.Controls.Add(this.userNameLabel, 0, 0);
            this.fieldsLayout.Controls.Add(this.userNameTextBox, 1, 0);
            this.fieldsLayout.Controls.Add(this.passwordLabel, 0, 1);
            this.fieldsLayout.Controls.Add(this.passwordTextBox, 1, 1);
            this.fieldsLayout.Controls.Add(this.optionsPanel, 1, 2);
            this.fieldsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsLayout.Location = new System.Drawing.Point(28, 94);
            this.fieldsLayout.Margin = new System.Windows.Forms.Padding(0);
            this.fieldsLayout.Name = "fieldsLayout";
            this.fieldsLayout.Padding = new System.Windows.Forms.Padding(18, 12, 18, 10);
            this.fieldsLayout.RowCount = 3;
            this.fieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.fieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.fieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.fieldsLayout.Size = new System.Drawing.Size(444, 154);
            this.fieldsLayout.TabIndex = 1;
            // 
            // userNameLabel
            // 
            this.userNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userNameLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.userNameLabel.Location = new System.Drawing.Point(18, 12);
            this.userNameLabel.Margin = new System.Windows.Forms.Padding(0);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Size = new System.Drawing.Size(92, 50);
            this.userNameLabel.TabIndex = 0;
            this.userNameLabel.Text = "用户名";
            this.userNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.userNameTextBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.userNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.userNameTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this.userNameTextBox.Location = new System.Drawing.Point(110, 23);
            this.userNameTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.userNameTextBox.MaxLength = 32;
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(316, 28);
            this.userNameTextBox.TabIndex = 1;
            // 
            // passwordLabel
            // 
            this.passwordLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passwordLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.passwordLabel.Location = new System.Drawing.Point(18, 62);
            this.passwordLabel.Margin = new System.Windows.Forms.Padding(0);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(92, 50);
            this.passwordLabel.TabIndex = 2;
            this.passwordLabel.Text = "密码";
            this.passwordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordTextBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.passwordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passwordTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this.passwordTextBox.Location = new System.Drawing.Point(110, 73);
            this.passwordTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.passwordTextBox.MaxLength = 128;
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(316, 28);
            this.passwordTextBox.TabIndex = 3;
            this.passwordTextBox.UseSystemPasswordChar = true;
            // 
            // optionsPanel
            // 
            this.optionsPanel.Controls.Add(this.showPasswordCheckBox);
            this.optionsPanel.Controls.Add(this.rememberPasswordCheckBox);
            this.optionsPanel.Controls.Add(this.autoLoginCheckBox);
            this.optionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsPanel.Location = new System.Drawing.Point(110, 112);
            this.optionsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.optionsPanel.Name = "optionsPanel";
            this.optionsPanel.Size = new System.Drawing.Size(316, 32);
            this.optionsPanel.TabIndex = 4;
            this.optionsPanel.WrapContents = false;
            // 
            // showPasswordCheckBox
            // 
            this.showPasswordCheckBox.AutoSize = true;
            this.showPasswordCheckBox.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.showPasswordCheckBox.Location = new System.Drawing.Point(0, 5);
            this.showPasswordCheckBox.Margin = new System.Windows.Forms.Padding(0, 5, 14, 0);
            this.showPasswordCheckBox.Name = "showPasswordCheckBox";
            this.showPasswordCheckBox.Size = new System.Drawing.Size(89, 21);
            this.showPasswordCheckBox.TabIndex = 0;
            this.showPasswordCheckBox.Text = "显示密码";
            this.showPasswordCheckBox.UseVisualStyleBackColor = true;
            this.showPasswordCheckBox.CheckedChanged += new System.EventHandler(this.ShowPasswordCheckBoxOnCheckedChanged);
            // 
            // rememberPasswordCheckBox
            // 
            this.rememberPasswordCheckBox.AutoSize = true;
            this.rememberPasswordCheckBox.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.rememberPasswordCheckBox.Location = new System.Drawing.Point(103, 5);
            this.rememberPasswordCheckBox.Margin = new System.Windows.Forms.Padding(0, 5, 14, 0);
            this.rememberPasswordCheckBox.Name = "rememberPasswordCheckBox";
            this.rememberPasswordCheckBox.Size = new System.Drawing.Size(89, 21);
            this.rememberPasswordCheckBox.TabIndex = 1;
            this.rememberPasswordCheckBox.Text = "记住密码";
            this.rememberPasswordCheckBox.UseVisualStyleBackColor = true;
            // 
            // autoLoginCheckBox
            // 
            this.autoLoginCheckBox.AutoSize = true;
            this.autoLoginCheckBox.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.autoLoginCheckBox.Location = new System.Drawing.Point(206, 5);
            this.autoLoginCheckBox.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.autoLoginCheckBox.Name = "autoLoginCheckBox";
            this.autoLoginCheckBox.Size = new System.Drawing.Size(89, 21);
            this.autoLoginCheckBox.TabIndex = 2;
            this.autoLoginCheckBox.Text = "自动登录";
            this.autoLoginCheckBox.UseVisualStyleBackColor = true;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoEllipsis = true;
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.statusLabel.Location = new System.Drawing.Point(30, 258);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(2, 10, 2, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(440, 48);
            this.statusLabel.TabIndex = 2;
            // 
            // footerPanel
            // 
            this.footerPanel.Controls.Add(this.loginButton);
            this.footerPanel.Controls.Add(this.cancelButton);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.footerPanel.Location = new System.Drawing.Point(28, 306);
            this.footerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(444, 46);
            this.footerPanel.TabIndex = 3;
            this.footerPanel.WrapContents = false;
            // 
            // loginButton
            // 
            this.loginButton.BackColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.loginButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginButton.ForeColor = System.Drawing.Color.White;
            this.loginButton.Location = new System.Drawing.Point(352, 5);
            this.loginButton.Margin = new System.Windows.Forms.Padding(8, 5, 0, 5);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(92, 36);
            this.loginButton.TabIndex = 0;
            this.loginButton.Text = "登录";
            this.loginButton.UseVisualStyleBackColor = false;
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 225, 232);
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            this.cancelButton.Location = new System.Drawing.Point(252, 5);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(8, 5, 0, 5);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(92, 36);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "取消";
            this.cancelButton.UseVisualStyleBackColor = false;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // Login
            // 
            this.AcceptButton = this.loginButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(500, 370);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "用户登录";
            this.Shown += new System.EventHandler(this.LoginOnShown);
            this.rootLayout.ResumeLayout(false);
            this.headerLayout.ResumeLayout(false);
            this.fieldsLayout.ResumeLayout(false);
            this.fieldsLayout.PerformLayout();
            this.optionsPanel.ResumeLayout(false);
            this.optionsPanel.PerformLayout();
            this.footerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
