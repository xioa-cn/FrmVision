namespace FrmViews.Views
{
    partial class ParametersConfigFrm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.TableLayoutPanel editorLayout;
        private System.Windows.Forms.Label directoryLabel;
        private System.Windows.Forms.TextBox directoryTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label stateLabel;
        private System.Windows.Forms.FlowLayoutPanel footerPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ErrorProvider errorProvider;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

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
            this.directoryLabel = new System.Windows.Forms.Label();
            this.directoryTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.stateLabel = new System.Windows.Forms.Label();
            this.footerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.rootLayout.SuspendLayout();
            this.headerLayout.SuspendLayout();
            this.editorLayout.SuspendLayout();
            this.footerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.headerLayout, 0, 0);
            this.rootLayout.Controls.Add(this.editorLayout, 0, 1);
            this.rootLayout.Controls.Add(this.stateLabel, 0, 2);
            this.rootLayout.Controls.Add(this.footerPanel, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Margin = new System.Windows.Forms.Padding(0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(24, 20, 24, 18);
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.rootLayout.Size = new System.Drawing.Size(720, 330);
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
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Size = new System.Drawing.Size(672, 66);
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
            this.titleLabel.Size = new System.Drawing.Size(672, 34);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "视觉参数目录";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.subtitleLabel.Location = new System.Drawing.Point(0, 34);
            this.subtitleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(672, 32);
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "产品型录与视觉工具的统一存储位置";
            this.subtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // editorLayout
            // 
            this.editorLayout.BackColor = System.Drawing.Color.White;
            this.editorLayout.ColumnCount = 2;
            this.editorLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.editorLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.editorLayout.Controls.Add(this.directoryLabel, 0, 0);
            this.editorLayout.Controls.Add(this.directoryTextBox, 0, 1);
            this.editorLayout.Controls.Add(this.browseButton, 1, 1);
            this.editorLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorLayout.Location = new System.Drawing.Point(24, 86);
            this.editorLayout.Margin = new System.Windows.Forms.Padding(0);
            this.editorLayout.Name = "editorLayout";
            this.editorLayout.Padding = new System.Windows.Forms.Padding(16, 10, 16, 12);
            this.editorLayout.RowCount = 2;
            this.editorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.editorLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.editorLayout.Size = new System.Drawing.Size(672, 110);
            this.editorLayout.TabIndex = 1;
            // 
            // directoryLabel
            // 
            this.editorLayout.SetColumnSpan(this.directoryLabel, 2);
            this.directoryLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directoryLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.directoryLabel.Location = new System.Drawing.Point(16, 10);
            this.directoryLabel.Margin = new System.Windows.Forms.Padding(0);
            this.directoryLabel.Name = "directoryLabel";
            this.directoryLabel.Size = new System.Drawing.Size(640, 34);
            this.directoryLabel.TabIndex = 0;
            this.directoryLabel.Text = "视觉文件根目录";
            this.directoryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // directoryTextBox
            // 
            this.directoryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))));
            this.directoryTextBox.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.directoryTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.directoryTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            this.directoryTextBox.Location = new System.Drawing.Point(16, 53);
            this.directoryTextBox.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.directoryTextBox.Name = "directoryTextBox";
            this.directoryTextBox.Size = new System.Drawing.Size(516, 28);
            this.directoryTextBox.TabIndex = 1;
            this.directoryTextBox.TextChanged += new System.EventHandler(this.DirectoryTextBoxOnTextChanged);
            // 
            // browseButton
            // 
            this.browseButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.browseButton.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.browseButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 225, 232);
            this.browseButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(231, 235, 241);
            this.browseButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(241, 244, 248);
            this.browseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.browseButton.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            this.browseButton.Location = new System.Drawing.Point(540, 51);
            this.browseButton.Margin = new System.Windows.Forms.Padding(0);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(100, 30);
            this.browseButton.TabIndex = 2;
            this.browseButton.Text = "选择文件夹";
            this.browseButton.UseVisualStyleBackColor = false;
            this.browseButton.Click += new System.EventHandler(this.BrowseButtonOnClick);
            // 
            // stateLabel
            // 
            this.stateLabel.AutoEllipsis = true;
            this.stateLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.stateLabel.Location = new System.Drawing.Point(26, 206);
            this.stateLabel.Margin = new System.Windows.Forms.Padding(2, 10, 2, 0);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(668, 60);
            this.stateLabel.TabIndex = 2;
            this.stateLabel.Text = "尚未配置视觉文件根目录";
            // 
            // footerPanel
            // 
            this.footerPanel.Controls.Add(this.saveButton);
            this.footerPanel.Controls.Add(this.cancelButton);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.footerPanel.Location = new System.Drawing.Point(24, 266);
            this.footerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(672, 46);
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
            this.saveButton.Location = new System.Drawing.Point(580, 5);
            this.saveButton.Margin = new System.Windows.Forms.Padding(8, 5, 0, 5);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(92, 36);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "保存";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.SaveButtonOnClick);
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(220, 225, 232);
            this.cancelButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(231, 235, 241);
            this.cancelButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(241, 244, 248);
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            this.cancelButton.Location = new System.Drawing.Point(480, 5);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(8, 5, 0, 5);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(92, 36);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "取消";
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.CancelButtonOnClick);
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // ParametersConfigFrm
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(720, 330);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ParametersConfigFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "参数路径设置";
            this.rootLayout.ResumeLayout(false);
            this.headerLayout.ResumeLayout(false);
            this.editorLayout.ResumeLayout(false);
            this.editorLayout.PerformLayout();
            this.footerPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
