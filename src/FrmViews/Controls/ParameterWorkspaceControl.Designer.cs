namespace FrmViews.Controls
{
    partial class ParameterWorkspaceControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel scrollPanel;
        private System.Windows.Forms.TableLayoutPanel contentLayout;
        private System.Windows.Forms.TableLayoutPanel productSection;
        private System.Windows.Forms.Label productTitleLabel;
        private System.Windows.Forms.Label operationModeLabel;
        private System.Windows.Forms.ComboBox operationModeComboBox;
        private System.Windows.Forms.Button reloadButton;
        private System.Windows.Forms.TableLayoutPanel cameraSection;
        private System.Windows.Forms.Label cameraTitleLabel;
        private System.Windows.Forms.TableLayoutPanel cameraGrid;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.scrollPanel = new System.Windows.Forms.Panel();
            this.contentLayout = new System.Windows.Forms.TableLayoutPanel();
            this.productSection = new System.Windows.Forms.TableLayoutPanel();
            this.productTitleLabel = new System.Windows.Forms.Label();
            this.operationModeLabel = new System.Windows.Forms.Label();
            this.operationModeComboBox = new System.Windows.Forms.ComboBox();
            this.reloadButton = new System.Windows.Forms.Button();
            this.cameraSection = new System.Windows.Forms.TableLayoutPanel();
            this.cameraTitleLabel = new System.Windows.Forms.Label();
            this.cameraGrid = new System.Windows.Forms.TableLayoutPanel();
            this.scrollPanel.SuspendLayout();
            this.contentLayout.SuspendLayout();
            this.productSection.SuspendLayout();
            this.cameraSection.SuspendLayout();
            this.SuspendLayout();
            // 
            // scrollPanel
            // 
            this.scrollPanel.AutoScroll = true;
            this.scrollPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.scrollPanel.Controls.Add(this.contentLayout);
            this.scrollPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollPanel.Location = new System.Drawing.Point(0, 0);
            this.scrollPanel.Name = "scrollPanel";
            this.scrollPanel.Padding = new System.Windows.Forms.Padding(20, 16, 20, 16);
            this.scrollPanel.Size = new System.Drawing.Size(1200, 720);
            this.scrollPanel.TabIndex = 0;
            // 
            // contentLayout
            // 
            this.contentLayout.AutoSize = true;
            this.contentLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.contentLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.contentLayout.ColumnCount = 1;
            this.contentLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentLayout.Controls.Add(this.productSection, 0, 0);
            this.contentLayout.Controls.Add(this.cameraSection, 0, 1);
            this.contentLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.contentLayout.Location = new System.Drawing.Point(20, 16);
            this.contentLayout.Margin = new System.Windows.Forms.Padding(0);
            this.contentLayout.Name = "contentLayout";
            this.contentLayout.RowCount = 2;
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.contentLayout.Size = new System.Drawing.Size(1160, 332);
            this.contentLayout.TabIndex = 0;
            // 
            // productSection
            // 
            this.productSection.AutoSize = true;
            this.productSection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.productSection.BackColor = System.Drawing.Color.White;
            this.productSection.ColumnCount = 4;
            this.productSection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 138F));
            this.productSection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 280F));
            this.productSection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.productSection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.productSection.Controls.Add(this.productTitleLabel, 0, 0);
            this.productSection.Controls.Add(this.operationModeLabel, 0, 1);
            this.productSection.Controls.Add(this.operationModeComboBox, 1, 1);
            this.productSection.Controls.Add(this.reloadButton, 2, 1);
            this.productSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productSection.Location = new System.Drawing.Point(6, 6);
            this.productSection.Margin = new System.Windows.Forms.Padding(6);
            this.productSection.Name = "productSection";
            this.productSection.Padding = new System.Windows.Forms.Padding(18, 8, 18, 12);
            this.productSection.RowCount = 3;
            this.productSection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.productSection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.productSection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.productSection.Size = new System.Drawing.Size(1148, 144);
            this.productSection.TabIndex = 0;
            // 
            // productTitleLabel
            // 
            this.productSection.SetColumnSpan(this.productTitleLabel, 4);
            this.productTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productTitleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.productTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(32)))), ((int)(((byte)(45)))));
            this.productTitleLabel.Location = new System.Drawing.Point(21, 8);
            this.productTitleLabel.Name = "productTitleLabel";
            this.productTitleLabel.Size = new System.Drawing.Size(1106, 44);
            this.productTitleLabel.TabIndex = 0;
            this.productTitleLabel.Text = "运行设置";
            this.productTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // operationModeLabel
            // 
            this.operationModeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operationModeLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.operationModeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(112)))), ((int)(((byte)(128)))));
            this.operationModeLabel.Location = new System.Drawing.Point(21, 52);
            this.operationModeLabel.Name = "operationModeLabel";
            this.operationModeLabel.Size = new System.Drawing.Size(132, 48);
            this.operationModeLabel.TabIndex = 1;
            this.operationModeLabel.Text = "运行模式";
            this.operationModeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // operationModeComboBox
            // 
            this.operationModeComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.operationModeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.operationModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.operationModeComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.operationModeComboBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.operationModeComboBox.Items.AddRange(new object[] {
            "联机模式",
            "离线模式",
            "手动模式",
            "停止模式"});
            this.operationModeComboBox.Location = new System.Drawing.Point(156, 59);
            this.operationModeComboBox.Margin = new System.Windows.Forms.Padding(0, 7, 0, 7);
            this.operationModeComboBox.Name = "operationModeComboBox";
            this.operationModeComboBox.Size = new System.Drawing.Size(280, 31);
            this.operationModeComboBox.TabIndex = 2;
            // 
            // reloadButton
            // 
            this.reloadButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reloadButton.Location = new System.Drawing.Point(448, 58);
            this.reloadButton.Margin = new System.Windows.Forms.Padding(12, 6, 0, 6);
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.Size = new System.Drawing.Size(128, 36);
            this.reloadButton.TabIndex = 3;
            this.reloadButton.Text = "刷新型录";
            // 
            // cameraSection
            // 
            this.cameraSection.AutoSize = true;
            this.cameraSection.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cameraSection.BackColor = System.Drawing.Color.White;
            this.cameraSection.ColumnCount = 1;
            this.cameraSection.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cameraSection.Controls.Add(this.cameraTitleLabel, 0, 0);
            this.cameraSection.Controls.Add(this.cameraGrid, 0, 1);
            this.cameraSection.Dock = System.Windows.Forms.DockStyle.Top;
            this.cameraSection.Location = new System.Drawing.Point(6, 162);
            this.cameraSection.Margin = new System.Windows.Forms.Padding(6);
            this.cameraSection.Name = "cameraSection";
            this.cameraSection.Padding = new System.Windows.Forms.Padding(12, 8, 12, 12);
            this.cameraSection.RowCount = 2;
            this.cameraSection.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.cameraSection.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.cameraSection.Size = new System.Drawing.Size(1148, 164);
            this.cameraSection.TabIndex = 1;
            // 
            // cameraTitleLabel
            // 
            this.cameraTitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cameraTitleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.cameraTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(32)))), ((int)(((byte)(45)))));
            this.cameraTitleLabel.Location = new System.Drawing.Point(15, 8);
            this.cameraTitleLabel.Name = "cameraTitleLabel";
            this.cameraTitleLabel.Size = new System.Drawing.Size(1118, 44);
            this.cameraTitleLabel.TabIndex = 0;
            this.cameraTitleLabel.Text = "相机参数型号";
            this.cameraTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cameraGrid
            // 
            this.cameraGrid.AutoSize = true;
            this.cameraGrid.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cameraGrid.BackColor = System.Drawing.Color.White;
            this.cameraGrid.ColumnCount = 1;
            this.cameraGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.cameraGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.cameraGrid.Location = new System.Drawing.Point(12, 52);
            this.cameraGrid.Margin = new System.Windows.Forms.Padding(0);
            this.cameraGrid.MinimumSize = new System.Drawing.Size(0, 100);
            this.cameraGrid.Name = "cameraGrid";
            this.cameraGrid.RowCount = 1;
            this.cameraGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.cameraGrid.Size = new System.Drawing.Size(1124, 100);
            this.cameraGrid.TabIndex = 1;
            // 
            // ParameterWorkspaceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.scrollPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ParameterWorkspaceControl";
            this.Size = new System.Drawing.Size(1200, 720);
            this.scrollPanel.ResumeLayout(false);
            this.scrollPanel.PerformLayout();
            this.contentLayout.ResumeLayout(false);
            this.contentLayout.PerformLayout();
            this.productSection.ResumeLayout(false);
            this.cameraSection.ResumeLayout(false);
            this.cameraSection.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
