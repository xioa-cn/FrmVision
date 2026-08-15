namespace FrmViews.Views
{
    partial class UserManager
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.DataGridView usersGrid;
        private System.Windows.Forms.BindingSource usersBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn idColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn userNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn displayNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn roleColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enabledColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdAtColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastLoginAtColumn;
        private System.Windows.Forms.TableLayoutPanel editorLayout;
        private System.Windows.Forms.Label displayNameLabel;
        private System.Windows.Forms.TextBox displayNameTextBox;
        private System.Windows.Forms.Label roleLabel;
        private System.Windows.Forms.ComboBox roleComboBox;
        private System.Windows.Forms.Label newPasswordLabel;
        private System.Windows.Forms.TextBox newPasswordTextBox;
        private System.Windows.Forms.CheckBox enabledCheckBox;
        private System.Windows.Forms.TableLayoutPanel footerLayout;
        private System.Windows.Forms.FlowLayoutPanel leftButtonsPanel;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.FlowLayoutPanel rightButtonsPanel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button resetPasswordButton;
        private System.Windows.Forms.Button saveButton;

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
            this.usersGrid = new System.Windows.Forms.DataGridView();
            this.idColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.userNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.displayNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.roleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.enabledColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.createdAtColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastLoginAtColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.usersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.editorLayout = new System.Windows.Forms.TableLayoutPanel();
            this.displayNameLabel = new System.Windows.Forms.Label();
            this.displayNameTextBox = new System.Windows.Forms.TextBox();
            this.roleLabel = new System.Windows.Forms.Label();
            this.roleComboBox = new System.Windows.Forms.ComboBox();
            this.newPasswordLabel = new System.Windows.Forms.Label();
            this.newPasswordTextBox = new System.Windows.Forms.TextBox();
            this.enabledCheckBox = new System.Windows.Forms.CheckBox();
            this.footerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.leftButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.refreshButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.rightButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.closeButton = new System.Windows.Forms.Button();
            this.resetPasswordButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.rootLayout.SuspendLayout();
            this.headerLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.usersGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersBindingSource)).BeginInit();
            this.editorLayout.SuspendLayout();
            this.footerLayout.SuspendLayout();
            this.leftButtonsPanel.SuspendLayout();
            this.rightButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.headerLayout, 0, 0);
            this.rootLayout.Controls.Add(this.usersGrid, 0, 1);
            this.rootLayout.Controls.Add(this.editorLayout, 0, 2);
            this.rootLayout.Controls.Add(this.footerLayout, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(22, 18, 22, 16);
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 126F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.rootLayout.Size = new System.Drawing.Size(980, 620);
            this.rootLayout.TabIndex = 0;
            // 
            // headerLayout
            // 
            this.headerLayout.ColumnCount = 1;
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Controls.Add(this.titleLabel, 0, 0);
            this.headerLayout.Controls.Add(this.subtitleLabel, 0, 1);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.RowCount = 2;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "用户管理";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.subtitleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "管理本机账户、角色与登录状态";
            this.subtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // usersGrid
            // 
            this.usersGrid.AllowUserToAddRows = false;
            this.usersGrid.AllowUserToDeleteRows = false;
            this.usersGrid.AllowUserToResizeRows = false;
            this.usersGrid.AutoGenerateColumns = false;
            this.usersGrid.BackgroundColor = System.Drawing.Color.White;
            this.usersGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.usersGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.usersGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.usersGrid.ColumnHeadersHeight = 40;
            this.usersGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.usersGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idColumn,
            this.userNameColumn,
            this.displayNameColumn,
            this.roleColumn,
            this.enabledColumn,
            this.createdAtColumn,
            this.lastLoginAtColumn});
            this.usersGrid.DataSource = this.usersBindingSource;
            this.usersGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usersGrid.EnableHeadersVisualStyles = false;
            this.usersGrid.GridColor = System.Drawing.Color.FromArgb(220, 225, 232);
            this.usersGrid.Location = new System.Drawing.Point(22, 84);
            this.usersGrid.Margin = new System.Windows.Forms.Padding(0);
            this.usersGrid.MultiSelect = false;
            this.usersGrid.Name = "usersGrid";
            this.usersGrid.ReadOnly = true;
            this.usersGrid.RowHeadersVisible = false;
            this.usersGrid.RowTemplate.Height = 38;
            this.usersGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.usersGrid.TabIndex = 1;
            // 
            // grid columns
            // 
            this.idColumn.DataPropertyName = "Id";
            this.idColumn.HeaderText = "ID";
            this.idColumn.MinimumWidth = 45;
            this.idColumn.Name = "idColumn";
            this.idColumn.ReadOnly = true;
            this.idColumn.Width = 55;
            this.userNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.userNameColumn.DataPropertyName = "UserName";
            this.userNameColumn.FillWeight = 90F;
            this.userNameColumn.HeaderText = "用户名";
            this.userNameColumn.MinimumWidth = 100;
            this.userNameColumn.Name = "userNameColumn";
            this.userNameColumn.ReadOnly = true;
            this.displayNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.displayNameColumn.DataPropertyName = "DisplayName";
            this.displayNameColumn.HeaderText = "显示名称";
            this.displayNameColumn.MinimumWidth = 110;
            this.displayNameColumn.Name = "displayNameColumn";
            this.displayNameColumn.ReadOnly = true;
            this.roleColumn.DataPropertyName = "Role";
            this.roleColumn.HeaderText = "角色";
            this.roleColumn.Name = "roleColumn";
            this.roleColumn.ReadOnly = true;
            this.roleColumn.Width = 90;
            this.enabledColumn.DataPropertyName = "IsEnabled";
            this.enabledColumn.HeaderText = "启用";
            this.enabledColumn.Name = "enabledColumn";
            this.enabledColumn.ReadOnly = true;
            this.enabledColumn.Width = 65;
            this.createdAtColumn.DataPropertyName = "CreatedAt";
            this.createdAtColumn.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            this.createdAtColumn.HeaderText = "创建时间";
            this.createdAtColumn.Name = "createdAtColumn";
            this.createdAtColumn.ReadOnly = true;
            this.createdAtColumn.Width = 145;
            this.lastLoginAtColumn.DataPropertyName = "LastLoginAt";
            this.lastLoginAtColumn.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            this.lastLoginAtColumn.HeaderText = "最后登录";
            this.lastLoginAtColumn.Name = "lastLoginAtColumn";
            this.lastLoginAtColumn.ReadOnly = true;
            this.lastLoginAtColumn.Width = 145;
            // 
            // usersBindingSource
            // 
            this.usersBindingSource.CurrentChanged += new System.EventHandler(this.UsersBindingSourceOnCurrentChanged);
            // 
            // editorLayout
            // 
            this.editorLayout.BackColor = System.Drawing.Color.White;
            this.editorLayout.ColumnCount = 4;
            this.editorLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.editorLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.editorLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.editorLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.editorLayout.Controls.Add(this.displayNameLabel, 0, 0);
            this.editorLayout.Controls.Add(this.displayNameTextBox, 1, 0);
            this.editorLayout.Controls.Add(this.roleLabel, 2, 0);
            this.editorLayout.Controls.Add(this.roleComboBox, 3, 0);
            this.editorLayout.Controls.Add(this.newPasswordLabel, 0, 1);
            this.editorLayout.Controls.Add(this.newPasswordTextBox, 1, 1);
            this.editorLayout.Controls.Add(this.enabledCheckBox, 3, 1);
            this.editorLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorLayout.Location = new System.Drawing.Point(22, 426);
            this.editorLayout.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.editorLayout.Name = "editorLayout";
            this.editorLayout.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.editorLayout.RowCount = 2;
            this.editorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.editorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.editorLayout.TabIndex = 2;
            // 
            // editor labels
            // 
            this.displayNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayNameLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.displayNameLabel.Margin = new System.Windows.Forms.Padding(0);
            this.displayNameLabel.Name = "displayNameLabel";
            this.displayNameLabel.Text = "显示名称";
            this.displayNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.roleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roleLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.roleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.roleLabel.Name = "roleLabel";
            this.roleLabel.Text = "角色";
            this.roleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.newPasswordLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.newPasswordLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.newPasswordLabel.Margin = new System.Windows.Forms.Padding(0);
            this.newPasswordLabel.Name = "newPasswordLabel";
            this.newPasswordLabel.Text = "新密码";
            this.newPasswordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // displayNameTextBox
            // 
            this.displayNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.displayNameTextBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.displayNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.displayNameTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this.displayNameTextBox.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.displayNameTextBox.MaxLength = 50;
            this.displayNameTextBox.Name = "displayNameTextBox";
            this.displayNameTextBox.TabIndex = 1;
            // 
            // roleComboBox
            // 
            this.roleComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.roleComboBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.roleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.roleComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.roleComboBox.Margin = new System.Windows.Forms.Padding(0);
            this.roleComboBox.Name = "roleComboBox";
            this.roleComboBox.TabIndex = 3;
            // 
            // newPasswordTextBox
            // 
            this.newPasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.newPasswordTextBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.newPasswordTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.newPasswordTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this.newPasswordTextBox.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.newPasswordTextBox.MaxLength = 128;
            this.newPasswordTextBox.Name = "newPasswordTextBox";
            this.newPasswordTextBox.TabIndex = 5;
            this.newPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // enabledCheckBox
            // 
            this.enabledCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.enabledCheckBox.AutoSize = true;
            this.enabledCheckBox.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            this.enabledCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.enabledCheckBox.Name = "enabledCheckBox";
            this.enabledCheckBox.TabIndex = 6;
            this.enabledCheckBox.Text = "允许登录";
            this.enabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // footerLayout
            // 
            this.footerLayout.ColumnCount = 3;
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.footerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.footerLayout.Controls.Add(this.leftButtonsPanel, 0, 0);
            this.footerLayout.Controls.Add(this.statusLabel, 1, 0);
            this.footerLayout.Controls.Add(this.rightButtonsPanel, 2, 0);
            this.footerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.footerLayout.Name = "footerLayout";
            this.footerLayout.TabIndex = 3;
            // 
            // leftButtonsPanel
            // 
            this.leftButtonsPanel.AutoSize = true;
            this.leftButtonsPanel.Controls.Add(this.refreshButton);
            this.leftButtonsPanel.Controls.Add(this.deleteButton);
            this.leftButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftButtonsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.leftButtonsPanel.Name = "leftButtonsPanel";
            this.leftButtonsPanel.WrapContents = false;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoEllipsis = true;
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(12, 0, 12, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rightButtonsPanel
            // 
            this.rightButtonsPanel.AutoSize = true;
            this.rightButtonsPanel.Controls.Add(this.closeButton);
            this.rightButtonsPanel.Controls.Add(this.resetPasswordButton);
            this.rightButtonsPanel.Controls.Add(this.saveButton);
            this.rightButtonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightButtonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.rightButtonsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.rightButtonsPanel.Name = "rightButtonsPanel";
            this.rightButtonsPanel.WrapContents = false;
            // 
            // command buttons
            // 
            this.refreshButton.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.refreshButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 225, 232);
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshButton.Margin = new System.Windows.Forms.Padding(0, 8, 8, 8);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(82, 36);
            this.refreshButton.Text = "刷新";
            this.refreshButton.UseVisualStyleBackColor = false;
            this.deleteButton.BackColor = System.Drawing.Color.White;
            this.deleteButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteButton.ForeColor = System.Drawing.Color.FromArgb(220, 38, 38);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(0, 8, 8, 8);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(82, 36);
            this.deleteButton.Text = "删除";
            this.deleteButton.UseVisualStyleBackColor = false;
            this.closeButton.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 225, 232);
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Margin = new System.Windows.Forms.Padding(8, 8, 0, 8);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(82, 36);
            this.closeButton.Text = "关闭";
            this.closeButton.UseVisualStyleBackColor = false;
            this.resetPasswordButton.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.resetPasswordButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 225, 232);
            this.resetPasswordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.resetPasswordButton.Margin = new System.Windows.Forms.Padding(8, 8, 0, 8);
            this.resetPasswordButton.Name = "resetPasswordButton";
            this.resetPasswordButton.Size = new System.Drawing.Size(98, 36);
            this.resetPasswordButton.Text = "重置密码";
            this.resetPasswordButton.UseVisualStyleBackColor = false;
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.saveButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.ForeColor = System.Drawing.Color.White;
            this.saveButton.Margin = new System.Windows.Forms.Padding(8, 8, 0, 8);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(82, 36);
            this.saveButton.Text = "保存";
            this.saveButton.UseVisualStyleBackColor = false;
            // 
            // UserManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(980, 620);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.MinimumSize = new System.Drawing.Size(820, 540);
            this.Name = "UserManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "用户管理";
            this.Shown += new System.EventHandler(this.UserManagerOnShown);
            this.rootLayout.ResumeLayout(false);
            this.headerLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.usersGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.usersBindingSource)).EndInit();
            this.editorLayout.ResumeLayout(false);
            this.editorLayout.PerformLayout();
            this.footerLayout.ResumeLayout(false);
            this.footerLayout.PerformLayout();
            this.leftButtonsPanel.ResumeLayout(false);
            this.rightButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
