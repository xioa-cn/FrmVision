using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrmServices.LogServices;
using FrmServices.Services;
using FrmServices.Services.UserManagement;
using FrmServices.ViewModel;

namespace FrmViews.Controls
{
    public partial class ParameterWorkspaceControl : UserControl
    {
        private readonly ParameterCatalogService _catalogService = new ParameterCatalogService();
        private readonly List<CameraParameterSelector> _cameraSelectors =
            new List<CameraParameterSelector>();
        private IReadOnlyDictionary<string, IReadOnlyList<string>> _parametersByProduct =
            new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);
        private ObservableCollection<CameraPanelViewModel> _cameras;
        private MainFrmViewModel _viewModel;
        private int _reloadVersion;
        private bool _canApplyRecipe;

        public ParameterWorkspaceControl()
        {
            InitializeComponent();
            reloadButton.Click += ReloadButtonOnClick;
            SizeChanged += (sender, args) => LayoutCameraSelectors();
            Disposed += (sender, args) => DetachViewModel();
        }

        public void Bind(MainFrmViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            DetachViewModel();
            _viewModel = viewModel;
            _cameras = viewModel.Cameras;
            _cameras.CollectionChanged += CamerasOnCollectionChanged;

            BindText(operationModeComboBox, viewModel, nameof(viewModel.OperationMode));

            RenderCameraSelectors();
            SetCanApplyRecipe(UserSession.CanApplyRecipe);
            _ = ReloadParameterListsAsync();
        }

        public void SetCanApplyRecipe(bool canApplyRecipe)
        {
            _canApplyRecipe = canApplyRecipe;
            operationModeComboBox.Enabled = canApplyRecipe;
            reloadButton.Enabled = canApplyRecipe;
            foreach (CameraParameterSelector selector in _cameraSelectors)
                selector.SetCanSwitch(canApplyRecipe);
        }

        private void CamerasOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(RenderCameraSelectors));
                return;
            }

            RenderCameraSelectors();
        }

        private void RenderCameraSelectors()
        {
            cameraGrid.SuspendLayout();
            try
            {
                foreach (var selector in _cameraSelectors)
                {
                    selector.SwitchRequested -= CameraSelectorOnSwitchRequested;
                    selector.Dispose();
                }

                _cameraSelectors.Clear();
                cameraGrid.Controls.Clear();
                if (_cameras != null)
                {
                    foreach (var camera in _cameras)
                    {
                        if (!_parametersByProduct.TryGetValue(
                                camera.ProductName, out var parameterNames))
                            parameterNames = Array.Empty<string>();

                        var selector = new CameraParameterSelector
                        {
                            Dock = DockStyle.Fill,
                            Margin = new Padding(6)
                        };
                        selector.Bind(camera, parameterNames);
                        selector.SetCanSwitch(_canApplyRecipe);
                        selector.SwitchRequested += CameraSelectorOnSwitchRequested;
                        _cameraSelectors.Add(selector);
                        cameraGrid.Controls.Add(selector);
                    }
                }

                LayoutCameraSelectors();
            }
            finally
            {
                cameraGrid.ResumeLayout(true);
            }
        }

        private void LayoutCameraSelectors()
        {
            if (_cameraSelectors.Count == 0) return;

            var availableWidth = Math.Max(280,
                ClientSize.Width - scrollPanel.Padding.Horizontal - cameraSection.Padding.Horizontal - 28);
            var columnCount = Math.Max(1, Math.Min(_cameraSelectors.Count, availableWidth / 320));
            var rowCount = (int)Math.Ceiling(_cameraSelectors.Count / (double)columnCount);

            cameraGrid.SuspendLayout();
            try
            {
                cameraGrid.ColumnCount = columnCount;
                cameraGrid.RowCount = rowCount;
                cameraGrid.ColumnStyles.Clear();
                cameraGrid.RowStyles.Clear();
                for (var column = 0; column < columnCount; column++)
                    cameraGrid.ColumnStyles.Add(
                        new ColumnStyle(SizeType.Percent, 100F / columnCount));
                for (var row = 0; row < rowCount; row++)
                    cameraGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 282F));

                for (var index = 0; index < _cameraSelectors.Count; index++)
                    cameraGrid.SetCellPosition(_cameraSelectors[index],
                        new TableLayoutPanelCellPosition(index % columnCount, index / columnCount));
            }
            finally
            {
                cameraGrid.ResumeLayout(true);
            }
        }

        private async void ReloadButtonOnClick(object sender, EventArgs e)
        {
            await ReloadParameterListsAsync();
        }

        private async Task ReloadParameterListsAsync()
        {
            if (_viewModel == null) return;

            var version = ++_reloadVersion;
            reloadButton.Enabled = false;
            //catalogStatusLabel.ForeColor = UiTheme.Muted;
            //catalogStatusLabel.Text = "正在读取参数型录…";
            try
            {
                var catalog = await _catalogService.GetCatalogAsync();
                if (version != _reloadVersion || IsDisposed || Disposing) return;

                _parametersByProduct = catalog;
                RenderCameraSelectors();
                var parameterCount = catalog.Values.Sum(names => names.Count);
                //catalogStatusLabel.Text = "已读取 " + catalog.Count + " 个产品，" +
                //                          parameterCount + " 个参数型号";
                _viewModel.StatusMessage = "参数型录已刷新";
            }
            catch (Exception)
            {
                if (version != _reloadVersion || IsDisposed || Disposing) return;

                _parametersByProduct =
                    new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);
                RenderCameraSelectors();
                //catalogStatusLabel.ForeColor = UiTheme.Danger;
                //catalogStatusLabel.Text = ex.Message;
                _viewModel.StatusMessage = "参数型录读取失败";
            }
            finally
            {
                if (version == _reloadVersion && !IsDisposed && !Disposing)
                    reloadButton.Enabled = _canApplyRecipe;
            }
        }

        private void CameraSelectorOnSwitchRequested(
            object sender, ParameterSwitchRequestedEventArgs e)
        {
            if (!UserSession.CanApplyRecipe)
            {
                MessageBox.Show(this, "配方应用需要员工、工程师或管理员权限。",
                    "权限不足", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                SetCanApplyRecipe(false);
                return;
            }

            e.Camera.ParameterName = e.ParameterName;
            _viewModel?.SaveCameraParameters();
            if (_viewModel != null)
                _viewModel.StatusMessage = e.Camera.Name + " 已切换到参数“" + e.ParameterName + "”";
            AppLog.Info(e.Camera.Name + " 已切换到参数“" + e.ParameterName + "”");
        }

        private void DetachViewModel()
        {
            _reloadVersion++;
            if (_cameras != null)
                _cameras.CollectionChanged -= CamerasOnCollectionChanged;
            _cameras = null;
            _viewModel = null;
        }

        private static void BindText(Control control, object source, string propertyName)
        {
            control.DataBindings.Clear();
            control.DataBindings.Add(nameof(Text), source, propertyName, true,
                DataSourceUpdateMode.OnPropertyChanged);
        }

    }
}
