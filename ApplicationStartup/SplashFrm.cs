using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrmCommon;
using FrmCommon.ConfigUtils;
using FrmViews;

namespace ApplicationStartup
{
    public partial class SplashFrm : Form
    {
        private const int MinimumVisibleMilliseconds = 1200;
        private readonly Color _activeBlockColor = Color.FromArgb(37, 99, 235);
        private readonly Color _inactiveBlockColor = Color.FromArgb(191, 219, 254);
        private int _displayedProgress = 8;
        private int _targetProgress = 8;
        private int _animationFrame;
        private bool _startupStarted;
        private Form _mainForm;

        public SplashFrm()
        {
            InitializeComponent();
            versionLabel.Text = "版本 " + GetDisplayVersion();
            UpdateProgressVisuals();
        }

        private async void SplashFrmOnShown(object sender, EventArgs e)
        {
            if (_startupStarted) return;
            _startupStarted = true;
            animationTimer.Start();

            var visibleTime = Stopwatch.StartNew();

            try
            {
                SetStartupStage(16, "正在读取系统配置");
                await Task.Run((Action)GlobalConfig.Initialize);

                SetStartupStage(42,
                    string.IsNullOrWhiteSpace(GlobalConfig.LoadError)
                        ? "系统配置读取完成"
                        : "配置读取异常，已使用默认设置");

                SetStartupStage(58, "正在验证通讯组件授权");
                bool hslAuthorized = await Task.Run(
                    (Func<bool>)HslAuthExtensios.SetAuthTool);

                SetStartupStage(74,
                    hslAuthorized
                        ? "通讯组件授权成功"
                        : "通讯组件尚未授权，将以受限模式启动");

                await Task.Delay(120);
                SetStartupStage(86, "正在创建主工作区");
                _mainForm = new MainFrm();

                int remainingDelay = MinimumVisibleMilliseconds -
                                     (int)visibleTime.ElapsedMilliseconds;
                if (remainingDelay > 0)
                    await Task.Delay(remainingDelay);

                SetStartupStage(100, "启动完成");
                _displayedProgress = 100;
                UpdateProgressVisuals();
                await Task.Delay(2500);

                if (IsDisposed || Disposing) return;

                animationTimer.Stop();
                _mainForm.FormClosed += MainFormOnClosed;
                _mainForm.Show();
                Hide();
            }
            catch (Exception exception)
            {
                ShowStartupFailure(exception);
            }
        }

        private void AnimationTimerOnTick(object sender, EventArgs e)
        {
            if (_displayedProgress < _targetProgress)
            {
                int distance = _targetProgress - _displayedProgress;
                _displayedProgress += Math.Max(1, distance / 6);
                if (_displayedProgress > _targetProgress)
                    _displayedProgress = _targetProgress;
            }

            _animationFrame = (_animationFrame + 1) % 30;
            int activeBlock = _animationFrame / 10;
            loadingBlock1.BackColor = activeBlock == 0
                ? _activeBlockColor
                : _inactiveBlockColor;
            loadingBlock2.BackColor = activeBlock == 1
                ? _activeBlockColor
                : _inactiveBlockColor;
            loadingBlock3.BackColor = activeBlock == 2
                ? _activeBlockColor
                : _inactiveBlockColor;

            UpdateProgressVisuals();
        }

        private void SetStartupStage(int progress, string status)
        {
            _targetProgress = Math.Max(_targetProgress,
                Math.Min(100, progress));
            statusLabel.Text = status;
        }

        private void UpdateProgressVisuals()
        {
            int availableWidth = progressTrackPanel.ClientSize.Width;
            int fillWidth = availableWidth * _displayedProgress / 100;
            progressFillPanel.Width = Math.Max(1,
                Math.Min(availableWidth, fillWidth));
            percentageLabel.Text = _displayedProgress + "%";
        }

        private void MainFormOnClosed(object sender, FormClosedEventArgs e)
        {
            _mainForm.FormClosed -= MainFormOnClosed;
            _mainForm = null;
            Close();
        }

        private void ShowStartupFailure(Exception exception)
        {
            animationTimer.Stop();
            startupTitleLabel.Text = "启动失败";
            statusLabel.Text = exception.GetBaseException().Message;
            percentageLabel.ForeColor = Color.FromArgb(185, 28, 28);
            progressFillPanel.BackColor = Color.FromArgb(220, 38, 38);
            topStatusAccentPanel.BackColor = Color.FromArgb(220, 38, 38);

            MessageBox.Show(this,
                "FrmVision 启动失败：" + exception.GetBaseException().Message,
                "启动失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }

        private static string GetDisplayVersion()
        {
            Version version = typeof(SplashFrm).Assembly.GetName().Version;
            return version == null
                ? "1.0.0"
                : string.Format("{0}.{1}.{2}", version.Major,
                    version.Minor, version.Build);
        }
    }
}
