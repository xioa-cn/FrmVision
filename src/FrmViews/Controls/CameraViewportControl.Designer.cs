namespace FrmViews.Controls
{
    partial class CameraViewportControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label productCaptionLabel;
        private System.Windows.Forms.Label productLabel;
        private System.Windows.Forms.Label resultLabel;
        private System.Windows.Forms.Panel statusDot;
        private System.Windows.Forms.Label connectionLabel;
        private System.Windows.Forms.Panel displayHost;
        private Cognex.VisionPro.CogRecordDisplay cogRecordDisplay;
        private System.Windows.Forms.ToolTip recipeToolTip;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CameraViewportControl));
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.nameLabel = new System.Windows.Forms.Label();
            this.productCaptionLabel = new System.Windows.Forms.Label();
            this.productLabel = new System.Windows.Forms.Label();
            this.resultLabel = new System.Windows.Forms.Label();
            this.statusDot = new System.Windows.Forms.Panel();
            this.connectionLabel = new System.Windows.Forms.Label();
            this.displayHost = new System.Windows.Forms.Panel();
            this.cogRecordDisplay = new Cognex.VisionPro.CogRecordDisplay();
            this.recipeToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.rootLayout.SuspendLayout();
            this.headerLayout.SuspendLayout();
            this.displayHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecordDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.BackColor = System.Drawing.Color.White;
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.headerLayout, 0, 0);
            this.rootLayout.Controls.Add(this.displayHost, 0, 1);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Margin = new System.Windows.Forms.Padding(0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 2;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(480, 330);
            this.rootLayout.TabIndex = 0;
            // 
            // headerLayout
            // 
            this.headerLayout.BackColor = System.Drawing.Color.White;
            this.headerLayout.ColumnCount = 6;
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.headerLayout.Controls.Add(this.nameLabel, 0, 0);
            this.headerLayout.Controls.Add(this.productCaptionLabel, 1, 0);
            this.headerLayout.Controls.Add(this.productLabel, 2, 0);
            this.headerLayout.Controls.Add(this.resultLabel, 3, 0);
            this.headerLayout.Controls.Add(this.statusDot, 4, 0);
            this.headerLayout.Controls.Add(this.connectionLabel, 5, 0);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayout.Location = new System.Drawing.Point(0, 0);
            this.headerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.Padding = new System.Windows.Forms.Padding(12, 0, 10, 0);
            this.headerLayout.RowCount = 1;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Size = new System.Drawing.Size(480, 44);
            this.headerLayout.TabIndex = 0;
            // 
            // nameLabel
            // 
            this.nameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.nameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(32)))), ((int)(((byte)(45)))));
            this.nameLabel.Location = new System.Drawing.Point(15, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(66, 44);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "相机 1";
            this.nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // productCaptionLabel
            // 
            this.productCaptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productCaptionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(112)))), ((int)(((byte)(128)))));
            this.productCaptionLabel.Location = new System.Drawing.Point(87, 0);
            this.productCaptionLabel.Name = "productCaptionLabel";
            this.productCaptionLabel.Size = new System.Drawing.Size(32, 44);
            this.productCaptionLabel.TabIndex = 1;
            this.productCaptionLabel.Text = "配方";
            this.productCaptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // productLabel
            // 
            this.productLabel.AutoEllipsis = true;
            this.productLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(242)))), ((int)(((byte)(255)))));
            this.productLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.productLabel.Location = new System.Drawing.Point(122, 10);
            this.productLabel.Margin = new System.Windows.Forms.Padding(0, 10, 8, 10);
            this.productLabel.Name = "productLabel";
            this.productLabel.Size = new System.Drawing.Size(54, 24);
            this.productLabel.TabIndex = 2;
            this.productLabel.Text = "46CC";
            this.productLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // resultLabel
            // 
            this.resultLabel.AutoEllipsis = true;
            this.resultLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(112)))), ((int)(((byte)(128)))));
            this.resultLabel.Location = new System.Drawing.Point(184, 0);
            this.resultLabel.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(212, 44);
            this.resultLabel.TabIndex = 3;
            this.resultLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // statusDot
            // 
            this.statusDot.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.statusDot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.statusDot.Location = new System.Drawing.Point(410, 18);
            this.statusDot.Margin = new System.Windows.Forms.Padding(0);
            this.statusDot.Name = "statusDot";
            this.statusDot.Size = new System.Drawing.Size(7, 7);
            this.statusDot.TabIndex = 4;
            // 
            // connectionLabel
            // 
            this.connectionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.connectionLabel.Location = new System.Drawing.Point(425, 0);
            this.connectionLabel.Name = "connectionLabel";
            this.connectionLabel.Size = new System.Drawing.Size(42, 44);
            this.connectionLabel.TabIndex = 5;
            this.connectionLabel.Text = "离线";
            this.connectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // displayHost
            // 
            this.displayHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(29)))), ((int)(((byte)(40)))));
            this.displayHost.Controls.Add(this.cogRecordDisplay);
            this.displayHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayHost.Location = new System.Drawing.Point(1, 44);
            this.displayHost.Margin = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.displayHost.Name = "displayHost";
            this.displayHost.Size = new System.Drawing.Size(478, 285);
            this.displayHost.TabIndex = 1;
            // 
            // cogRecordDisplay
            // 
            this.cogRecordDisplay.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogRecordDisplay.ColorMapLowerRoiLimit = 0D;
            this.cogRecordDisplay.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogRecordDisplay.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogRecordDisplay.ColorMapUpperRoiLimit = 1D;
            this.cogRecordDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogRecordDisplay.DoubleTapZoomCycleLength = 2;
            this.cogRecordDisplay.DoubleTapZoomSensitivity = 2.5D;
            this.cogRecordDisplay.Location = new System.Drawing.Point(0, 0);
            this.cogRecordDisplay.Margin = new System.Windows.Forms.Padding(0);
            this.cogRecordDisplay.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogRecordDisplay.MouseWheelSensitivity = 1D;
            this.cogRecordDisplay.Name = "cogRecordDisplay";
            this.cogRecordDisplay.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogRecordDisplay.OcxState")));
            this.cogRecordDisplay.Size = new System.Drawing.Size(478, 285);
            this.cogRecordDisplay.TabIndex = 0;
            // 
            // CameraViewportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(225)))), ((int)(((byte)(232)))));
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(260, 180);
            this.Name = "CameraViewportControl";
            this.Size = new System.Drawing.Size(480, 330);
            this.rootLayout.ResumeLayout(false);
            this.headerLayout.ResumeLayout(false);
            this.displayHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogRecordDisplay)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
