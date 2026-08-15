namespace FrmViews.Views
{
    partial class Register
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.TableLayoutPanel fieldsLayout;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Label displayNameLabel;
        private System.Windows.Forms.TextBox displayNameTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label confirmPasswordLabel;
        private System.Windows.Forms.TextBox confirmPasswordTextBox;
        private System.Windows.Forms.CheckBox showPasswordCheckBox;
        private System.Windows.Forms.Label roleHintLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.FlowLayoutPanel footerPanel;
        private System.Windows.Forms.Button registerButton;
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
            this.displayNameLabel = new System.Windows.Forms.Label();
            this.displayNameTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.confirmPasswordLabel = new System.Windows.Forms.Label();
            this.confirmPasswordTextBox = new System.Windows.Forms.TextBox();
            this.showPasswordCheckBox = new System.Windows.Forms.CheckBox();
            this.roleHintLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.footerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.registerButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.rootLayout.SuspendLayout();
            this.headerLayout.SuspendLayout();
            this.fieldsLayout.SuspendLayout();
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
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 282F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.rootLayout.Size = new System.Drawing.Size(540, 500);
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
            this.headerLayout.Size = new System.Drawing.Size(484, 72);
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
            this.titleLabel.Size = new System.Drawing.Size(484, 38);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "用户注册";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.subtitleLabel.Location = new System.Drawing.Point(0, 38);
            this.subtitleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(484, 34);
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "创建本机视觉系统账户";
            this.subtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fieldsLayout
            // 
            this.fieldsLayout.BackColor = System.Drawing.Color.White;
            this.fieldsLayout.ColumnCount = 2;
            this.fieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.fieldsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.fieldsLayout.Controls.Add(this.userNameLabel, 0, 0);
            this.fieldsLayout.Controls.Add(this.userNameTextBox, 1, 0);
            this.fieldsLayout.Controls.Add(this.displayNameLabel, 0, 1);
            this.fieldsLayout.Controls.Add(this.displayNameTextBox, 1, 1);
            this.fieldsLayout.Controls.Add(this.passwordLabel, 0, 2);
            this.fieldsLayout.Controls.Add(this.passwordTextBox, 1, 2);
            this.fieldsLayout.Controls.Add(this.confirmPasswordLabel, 0, 3);
            this.fieldsLayout.Controls.Add(this.confirmPasswordTextBox, 1, 3);
            this.fieldsLayout.Controls.Add(this.showPasswordCheckBox, 1, 4);
            this.fieldsLayout.Controls.Add(this.roleHintLabel, 0, 5);
            this.fieldsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsLayout.Location = new System.Drawing.Point(28, 94);
            this.fieldsLayout.Margin = new System.Windows.Forms.Padding(0);
            this.fieldsLayout.Name = "fieldsLayout";
            this.fieldsLayout.Padding = new System.Windows.Forms.Padding(18, 10, 18, 10);
            this.fieldsLayout.RowCount = 6;
            this.fieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.fieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.fieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.fieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.fieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.fieldsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.fieldsLayout.Size = new System.Drawing.Size(484, 282);
            this.fieldsLayout.TabIndex = 1;
            // 
            // field labels
            // 
            this.userNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userNameLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.userNameLabel.Margin = new System.Windows.Forms.Padding(0);
            this.userNameLabel.Text = "用户名";
            this.userNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.displayNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayNameLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.displayNameLabel.Margin = new System.Windows.Forms.Padding(0);
            this.displayNameLabel.Text = "显示名称";
            this.displayNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.passwordLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passwordLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.passwordLabel.Margin = new System.Windows.Forms.Padding(0);
            this.passwordLabel.Text = "密码";
            this.passwordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.confirmPasswordLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.confirmPasswordLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.confirmPasswordLabel.Margin = new System.Windows.Forms.Padding(0);
            this.confirmPasswordLabel.Text = "确认密码";
            this.confirmPasswordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // text boxes
            // 
            this.userNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.userNameTextBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.userNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.userNameTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this.userNameTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.userNameTextBox.MaxLength = 32;
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(340, 28);
            this.userNameTextBox.TabIndex = 1;
            this.displayNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.displayNameTextBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.displayNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.displayNameTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this.displayNameTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.displayNameTextBox.MaxLength = 50;
            this.displayNameTextBox.Name = "displayNameTextBox";
            this.displayNameTextBox.Size = new System.Drawing.Size(340, 28);
            this.displayNameTextBox.TabIndex = 3;
            this.passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordTextBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.passwordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.passwordTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this.passwordTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.passwordTextBox.MaxLength = 128;
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(340, 28);
            this.passwordTextBox.TabIndex = 5;
            this.passwordTextBox.UseSystemPasswordChar = true;
            this.confirmPasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.confirmPasswordTextBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.confirmPasswordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.confirmPasswordTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this.confirmPasswordTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.confirmPasswordTextBox.MaxLength = 128;
            this.confirmPasswordTextBox.Name = "confirmPasswordTextBox";
            this.confirmPasswordTextBox.Size = new System.Drawing.Size(340, 28);
            this.confirmPasswordTextBox.TabIndex = 7;
            this.confirmPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // showPasswordCheckBox
            // 
            this.showPasswordCheckBox.AutoSize = true;
            this.showPasswordCheckBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.showPasswordCheckBox.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.showPasswordCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.showPasswordCheckBox.Name = "showPasswordCheckBox";
            this.showPasswordCheckBox.Size = new System.Drawing.Size(89, 32);
            this.showPasswordCheckBox.TabIndex = 8;
            this.showPasswordCheckBox.Text = "显示密码";
            this.showPasswordCheckBox.UseVisualStyleBackColor = true;
            this.showPasswordCheckBox.CheckedChanged += new System.EventHandler(this.ShowPasswordCheckBoxOnCheckedChanged);
            // 
            // roleHintLabel
            // 
            this.fieldsLayout.SetColumnSpan(this.roleHintLabel, 2);
            this.roleHintLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roleHintLabel.ForeColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.roleHintLabel.Margin = new System.Windows.Forms.Padding(0);
            this.roleHintLabel.Name = "roleHintLabel";
            this.roleHintLabel.Size = new System.Drawing.Size(448, 38);
            this.roleHintLabel.TabIndex = 9;
            this.roleHintLabel.Text = "正在确认账户角色...";
            this.roleHintLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoEllipsis = true;
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(2, 10, 2, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.TabIndex = 2;
            // 
            // footerPanel
            // 
            this.footerPanel.Controls.Add(this.registerButton);
            this.footerPanel.Controls.Add(this.cancelButton);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.footerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.TabIndex = 3;
            this.footerPanel.WrapContents = false;
            // 
            // registerButton
            // 
            this.registerButton.BackColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.registerButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.registerButton.ForeColor = System.Drawing.Color.White;
            this.registerButton.Margin = new System.Windows.Forms.Padding(8, 5, 0, 5);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(92, 36);
            this.registerButton.TabIndex = 0;
            this.registerButton.Text = "创建用户";
            this.registerButton.UseVisualStyleBackColor = false;
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 225, 232);
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
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
            // Register
            // 
            this.AcceptButton = this.registerButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(540, 500);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Register";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "用户注册";
            this.Shown += new System.EventHandler(this.RegisterOnShown);
            this.rootLayout.ResumeLayout(false);
            this.headerLayout.ResumeLayout(false);
            this.fieldsLayout.ResumeLayout(false);
            this.fieldsLayout.PerformLayout();
            this.footerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
