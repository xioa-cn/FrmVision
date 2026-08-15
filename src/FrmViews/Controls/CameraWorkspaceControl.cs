using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;
using FrmServices.ViewModel;

namespace FrmViews.Controls
{
    public partial class CameraWorkspaceControl : UserControl
    {
        private readonly List<CameraViewportControl> _cameraControls =
            new List<CameraViewportControl>();
        private ObservableCollection<CameraPanelViewModel> _cameras;

        public CameraWorkspaceControl()
        {
            InitializeComponent();
            cameraLayout.SizeChanged += (sender, args) => UpdateGridLayout();
            Disposed += (sender, args) => DetachCameras();
        }

        public void Bind(ObservableCollection<CameraPanelViewModel> cameras)
        {
            if (cameras == null) throw new ArgumentNullException(nameof(cameras));
            if (ReferenceEquals(_cameras, cameras)) return;

            DetachCameras();
            _cameras = cameras;
            _cameras.CollectionChanged += CamerasOnCollectionChanged;
            RenderCameras();
        }

        public CameraViewportControl GetCamera(int index)
        {
            if (index < 1 || index > _cameraControls.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _cameraControls[index - 1];
        }

        private void DetachCameras()
        {
            if (_cameras != null)
                _cameras.CollectionChanged -= CamerasOnCollectionChanged;
            _cameras = null;
        }

        private void CamerasOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(RenderCameras));
                return;
            }

            RenderCameras();
        }

        private void RenderCameras()
        {
            if (IsDisposed || Disposing) return;

            cameraLayout.SuspendLayout();
            try
            {
                while (cameraLayout.Controls.Count > 0)
                {
                    var oldControl = cameraLayout.Controls[0];
                    cameraLayout.Controls.RemoveAt(0);
                    oldControl.Dispose();
                }

                _cameraControls.Clear();
                if (_cameras != null)
                {
                    foreach (var camera in _cameras)
                    {
                        var viewport = new CameraViewportControl
                        {
                            Dock = DockStyle.Fill,
                            MinimumSize = new Size(180, 140)
                        };
                        cameraLayout.Controls.Add(viewport);
                        viewport.Bind(camera);
                        _cameraControls.Add(viewport);
                    }
                }
            }
            finally
            {
                cameraLayout.ResumeLayout(true);
            }

            UpdateGridLayout();
            cameraLayout.PerformLayout();
        }

        private void UpdateGridLayout()
        {
            var cameraCount = _cameraControls.Count;
            var columnCount = CalculateColumnCount(cameraCount);
            var rowCount = cameraCount == 0
                ? 1
                : (int)Math.Ceiling(cameraCount / (double)columnCount);

            cameraLayout.SuspendLayout();
            try
            {
                cameraLayout.ColumnStyles.Clear();
                cameraLayout.RowStyles.Clear();
                cameraLayout.ColumnCount = columnCount;
                cameraLayout.RowCount = rowCount;

                for (var column = 0; column < columnCount; column++)
                    cameraLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / columnCount));
                for (var row = 0; row < rowCount; row++)
                    cameraLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rowCount));

                for (var index = 0; index < cameraCount; index++)
                    cameraLayout.SetCellPosition(
                        _cameraControls[index],
                        new TableLayoutPanelCellPosition(index % columnCount, index / columnCount));
            }
            finally
            {
                cameraLayout.ResumeLayout(true);
            }
        }

        private int CalculateColumnCount(int cameraCount)
        {
            if (cameraCount <= 1) return 1;

            var availableWidth = Math.Max(1, cameraLayout.ClientSize.Width - cameraLayout.Padding.Horizontal);
            var widthLimitedColumns = Math.Max(1, availableWidth / 260);
            var balancedColumns = (int)Math.Ceiling(Math.Sqrt(cameraCount));
            return Math.Max(1, Math.Min(cameraCount, Math.Min(widthLimitedColumns, balancedColumns)));
        }
    }
}
