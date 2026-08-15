using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using FrmServices.ViewModel;

namespace FrmViews.Controls
{
    internal sealed partial class CameraParameterSelector : UserControl
    {
        private CameraPanelViewModel _camera;
        private bool _canSwitch;

        public CameraParameterSelector()
        {
            InitializeComponent();
            parameterList.SelectedIndexChanged += (sender, args) =>
                UpdateSwitchButtonState();
            switchButton.Click += SwitchButtonOnClick;
            Disposed += (sender, args) => DetachCamera();
        }

        public event EventHandler<ParameterSwitchRequestedEventArgs> SwitchRequested;

        public void Bind(CameraPanelViewModel camera, IEnumerable<string> parameterNames)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera));

            DetachCamera();
            _camera = camera;
            _camera.PropertyChanged += CameraOnPropertyChanged;
            titleLabel.Text = camera.Name + " 参数型号";
            positionLabel.Text = "检测位置：" + camera.ProductName;
            RefreshParameters(parameterNames);
            UpdateCurrentParameter();
        }

        public void RefreshParameters(IEnumerable<string> parameterNames)
        {
            var names = (parameterNames ?? Enumerable.Empty<string>()).ToArray();
            parameterList.BeginUpdate();
            try
            {
                parameterList.Items.Clear();
                parameterList.Items.AddRange(names.Cast<object>().ToArray());
                SelectCurrentParameter();
            }
            finally
            {
                parameterList.EndUpdate();
            }

            UpdateSwitchButtonState();
        }

        public void SetCanSwitch(bool canSwitch)
        {
            _canSwitch = canSwitch;
            UpdateSwitchButtonState();
        }

        private void SwitchButtonOnClick(object sender, EventArgs e)
        {
            if (!_canSwitch || _camera == null ||
                !(parameterList.SelectedItem is string parameterName)) return;
            SwitchRequested?.Invoke(this,
                new ParameterSwitchRequestedEventArgs(_camera, parameterName));
        }

        private void UpdateSwitchButtonState()
        {
            switchButton.Enabled = _canSwitch &&
                                   parameterList.SelectedItem != null;
        }

        private void CameraOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CameraPanelViewModel.ParameterName))
            {
                UpdateCurrentParameter();
                SelectCurrentParameter();
            }
            else if (e.PropertyName == nameof(CameraPanelViewModel.ProductName))
            {
                positionLabel.Text = "检测位置：" + _camera.ProductName;
            }
        }

        private void UpdateCurrentParameter()
        {
            currentLabel.Text = "当前参数：" + (_camera?.ParameterName ?? string.Empty);
            currentLabel.AutoEllipsis = true;
        }

        private void SelectCurrentParameter()
        {
            if (_camera == null) return;

            var match = parameterList.Items.Cast<object>()
                .FirstOrDefault(item => string.Equals(item?.ToString(), _camera.ParameterName,
                    StringComparison.OrdinalIgnoreCase));
            parameterList.SelectedItem = match;
        }

        private void DetachCamera()
        {
            if (_camera != null)
                _camera.PropertyChanged -= CameraOnPropertyChanged;
            _camera = null;
        }
    }

    internal sealed class ParameterSwitchRequestedEventArgs : EventArgs
    {
        public ParameterSwitchRequestedEventArgs(CameraPanelViewModel camera, string parameterName)
        {
            Camera = camera;
            ParameterName = parameterName;
        }

        public CameraPanelViewModel Camera { get; }
        public string ParameterName { get; }
    }
}
