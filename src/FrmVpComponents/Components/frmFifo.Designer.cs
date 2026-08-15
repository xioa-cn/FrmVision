namespace FrmVpComponents
{
    partial class frmFifo
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private System.Windows.Forms.TableLayoutPanel titleLayout;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btn_保存不带图像的工具;
        private System.Windows.Forms.Panel headerSeparatorPanel;
        private System.Windows.Forms.Panel editorHostPanel;
        private Cognex.VisionPro.CogAcqFifoEditV2 cogAcqFifoEditV21;
        private System.Windows.Forms.ToolTip toolTip1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
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
            this.titleLayout = new System.Windows.Forms.TableLayoutPanel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btn_保存不带图像的工具 = new System.Windows.Forms.Button();
            this.headerSeparatorPanel = new System.Windows.Forms.Panel();
            this.editorHostPanel = new System.Windows.Forms.Panel();
            this.cogAcqFifoEditV21 = new Cognex.VisionPro.CogAcqFifoEditV2();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.rootLayout.SuspendLayout();
            this.headerLayout.SuspendLayout();
            this.titleLayout.SuspendLayout();
            this.editorHostPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogAcqFifoEditV21)).BeginInit();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.headerLayout, 0, 0);
            this.rootLayout.Controls.Add(this.headerSeparatorPanel, 0, 1);
            this.rootLayout.Controls.Add(this.editorHostPanel, 0, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Margin = new System.Windows.Forms.Padding(0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(1180, 760);
            this.rootLayout.TabIndex = 0;
            // 
            // headerLayout
            // 
            this.headerLayout.BackColor = System.Drawing.Color.White;
            this.headerLayout.ColumnCount = 3;
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 124F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 148F));
            this.headerLayout.Controls.Add(this.titleLayout, 0, 0);
            this.headerLayout.Controls.Add(this.btnSave, 1, 0);
            this.headerLayout.Controls.Add(this.btn_保存不带图像的工具, 2, 0);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayout.Location = new System.Drawing.Point(0, 0);
            this.headerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.headerLayout.RowCount = 1;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Size = new System.Drawing.Size(1180, 70);
            this.headerLayout.TabIndex = 0;
            // 
            // titleLayout
            // 
            this.titleLayout.ColumnCount = 1;
            this.titleLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.titleLayout.Controls.Add(this.titleLabel, 0, 0);
            this.titleLayout.Controls.Add(this.subtitleLabel, 0, 1);
            this.titleLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleLayout.Location = new System.Drawing.Point(20, 10);
            this.titleLayout.Margin = new System.Windows.Forms.Padding(0);
            this.titleLayout.Name = "titleLayout";
            this.titleLayout.RowCount = 2;
            this.titleLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.titleLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.titleLayout.Size = new System.Drawing.Size(868, 50);
            this.titleLayout.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoEllipsis = true;
            this.titleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            this.titleLabel.Location = new System.Drawing.Point(0, 0);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(868, 29);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "相机采集配置";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.AutoEllipsis = true;
            this.subtitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subtitleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.5F);
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.subtitleLabel.Location = new System.Drawing.Point(0, 29);
            this.subtitleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(868, 21);
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "Acquisition FIFO 参数与实时采集配置";
            this.subtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(24, 68, 190);
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(28, 78, 216);
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(900, 17);
            this.btnSave.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(112, 36);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "保存工具";
            this.toolTip1.SetToolTip(this.btnSave, "保存完整的相机采集工具");
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btn_保存不带图像的工具
            // 
            this.btn_保存不带图像的工具.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_保存不带图像的工具.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.btn_保存不带图像的工具.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btn_保存不带图像的工具.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(211, 218, 228);
            this.btn_保存不带图像的工具.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(225, 232, 242);
            this.btn_保存不带图像的工具.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(238, 243, 249);
            this.btn_保存不带图像的工具.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_保存不带图像的工具.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.btn_保存不带图像的工具.ForeColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.btn_保存不带图像的工具.Location = new System.Drawing.Point(1024, 17);
            this.btn_保存不带图像的工具.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btn_保存不带图像的工具.Name = "btn_保存不带图像的工具";
            this.btn_保存不带图像的工具.Size = new System.Drawing.Size(136, 36);
            this.btn_保存不带图像的工具.TabIndex = 2;
            this.btn_保存不带图像的工具.Text = "精简保存";
            this.toolTip1.SetToolTip(this.btn_保存不带图像的工具, "保存为不带图像数据的工具文件");
            this.btn_保存不带图像的工具.UseVisualStyleBackColor = false;
            this.btn_保存不带图像的工具.Click += new System.EventHandler(this.btn_保存不带图像的工具_Click);
            // 
            // headerSeparatorPanel
            // 
            this.headerSeparatorPanel.BackColor = System.Drawing.Color.FromArgb(218, 224, 232);
            this.headerSeparatorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerSeparatorPanel.Location = new System.Drawing.Point(0, 70);
            this.headerSeparatorPanel.Margin = new System.Windows.Forms.Padding(0);
            this.headerSeparatorPanel.Name = "headerSeparatorPanel";
            this.headerSeparatorPanel.Size = new System.Drawing.Size(1180, 1);
            this.headerSeparatorPanel.TabIndex = 1;
            // 
            // editorHostPanel
            // 
            this.editorHostPanel.BackColor = System.Drawing.Color.FromArgb(211, 218, 228);
            this.editorHostPanel.Controls.Add(this.cogAcqFifoEditV21);
            this.editorHostPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorHostPanel.Location = new System.Drawing.Point(16, 85);
            this.editorHostPanel.Margin = new System.Windows.Forms.Padding(16, 14, 16, 16);
            this.editorHostPanel.Name = "editorHostPanel";
            this.editorHostPanel.Padding = new System.Windows.Forms.Padding(1);
            this.editorHostPanel.Size = new System.Drawing.Size(1148, 659);
            this.editorHostPanel.TabIndex = 2;
            // 
            // cogAcqFifoEditV21
            // 
            this.cogAcqFifoEditV21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogAcqFifoEditV21.Location = new System.Drawing.Point(1, 1);
            this.cogAcqFifoEditV21.MinimumSize = new System.Drawing.Size(489, 0);
            this.cogAcqFifoEditV21.Name = "cogAcqFifoEditV21";
            this.cogAcqFifoEditV21.Size = new System.Drawing.Size(1146, 657);
            this.cogAcqFifoEditV21.SuspendElectricRuns = false;
            this.cogAcqFifoEditV21.TabIndex = 0;
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 4000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.ReshowDelay = 100;
            // 
            // frmFifo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(244, 247, 250);
            this.ClientSize = new System.Drawing.Size(1180, 760);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "frmFifo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "相机采集配置";
            this.rootLayout.ResumeLayout(false);
            this.headerLayout.ResumeLayout(false);
            this.titleLayout.ResumeLayout(false);
            this.editorHostPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogAcqFifoEditV21)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
