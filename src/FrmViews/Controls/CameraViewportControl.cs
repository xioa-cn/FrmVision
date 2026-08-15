using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Cognex.VisionPro;
using FrmServices.ViewModel;

namespace FrmViews.Controls
{
    public partial class CameraViewportControl : UserControl
    {
        private readonly Label _inspectionPositionCaptionLabel = new Label();
        private readonly Label _inspectionPositionLabel = new Label();
        private readonly TableLayoutPanel _recipeLayout = new TableLayoutPanel();
        private CameraPanelViewModel _viewModel;
        private bool _displayReady;

        public CameraViewportControl()
        {
            InitializeComponent();
            InitializeInspectionPositionDisplay();
            Disposed += (sender, args) => DetachViewModel();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            displayHost.BackColor = UiTheme.CameraCanvas;
            cogRecordDisplay.BackColor = UiTheme.CameraCanvas;
            cogRecordDisplay.AutoFit = true;
            cogRecordDisplay.AutoFitWithGraphics = true;
            cogRecordDisplay.HorizontalScrollBar = true;
            cogRecordDisplay.VerticalScrollBar = true;
            _displayReady = true;
            AttachRecordDisplay();
        }

        [Browsable(false)]
        public Panel DisplayHost => displayHost;

        [Browsable(false)]
        public CogRecordDisplay RecordDisplay => cogRecordDisplay;

        public void Bind(CameraPanelViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            DetachViewModel();
            _viewModel = viewModel;
            AttachRecordDisplay();

            nameLabel.DataBindings.Clear();
            productLabel.DataBindings.Clear();
            _inspectionPositionLabel.DataBindings.Clear();
            resultLabel.DataBindings.Clear();
            connectionLabel.DataBindings.Clear();
            nameLabel.DataBindings.Add(nameof(Label.Text), viewModel, nameof(viewModel.Name));
            productLabel.DataBindings.Add(nameof(Label.Text), viewModel, nameof(viewModel.ParameterName));
            _inspectionPositionLabel.DataBindings.Add(
                nameof(Label.Text), viewModel, nameof(viewModel.ProductName));
            resultLabel.DataBindings.Add(nameof(Label.Text), viewModel, nameof(viewModel.ResultText));
            connectionLabel.DataBindings.Add(nameof(Label.Text), viewModel, nameof(viewModel.ConnectionText));
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            UpdateRecipeToolTip();
            UpdateConnectionStyle();
        }

        private void InitializeInspectionPositionDisplay()
        {
            rootLayout.SuspendLayout();
            headerLayout.SuspendLayout();
            try
            {
                rootLayout.RowCount = 3;
                rootLayout.RowStyles.Clear();
                rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
                rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));

                headerLayout.RowCount = 1;
                headerLayout.RowStyles.Clear();
                headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                headerLayout.SetCellPosition(nameLabel, new TableLayoutPanelCellPosition(0, 0));
                headerLayout.SetCellPosition(resultLabel, new TableLayoutPanelCellPosition(3, 0));
                headerLayout.SetCellPosition(statusDot, new TableLayoutPanelCellPosition(4, 0));
                headerLayout.SetCellPosition(connectionLabel, new TableLayoutPanelCellPosition(5, 0));

                headerLayout.ColumnCount = 6;
                headerLayout.ColumnStyles.Clear();
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 72F));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 66F));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58F));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 16F));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));

                nameLabel.Margin = Padding.Empty;

                _inspectionPositionCaptionLabel.Dock = DockStyle.Fill;
                _inspectionPositionCaptionLabel.ForeColor = UiTheme.Muted;
                _inspectionPositionCaptionLabel.Margin = Padding.Empty;
                _inspectionPositionCaptionLabel.Text = "检测位置";
                _inspectionPositionCaptionLabel.TextAlign = ContentAlignment.MiddleLeft;

                _inspectionPositionLabel.AutoEllipsis = true;
                _inspectionPositionLabel.Dock = DockStyle.Fill;
                _inspectionPositionLabel.ForeColor = UiTheme.Text;
                _inspectionPositionLabel.Margin = new Padding(0, 0, 8, 0);
                _inspectionPositionLabel.TextAlign = ContentAlignment.MiddleLeft;

                headerLayout.Controls.Add(_inspectionPositionCaptionLabel, 1, 0);
                headerLayout.Controls.Add(_inspectionPositionLabel, 2, 0);

                headerLayout.Controls.Remove(productCaptionLabel);
                headerLayout.Controls.Remove(productLabel);

                _recipeLayout.BackColor = UiTheme.Surface;
                _recipeLayout.ColumnCount = 3;
                _recipeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44F));
                _recipeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
                _recipeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                _recipeLayout.Dock = DockStyle.Fill;
                _recipeLayout.Margin = Padding.Empty;
                _recipeLayout.Padding = new Padding(12, 0, 10, 0);
                _recipeLayout.RowCount = 1;
                _recipeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                productCaptionLabel.Margin = Padding.Empty;
                productLabel.Margin = new Padding(0, 5, 8, 5);
                _recipeLayout.Controls.Add(productCaptionLabel, 0, 0);
                _recipeLayout.Controls.Add(productLabel, 1, 0);
                rootLayout.Controls.Add(_recipeLayout, 0, 2);
            }
            finally
            {
                headerLayout.ResumeLayout(true);
                rootLayout.ResumeLayout(true);
            }
        }

        private void AttachRecordDisplay()
        {
            if (_displayReady && _viewModel != null)
                _viewModel.CogRecordDisplay = cogRecordDisplay;
        }

        private void DetachViewModel()
        {
            if (_viewModel == null) return;

            _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            if (ReferenceEquals(_viewModel.CogRecordDisplay, cogRecordDisplay))
                _viewModel.CogRecordDisplay = null;
            _viewModel = null;
        }

        public void SetDisplayControl(Control control)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            displayHost.Controls.Clear();
            control.Dock = DockStyle.Fill;
            displayHost.Controls.Add(control);
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CameraPanelViewModel.ParameterName))
                UpdateRecipeToolTip();

            if (e.PropertyName == nameof(CameraPanelViewModel.IsConnected) ||
                e.PropertyName == nameof(CameraPanelViewModel.ConnectionText))
                UpdateConnectionStyle();
        }

        private void UpdateRecipeToolTip()
        {
            recipeToolTip.SetToolTip(productLabel, _viewModel?.ParameterName ?? string.Empty);
        }

        private void UpdateConnectionStyle()
        {
            var connected = _viewModel != null && _viewModel.IsConnected;
            connectionLabel.ForeColor = connected ? UiTheme.Success : UiTheme.Danger;
            statusDot.BackColor = connectionLabel.ForeColor;
        }
    }
}
