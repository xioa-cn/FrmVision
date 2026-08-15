using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using FrmCommon;
using FrmViews.Controls;

namespace FrmViews.Views
{
    public partial class ParametersConfigFrm : Form
    {
        public ParametersConfigFrm()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            directoryTextBox.Text = GlobalConfig.Instance?.ConfigCommonDir ?? string.Empty;
            UpdateDirectoryState();
        }

        private void BrowseButtonOnClick(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog
                   {
                       Description = "选择视觉文件根目录",
                       ShowNewFolderButton = true
                   })
            {
                string currentPath = GetExpandedPath();
                if (!string.IsNullOrWhiteSpace(currentPath) &&
                    Directory.Exists(currentPath))
                    dialog.SelectedPath = currentPath;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                    directoryTextBox.Text = dialog.SelectedPath;
            }
        }

        private void DirectoryTextBoxOnTextChanged(object sender, EventArgs e)
        {
            errorProvider.SetError(directoryTextBox, string.Empty);
            UpdateDirectoryState();
        }

        private void UpdateDirectoryState()
        {
            string path = GetExpandedPath();
            if (string.IsNullOrWhiteSpace(path))
            {
                stateLabel.ForeColor = UiTheme.Muted;
                stateLabel.Text = string.IsNullOrWhiteSpace(GlobalConfig.LoadError)
                    ? "尚未配置视觉文件根目录"
                    : "配置读取失败：" + GlobalConfig.LoadError;
                return;
            }

            if (!Directory.Exists(path))
            {
                stateLabel.ForeColor = UiTheme.Danger;
                stateLabel.Text = "目录不可用";
                return;
            }

            string catalogPath = Path.Combine(path, "产品型录");
            bool catalogExists = Directory.Exists(catalogPath);
            stateLabel.ForeColor = catalogExists ? UiTheme.Success : UiTheme.Warning;
            stateLabel.Text = catalogExists
                ? "目录可用，已找到产品型录"
                : "目录可用，尚未找到产品型录";
        }

        private string GetExpandedPath()
        {
            string value = (directoryTextBox.Text ?? string.Empty).Trim();
            if (value.Length == 0) return string.Empty;

            try
            {
                return Path.GetFullPath(Environment.ExpandEnvironmentVariables(value));
            }
            catch
            {
                return value;
            }
        }

        private bool TryGetValidDirectory(out string directory)
        {
            directory = string.Empty;
            string value = (directoryTextBox.Text ?? string.Empty).Trim();
            if (value.Length == 0)
            {
                errorProvider.SetError(directoryTextBox, "请选择视觉文件根目录。");
                directoryTextBox.Focus();
                return false;
            }

            try
            {
                directory = Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(value));
            }
            catch (Exception ex)
            {
                errorProvider.SetError(directoryTextBox,
                    "目录格式无效：" + ex.GetBaseException().Message);
                directoryTextBox.Focus();
                return false;
            }

            if (!Directory.Exists(directory))
            {
                errorProvider.SetError(directoryTextBox, "所选目录不存在。");
                directoryTextBox.Focus();
                return false;
            }

            errorProvider.SetError(directoryTextBox, string.Empty);
            return true;
        }

        private void SaveButtonOnClick(object sender, EventArgs e)
        {
            if (!TryGetValidDirectory(out string directory)) return;

            try
            {
                GlobalConfig.Save(directory);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.GetBaseException().Message,
                    "参数路径设置", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CancelButtonOnClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
