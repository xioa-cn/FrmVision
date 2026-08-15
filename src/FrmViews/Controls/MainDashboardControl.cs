using System;
using System.Windows.Forms;
using FrmServices.ViewModel;

namespace FrmViews.Controls
{
    public partial class MainDashboardControl : UserControl
    {
        private bool _verticalLayout;

        public MainDashboardControl()
        {
            InitializeComponent();
            SizeChanged += MainDashboardControlOnSizeChanged;
        }

        public CameraWorkspaceControl CameraWorkspace => cameraWorkspace;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateResponsiveLayout();
        }

        public void Bind(MainFrmViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            cameraWorkspace.Bind(viewModel.Cameras);
        }

        public void ShowLogPage(int pageIndex)
        {
            switch (pageIndex)
            {
                case 0:
                    logControl.ShowLiveLogs();
                    break;
                case 1:
                    logControl.ShowHistoryLogs();
                    break;
                case 2:
                    logControl.ShowStorageSettings();
                    break;
            }
        }

        private void MainDashboardControlOnSizeChanged(object sender, EventArgs e)
        {
            UpdateResponsiveLayout();
        }

        private void UpdateResponsiveLayout()
        {
            if (Width <= 0 || Height <= 0) return;
            var useVertical = Width >= 750;
            if (_verticalLayout != useVertical)
            {
                splitContainer.Panel1MinSize = 0;
                splitContainer.Panel2MinSize = 0;
                splitContainer.Orientation = useVertical
                    ? Orientation.Vertical
                    : Orientation.Horizontal;
                _verticalLayout = useVertical;
            }

            var span = useVertical ? splitContainer.ClientSize.Width : splitContainer.ClientSize.Height;
            if (span < 40) return;
            var desired = useVertical ? (int)(span * 0.64F) : (int)(span * 0.50F);
            splitContainer.SplitterDistance = Math.Max(20,
                Math.Min(span - splitContainer.SplitterWidth - 20, desired));
        }
    }
}
