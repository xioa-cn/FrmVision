namespace FrmViews.Views
{
    partial class PlcComparisonOutputEditorFrm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Panel toolbarPanel;
        private System.Windows.Forms.Label sectionLabel;
        private System.Windows.Forms.FlowLayoutPanel toolbarActions;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.DataGridView conditionGrid;
        private System.Windows.Forms.DataGridViewComboBoxColumn operatorColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueColumn;
        private System.Windows.Forms.Panel footerPanel;
        private System.Windows.Forms.Label footerDivider;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle headerStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle cellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.components = new System.ComponentModel.Container();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.toolbarPanel = new System.Windows.Forms.Panel();
            this.sectionLabel = new System.Windows.Forms.Label();
            this.toolbarActions = new System.Windows.Forms.FlowLayoutPanel();
            this.addButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.conditionGrid = new System.Windows.Forms.DataGridView();
            this.operatorColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.valueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.footerPanel = new System.Windows.Forms.Panel();
            this.footerDivider = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.rootLayout.SuspendLayout();
            this.toolbarPanel.SuspendLayout();
            this.toolbarActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.conditionGrid)).BeginInit();
            this.footerPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.toolbarPanel, 0, 0);
            this.rootLayout.Controls.Add(this.conditionGrid, 0, 1);
            this.rootLayout.Controls.Add(this.footerPanel, 0, 2);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Margin = System.Windows.Forms.Padding.Empty;
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.Padding = new System.Windows.Forms.Padding(16, 10, 16, 0);
            this.rootLayout.RowCount = 3;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.rootLayout.Size = new System.Drawing.Size(540, 300);
            this.rootLayout.TabIndex = 0;
            //
            // toolbarPanel
            //
            this.toolbarPanel.Controls.Add(this.sectionLabel);
            this.toolbarPanel.Controls.Add(this.toolbarActions);
            this.toolbarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolbarPanel.Location = new System.Drawing.Point(16, 10);
            this.toolbarPanel.Margin = System.Windows.Forms.Padding.Empty;
            this.toolbarPanel.Name = "toolbarPanel";
            this.toolbarPanel.Size = new System.Drawing.Size(508, 48);
            this.toolbarPanel.TabIndex = 0;
            //
            // sectionLabel
            //
            this.sectionLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.sectionLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.sectionLabel.ForeColor = FrmViews.Controls.UiTheme.Text;
            this.sectionLabel.Location = new System.Drawing.Point(0, 0);
            this.sectionLabel.Name = "sectionLabel";
            this.sectionLabel.Size = new System.Drawing.Size(110, 48);
            this.sectionLabel.TabIndex = 0;
            this.sectionLabel.Text = "输出条件";
            this.sectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // toolbarActions
            //
            this.toolbarActions.Controls.Add(this.addButton);
            this.toolbarActions.Controls.Add(this.deleteButton);
            this.toolbarActions.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolbarActions.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.toolbarActions.Location = new System.Drawing.Point(304, 0);
            this.toolbarActions.Margin = System.Windows.Forms.Padding.Empty;
            this.toolbarActions.Name = "toolbarActions";
            this.toolbarActions.Padding = new System.Windows.Forms.Padding(0, 7, 0, 0);
            this.toolbarActions.Size = new System.Drawing.Size(204, 48);
            this.toolbarActions.TabIndex = 1;
            //
            // addButton
            //
            this.addButton.BackColor = FrmViews.Controls.UiTheme.PrimarySoft;
            this.addButton.FlatAppearance.BorderColor = FrmViews.Controls.UiTheme.PrimarySoft;
            this.addButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(220, 231, 252);
            this.addButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(226, 237, 255);
            this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.addButton.ForeColor = FrmViews.Controls.UiTheme.Primary;
            this.addButton.Location = new System.Drawing.Point(0, 7);
            this.addButton.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(98, 34);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "+  添加";
            this.addButton.UseVisualStyleBackColor = false;
            this.addButton.Click += new System.EventHandler(this.AddButtonOnClick);
            //
            // deleteButton
            //
            this.deleteButton.BackColor = FrmViews.Controls.UiTheme.Surface;
            this.deleteButton.FlatAppearance.BorderColor = FrmViews.Controls.UiTheme.Border;
            this.deleteButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(231, 235, 241);
            this.deleteButton.FlatAppearance.MouseOverBackColor = FrmViews.Controls.UiTheme.SurfaceMuted;
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.deleteButton.ForeColor = FrmViews.Controls.UiTheme.Muted;
            this.deleteButton.Location = new System.Drawing.Point(106, 7);
            this.deleteButton.Margin = System.Windows.Forms.Padding.Empty;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(98, 34);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "删除";
            this.deleteButton.UseVisualStyleBackColor = false;
            this.deleteButton.Click += new System.EventHandler(this.DeleteButtonOnClick);
            //
            // conditionGrid
            //
            this.conditionGrid.AllowUserToAddRows = false;
            this.conditionGrid.AllowUserToDeleteRows = false;
            this.conditionGrid.AllowUserToResizeColumns = false;
            this.conditionGrid.AllowUserToResizeRows = false;
            this.conditionGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.conditionGrid.BackgroundColor = FrmViews.Controls.UiTheme.Surface;
            this.conditionGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.conditionGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.conditionGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.conditionGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            headerStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            headerStyle.BackColor = FrmViews.Controls.UiTheme.SurfaceMuted;
            headerStyle.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            headerStyle.ForeColor = FrmViews.Controls.UiTheme.Muted;
            headerStyle.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            headerStyle.SelectionBackColor = FrmViews.Controls.UiTheme.SurfaceMuted;
            headerStyle.SelectionForeColor = FrmViews.Controls.UiTheme.Muted;
            this.conditionGrid.ColumnHeadersDefaultCellStyle = headerStyle;
            this.conditionGrid.ColumnHeadersHeight = 38;
            this.conditionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.conditionGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.operatorColumn,
                this.valueColumn});
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.BackColor = FrmViews.Controls.UiTheme.Surface;
            cellStyle.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);
            cellStyle.ForeColor = FrmViews.Controls.UiTheme.Text;
            cellStyle.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            cellStyle.SelectionBackColor = FrmViews.Controls.UiTheme.PrimarySoft;
            cellStyle.SelectionForeColor = FrmViews.Controls.UiTheme.Text;
            this.conditionGrid.DefaultCellStyle = cellStyle;
            this.conditionGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conditionGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.conditionGrid.EnableHeadersVisualStyles = false;
            this.conditionGrid.GridColor = FrmViews.Controls.UiTheme.Border;
            this.conditionGrid.Location = new System.Drawing.Point(16, 58);
            this.conditionGrid.Margin = System.Windows.Forms.Padding.Empty;
            this.conditionGrid.MultiSelect = false;
            this.conditionGrid.Name = "conditionGrid";
            this.conditionGrid.RowHeadersVisible = false;
            this.conditionGrid.RowTemplate.Height = 42;
            this.conditionGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.conditionGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.conditionGrid.ShowCellErrors = false;
            this.conditionGrid.ShowEditingIcon = false;
            this.conditionGrid.ShowRowErrors = false;
            this.conditionGrid.Size = new System.Drawing.Size(508, 180);
            this.conditionGrid.TabIndex = 1;
            //
            // operatorColumn
            //
            this.operatorColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.operatorColumn.FillWeight = 30F;
            this.operatorColumn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.operatorColumn.HeaderText = "比较符";
            this.operatorColumn.Name = "operatorColumn";
            //
            // valueColumn
            //
            this.valueColumn.FillWeight = 70F;
            this.valueColumn.HeaderText = "比较值";
            this.valueColumn.Name = "valueColumn";
            //
            // footerPanel
            //
            this.footerPanel.Controls.Add(this.footerDivider);
            this.footerPanel.Controls.Add(this.okButton);
            this.footerPanel.Controls.Add(this.cancelButton);
            this.footerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.footerPanel.Location = new System.Drawing.Point(16, 238);
            this.footerPanel.Margin = System.Windows.Forms.Padding.Empty;
            this.footerPanel.Name = "footerPanel";
            this.footerPanel.Size = new System.Drawing.Size(508, 62);
            this.footerPanel.TabIndex = 2;
            //
            // footerDivider
            //
            this.footerDivider.BackColor = FrmViews.Controls.UiTheme.Border;
            this.footerDivider.Dock = System.Windows.Forms.DockStyle.Top;
            this.footerDivider.Location = new System.Drawing.Point(0, 0);
            this.footerDivider.Name = "footerDivider";
            this.footerDivider.Size = new System.Drawing.Size(508, 1);
            this.footerDivider.TabIndex = 0;
            //
            // okButton
            //
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.okButton.BackColor = FrmViews.Controls.UiTheme.Primary;
            this.okButton.FlatAppearance.BorderColor = FrmViews.Controls.UiTheme.Primary;
            this.okButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(24, 68, 190);
            this.okButton.FlatAppearance.MouseOverBackColor = FrmViews.Controls.UiTheme.PrimaryHover;
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.okButton.ForeColor = System.Drawing.Color.White;
            this.okButton.Location = new System.Drawing.Point(404, 15);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(104, 36);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "确定";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.OkButtonOnClick);
            //
            // cancelButton
            //
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.cancelButton.BackColor = FrmViews.Controls.UiTheme.Surface;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatAppearance.BorderColor = FrmViews.Controls.UiTheme.Border;
            this.cancelButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(231, 235, 241);
            this.cancelButton.FlatAppearance.MouseOverBackColor = FrmViews.Controls.UiTheme.SurfaceMuted;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.cancelButton.ForeColor = FrmViews.Controls.UiTheme.Text;
            this.cancelButton.Location = new System.Drawing.Point(292, 15);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(104, 36);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "取消";
            this.cancelButton.UseVisualStyleBackColor = false;
            //
            // PlcComparisonOutputEditorFrm
            //
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = FrmViews.Controls.UiTheme.Surface;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(540, 300);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlcComparisonOutputEditorFrm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "编辑比较输出";
            this.rootLayout.ResumeLayout(false);
            this.toolbarPanel.ResumeLayout(false);
            this.toolbarActions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.conditionGrid)).EndInit();
            this.footerPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
