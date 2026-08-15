using System.ComponentModel;

namespace FrmViews.Views
{
    partial class EditorFrm
    {
        private IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Panel toolbarPanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.FlowLayoutPanel toolbarActions;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label statusLabel;
        private ST.Library.UI.NodeEditor.STNodeEditorPannel nodeEditorPannel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.toolbarPanel = new System.Windows.Forms.Panel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.toolbarActions = new System.Windows.Forms.FlowLayoutPanel();
            this.saveButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.nodeEditorPannel = new ST.Library.UI.NodeEditor.STNodeEditorPannel();
            this.rootLayout.SuspendLayout();
            this.toolbarPanel.SuspendLayout();
            this.toolbarActions.SuspendLayout();
            this.SuspendLayout();
            //
            // rootLayout
            //
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.toolbarPanel, 0, 0);
            this.rootLayout.Controls.Add(this.nodeEditorPannel, 0, 1);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Margin = new System.Windows.Forms.Padding(0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 2;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(1200, 760);
            this.rootLayout.TabIndex = 0;
            //
            // toolbarPanel
            //
            this.toolbarPanel.BackColor = System.Drawing.Color.White;
            this.toolbarPanel.Controls.Add(this.titleLabel);
            this.toolbarPanel.Controls.Add(this.toolbarActions);
            this.toolbarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolbarPanel.Location = new System.Drawing.Point(0, 0);
            this.toolbarPanel.Margin = new System.Windows.Forms.Padding(0);
            this.toolbarPanel.Name = "toolbarPanel";
            this.toolbarPanel.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.toolbarPanel.Size = new System.Drawing.Size(1200, 52);
            this.toolbarPanel.TabIndex = 0;
            //
            // titleLabel
            //
            this.titleLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(24, 32, 45);
            this.titleLabel.Location = new System.Drawing.Point(16, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(160, 52);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "流程编辑";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // toolbarActions
            //
            this.toolbarActions.Controls.Add(this.saveButton);
            this.toolbarActions.Controls.Add(this.statusLabel);
            this.toolbarActions.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolbarActions.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.toolbarActions.Location = new System.Drawing.Point(934, 0);
            this.toolbarActions.Margin = new System.Windows.Forms.Padding(0);
            this.toolbarActions.Name = "toolbarActions";
            this.toolbarActions.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.toolbarActions.Size = new System.Drawing.Size(250, 52);
            this.toolbarActions.TabIndex = 1;
            this.toolbarActions.WrapContents = false;
            //
            // saveButton
            //
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.saveButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(36, 99, 235);
            this.saveButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(24, 68, 190);
            this.saveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(28, 78, 216);
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.saveButton.ForeColor = System.Drawing.Color.White;
            this.saveButton.Location = new System.Drawing.Point(150, 8);
            this.saveButton.Margin = new System.Windows.Forms.Padding(0);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(100, 36);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "保存";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.SaveButtonOnClick);
            //
            // statusLabel
            //
            this.statusLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.statusLabel.Location = new System.Drawing.Point(30, 8);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(110, 36);
            this.statusLabel.TabIndex = 1;
            this.statusLabel.Text = "等待加载";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // nodeEditorPannel
            //
            this.nodeEditorPannel.BackColor = System.Drawing.Color.FromArgb(21, 29, 40);
            this.nodeEditorPannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nodeEditorPannel.HandleLineColor = System.Drawing.Color.FromArgb(100, 112, 128);
            this.nodeEditorPannel.LeftLayout = true;
            this.nodeEditorPannel.Location = new System.Drawing.Point(0, 52);
            this.nodeEditorPannel.Margin = new System.Windows.Forms.Padding(0);
            this.nodeEditorPannel.MinimumSize = new System.Drawing.Size(250, 250);
            this.nodeEditorPannel.Name = "nodeEditorPannel";
            this.nodeEditorPannel.PropertyGrid.ShowHelp = false;
            this.nodeEditorPannel.PropertyGrid.ShowLink = false;
            this.nodeEditorPannel.PropertyGrid.ShowMail = false;
            this.nodeEditorPannel.ShowConnectionStatus = true;
            this.nodeEditorPannel.ShowScale = true;
            this.nodeEditorPannel.Size = new System.Drawing.Size(1200, 708);
            this.nodeEditorPannel.SplitLineColor = System.Drawing.Color.FromArgb(55, 65, 81);
            this.nodeEditorPannel.TabIndex = 1;
            //
            // EditorFrm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.ClientSize = new System.Drawing.Size(1200, 760);
            this.Controls.Add(this.rootLayout);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "EditorFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "流程编辑";
            this.rootLayout.ResumeLayout(false);
            this.toolbarPanel.ResumeLayout(false);
            this.toolbarActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
