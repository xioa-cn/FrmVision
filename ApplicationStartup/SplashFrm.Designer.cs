using System.ComponentModel;

namespace ApplicationStartup
{
    partial class SplashFrm
    {
        private IContainer components = null;
        private System.Windows.Forms.Panel surfacePanel;
        private System.Windows.Forms.Panel topAccentPanel;
        private System.Windows.Forms.Panel topStatusAccentPanel;
        private System.Windows.Forms.Panel brandPanel;
        private System.Windows.Forms.Panel brandMarkPanel;
        private System.Windows.Forms.Label brandMarkLabel;
        private System.Windows.Forms.Label productNameLabel;
        private System.Windows.Forms.Label productSubtitleLabel;
        private System.Windows.Forms.Label productTypeLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.Label startupTitleLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label percentageLabel;
        private System.Windows.Forms.Panel progressTrackPanel;
        private System.Windows.Forms.Panel progressFillPanel;
        private System.Windows.Forms.FlowLayoutPanel loadingBlocksPanel;
        private System.Windows.Forms.Panel loadingBlock1;
        private System.Windows.Forms.Panel loadingBlock2;
        private System.Windows.Forms.Panel loadingBlock3;
        private System.Windows.Forms.Label loadingHintLabel;
        private System.Windows.Forms.Timer animationTimer;

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
            this.surfacePanel = new System.Windows.Forms.Panel();
            this.brandPanel = new System.Windows.Forms.Panel();
            this.versionLabel = new System.Windows.Forms.Label();
            this.productTypeLabel = new System.Windows.Forms.Label();
            this.productSubtitleLabel = new System.Windows.Forms.Label();
            this.productNameLabel = new System.Windows.Forms.Label();
            this.brandMarkPanel = new System.Windows.Forms.Panel();
            this.brandMarkLabel = new System.Windows.Forms.Label();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.loadingHintLabel = new System.Windows.Forms.Label();
            this.loadingBlocksPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.loadingBlock1 = new System.Windows.Forms.Panel();
            this.loadingBlock2 = new System.Windows.Forms.Panel();
            this.loadingBlock3 = new System.Windows.Forms.Panel();
            this.progressTrackPanel = new System.Windows.Forms.Panel();
            this.progressFillPanel = new System.Windows.Forms.Panel();
            this.percentageLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.startupTitleLabel = new System.Windows.Forms.Label();
            this.topAccentPanel = new System.Windows.Forms.Panel();
            this.topStatusAccentPanel = new System.Windows.Forms.Panel();
            this.animationTimer = new System.Windows.Forms.Timer(this.components);
            this.surfacePanel.SuspendLayout();
            this.brandPanel.SuspendLayout();
            this.brandMarkPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.loadingBlocksPanel.SuspendLayout();
            this.progressTrackPanel.SuspendLayout();
            this.topAccentPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // surfacePanel
            // 
            this.surfacePanel.BackColor = System.Drawing.Color.White;
            this.surfacePanel.Controls.Add(this.brandPanel);
            this.surfacePanel.Controls.Add(this.statusPanel);
            this.surfacePanel.Controls.Add(this.topAccentPanel);
            this.surfacePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.surfacePanel.Location = new System.Drawing.Point(1, 1);
            this.surfacePanel.Margin = new System.Windows.Forms.Padding(0);
            this.surfacePanel.Name = "surfacePanel";
            this.surfacePanel.Size = new System.Drawing.Size(718, 408);
            this.surfacePanel.TabIndex = 0;
            // 
            // brandPanel
            // 
            this.brandPanel.BackColor = System.Drawing.Color.White;
            this.brandPanel.Controls.Add(this.versionLabel);
            this.brandPanel.Controls.Add(this.productTypeLabel);
            this.brandPanel.Controls.Add(this.productSubtitleLabel);
            this.brandPanel.Controls.Add(this.productNameLabel);
            this.brandPanel.Controls.Add(this.brandMarkPanel);
            this.brandPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brandPanel.Location = new System.Drawing.Point(0, 6);
            this.brandPanel.Margin = new System.Windows.Forms.Padding(0);
            this.brandPanel.Name = "brandPanel";
            this.brandPanel.Size = new System.Drawing.Size(718, 246);
            this.brandPanel.TabIndex = 1;
            // 
            // versionLabel
            // 
            this.versionLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.versionLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.versionLabel.Location = new System.Drawing.Point(532, 45);
            this.versionLabel.Margin = new System.Windows.Forms.Padding(0);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(142, 24);
            this.versionLabel.TabIndex = 4;
            this.versionLabel.Text = "版本 1.0.0";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // productTypeLabel
            // 
            this.productTypeLabel.AutoSize = false;
            this.productTypeLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.productTypeLabel.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.productTypeLabel.Location = new System.Drawing.Point(44, 44);
            this.productTypeLabel.Margin = new System.Windows.Forms.Padding(0);
            this.productTypeLabel.Name = "productTypeLabel";
            this.productTypeLabel.Size = new System.Drawing.Size(190, 24);
            this.productTypeLabel.TabIndex = 0;
            this.productTypeLabel.Text = "INDUSTRIAL VISION";
            this.productTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // productSubtitleLabel
            // 
            this.productSubtitleLabel.AutoSize = false;
            this.productSubtitleLabel.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.productSubtitleLabel.Location = new System.Drawing.Point(132, 154);
            this.productSubtitleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.productSubtitleLabel.Name = "productSubtitleLabel";
            this.productSubtitleLabel.Size = new System.Drawing.Size(430, 34);
            this.productSubtitleLabel.TabIndex = 3;
            this.productSubtitleLabel.Text = "视觉检测与自动化控制系统";
            this.productSubtitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // productNameLabel
            // 
            this.productNameLabel.AutoSize = false;
            this.productNameLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 25F, System.Drawing.FontStyle.Bold);
            this.productNameLabel.ForeColor = System.Drawing.Color.FromArgb(25, 32, 44);
            this.productNameLabel.Location = new System.Drawing.Point(132, 98);
            this.productNameLabel.Margin = new System.Windows.Forms.Padding(0);
            this.productNameLabel.Name = "productNameLabel";
            this.productNameLabel.Size = new System.Drawing.Size(430, 56);
            this.productNameLabel.TabIndex = 2;
            this.productNameLabel.Text = "FrmVision";
            this.productNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // brandMarkPanel
            // 
            this.brandMarkPanel.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.brandMarkPanel.Controls.Add(this.brandMarkLabel);
            this.brandMarkPanel.Location = new System.Drawing.Point(44, 104);
            this.brandMarkPanel.Margin = new System.Windows.Forms.Padding(0);
            this.brandMarkPanel.Name = "brandMarkPanel";
            this.brandMarkPanel.Size = new System.Drawing.Size(68, 68);
            this.brandMarkPanel.TabIndex = 1;
            // 
            // brandMarkLabel
            // 
            this.brandMarkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brandMarkLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 18F, System.Drawing.FontStyle.Bold);
            this.brandMarkLabel.ForeColor = System.Drawing.Color.White;
            this.brandMarkLabel.Location = new System.Drawing.Point(0, 0);
            this.brandMarkLabel.Margin = new System.Windows.Forms.Padding(0);
            this.brandMarkLabel.Name = "brandMarkLabel";
            this.brandMarkLabel.Size = new System.Drawing.Size(68, 68);
            this.brandMarkLabel.TabIndex = 0;
            this.brandMarkLabel.Text = "FV";
            this.brandMarkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusPanel
            // 
            this.statusPanel.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.statusPanel.Controls.Add(this.loadingHintLabel);
            this.statusPanel.Controls.Add(this.loadingBlocksPanel);
            this.statusPanel.Controls.Add(this.progressTrackPanel);
            this.statusPanel.Controls.Add(this.percentageLabel);
            this.statusPanel.Controls.Add(this.statusLabel);
            this.statusPanel.Controls.Add(this.startupTitleLabel);
            this.statusPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusPanel.Location = new System.Drawing.Point(0, 252);
            this.statusPanel.Margin = new System.Windows.Forms.Padding(0);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(718, 156);
            this.statusPanel.TabIndex = 2;
            // 
            // loadingHintLabel
            // 
            this.loadingHintLabel.AutoSize = false;
            this.loadingHintLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.loadingHintLabel.Location = new System.Drawing.Point(98, 112);
            this.loadingHintLabel.Margin = new System.Windows.Forms.Padding(0);
            this.loadingHintLabel.Name = "loadingHintLabel";
            this.loadingHintLabel.Size = new System.Drawing.Size(310, 22);
            this.loadingHintLabel.TabIndex = 5;
            this.loadingHintLabel.Text = "正在加载系统组件，请稍候";
            this.loadingHintLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // loadingBlocksPanel
            // 
            this.loadingBlocksPanel.Controls.Add(this.loadingBlock1);
            this.loadingBlocksPanel.Controls.Add(this.loadingBlock2);
            this.loadingBlocksPanel.Controls.Add(this.loadingBlock3);
            this.loadingBlocksPanel.Location = new System.Drawing.Point(44, 116);
            this.loadingBlocksPanel.Margin = new System.Windows.Forms.Padding(0);
            this.loadingBlocksPanel.Name = "loadingBlocksPanel";
            this.loadingBlocksPanel.Size = new System.Drawing.Size(44, 14);
            this.loadingBlocksPanel.TabIndex = 4;
            this.loadingBlocksPanel.WrapContents = false;
            // 
            // loadingBlock1
            // 
            this.loadingBlock1.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.loadingBlock1.Location = new System.Drawing.Point(0, 2);
            this.loadingBlock1.Margin = new System.Windows.Forms.Padding(0, 2, 6, 0);
            this.loadingBlock1.Name = "loadingBlock1";
            this.loadingBlock1.Size = new System.Drawing.Size(8, 8);
            this.loadingBlock1.TabIndex = 0;
            // 
            // loadingBlock2
            // 
            this.loadingBlock2.BackColor = System.Drawing.Color.FromArgb(191, 219, 254);
            this.loadingBlock2.Location = new System.Drawing.Point(14, 2);
            this.loadingBlock2.Margin = new System.Windows.Forms.Padding(0, 2, 6, 0);
            this.loadingBlock2.Name = "loadingBlock2";
            this.loadingBlock2.Size = new System.Drawing.Size(8, 8);
            this.loadingBlock2.TabIndex = 1;
            // 
            // loadingBlock3
            // 
            this.loadingBlock3.BackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            this.loadingBlock3.Location = new System.Drawing.Point(28, 2);
            this.loadingBlock3.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.loadingBlock3.Name = "loadingBlock3";
            this.loadingBlock3.Size = new System.Drawing.Size(8, 8);
            this.loadingBlock3.TabIndex = 2;
            // 
            // progressTrackPanel
            // 
            this.progressTrackPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.progressTrackPanel.BackColor = System.Drawing.Color.FromArgb(218, 224, 232);
            this.progressTrackPanel.Controls.Add(this.progressFillPanel);
            this.progressTrackPanel.Location = new System.Drawing.Point(44, 88);
            this.progressTrackPanel.Margin = new System.Windows.Forms.Padding(0);
            this.progressTrackPanel.Name = "progressTrackPanel";
            this.progressTrackPanel.Size = new System.Drawing.Size(630, 5);
            this.progressTrackPanel.TabIndex = 3;
            // 
            // progressFillPanel
            // 
            this.progressFillPanel.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.progressFillPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.progressFillPanel.Location = new System.Drawing.Point(0, 0);
            this.progressFillPanel.Margin = new System.Windows.Forms.Padding(0);
            this.progressFillPanel.Name = "progressFillPanel";
            this.progressFillPanel.Size = new System.Drawing.Size(50, 5);
            this.progressFillPanel.TabIndex = 0;
            // 
            // percentageLabel
            // 
            this.percentageLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.percentageLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.percentageLabel.ForeColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.percentageLabel.Location = new System.Drawing.Point(604, 28);
            this.percentageLabel.Margin = new System.Windows.Forms.Padding(0);
            this.percentageLabel.Name = "percentageLabel";
            this.percentageLabel.Size = new System.Drawing.Size(70, 30);
            this.percentageLabel.TabIndex = 2;
            this.percentageLabel.Text = "8%";
            this.percentageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoEllipsis = true;
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            this.statusLabel.Location = new System.Drawing.Point(44, 56);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(560, 24);
            this.statusLabel.TabIndex = 1;
            this.statusLabel.Text = "正在准备运行环境";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // startupTitleLabel
            // 
            this.startupTitleLabel.AutoSize = false;
            this.startupTitleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.startupTitleLabel.ForeColor = System.Drawing.Color.FromArgb(25, 32, 44);
            this.startupTitleLabel.Location = new System.Drawing.Point(44, 24);
            this.startupTitleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.startupTitleLabel.Name = "startupTitleLabel";
            this.startupTitleLabel.Size = new System.Drawing.Size(260, 32);
            this.startupTitleLabel.TabIndex = 0;
            this.startupTitleLabel.Text = "正在启动";
            this.startupTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // topAccentPanel
            // 
            this.topAccentPanel.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.topAccentPanel.Controls.Add(this.topStatusAccentPanel);
            this.topAccentPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topAccentPanel.Location = new System.Drawing.Point(0, 0);
            this.topAccentPanel.Margin = new System.Windows.Forms.Padding(0);
            this.topAccentPanel.Name = "topAccentPanel";
            this.topAccentPanel.Size = new System.Drawing.Size(718, 6);
            this.topAccentPanel.TabIndex = 0;
            // 
            // topStatusAccentPanel
            // 
            this.topStatusAccentPanel.BackColor = System.Drawing.Color.FromArgb(22, 163, 74);
            this.topStatusAccentPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.topStatusAccentPanel.Location = new System.Drawing.Point(628, 0);
            this.topStatusAccentPanel.Margin = new System.Windows.Forms.Padding(0);
            this.topStatusAccentPanel.Name = "topStatusAccentPanel";
            this.topStatusAccentPanel.Size = new System.Drawing.Size(90, 6);
            this.topStatusAccentPanel.TabIndex = 0;
            // 
            // animationTimer
            // 
            this.animationTimer.Interval = 32;
            this.animationTimer.Tick += new System.EventHandler(this.AnimationTimerOnTick);
            // 
            // SplashFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.ClientSize = new System.Drawing.Size(720, 410);
            this.ControlBox = false;
            this.Controls.Add(this.surfacePanel);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SplashFrm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmVision 正在启动";
            this.Shown += new System.EventHandler(this.SplashFrmOnShown);
            this.surfacePanel.ResumeLayout(false);
            this.brandPanel.ResumeLayout(false);
            this.brandMarkPanel.ResumeLayout(false);
            this.statusPanel.ResumeLayout(false);
            this.loadingBlocksPanel.ResumeLayout(false);
            this.progressTrackPanel.ResumeLayout(false);
            this.topAccentPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
