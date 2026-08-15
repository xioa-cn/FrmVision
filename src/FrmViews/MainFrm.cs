using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrmCommon;
using FrmCommon.FrmExtensions;
using FrmCommon.Mvvm;
using FrmServices.LogServices;
using FrmServices.Services.EditorServices;
using FrmServices.Services.UserManagement;
using FrmServices.ViewModel;
using FrmViews.Services;
using FrmViews.Views;

namespace FrmViews
{
    public partial class MainFrm : ViewModelFrm, IViewModelFrm<MainFrmViewModel>
    {
        private const int WorkflowShutdownTimeoutMilliseconds = 3000;
        private const int CommunicationShutdownTimeoutMilliseconds = 1500;
        private const int LogFlushTimeoutMilliseconds = 400;
        private bool _isBound;
        private bool _allowClose;
        private bool _closePending;
        private bool _communicationDisposalStarted;
        private WorkflowRuntime _workflowRuntime;
        private CancellationTokenSource _workflowCancellation;
        private Task _workflowExecutionTask;
        private Task _communicationDisposalTask;
        private readonly SemaphoreSlim _workflowControlLock = new SemaphoreSlim(1, 1);
        private readonly object _workflowUiRoot = new object();
        private int _workflowNodeTransitionDelayMilliseconds;
        private int _workflowUiUpdateScheduled;
        private int _pendingSuccessfulWorkflowCycles;
        private string _pendingWorkflowStatus;
        private EditorFrm _editorFrm;

        public MainFrm() : base(false, true)
        {
            DataContext = new MainFrmViewModel();
            InitializeComponent();
            ViewModel.SetUiDispatcher(DispatchViewModelUpdate);
            FormClosing += MainFrmOnFormClosing;
            Disposed += MainFrmOnDisposed;
            navigationControl.WorkflowPauseRequested += NavigationOnWorkflowPauseRequested;
            navigationControl.WorkflowRestartRequested += NavigationOnWorkflowRestartRequested;
            navigationControl.LogoutRequested += NavigationOnLogoutRequested;
            navigationControl.OpenConfigDirectoryRequested +=
                NavigationOnOpenConfigDirectoryRequested;
            UserSession.CurrentUserChanged += UserSessionOnCurrentUserChanged;
            AppLog.Info("打开了软件");
        }

        private void DispatchViewModelUpdate(Action action)
        {
            if (action == null || IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(action);
                }
                catch (InvalidOperationException)
                {
                }

                return;
            }

            action();
        }

        public object DataContext { get; set; }
        public MainFrmViewModel ViewModel => (MainFrmViewModel)DataContext;

        public override void FrmBinding()
        {
            if (_isBound) return;
            _isBound = true;

            this.SetBinding()
                .BindingProperty(workspaceTabs, tab => tab.SelectedIndex,
                    vm => vm.SelectedPageIndex);

            InitializeWorkflowRuntime();


            SetSystemCommand();
            navigationControl.BindMenuItems(ViewModel.NavigationMenus);
            dashboardControl.Bind(ViewModel);
            parameterControl.Bind(ViewModel);
            statusBarControl.Bind(ViewModel);
            ApplyCurrentUser();
            navigationControl.SetActivePage(ViewModel.SelectedPageIndex);
            _ = LoadToolsAsync();
            _ = TryAutoLoginAsync();
            (ViewModel as ObservableObject).PropertyChanged += ViewModelOnPropertyChanged;
            Disposed += (sender, args) => (ViewModel as ObservableObject).PropertyChanged -= ViewModelOnPropertyChanged;
            base.FrmBinding();
        }

        private void SetSystemCommand()
        {
            var findCommunication = ViewModel.NavigationMenus
                .FirstOrDefault(e => e.Key == MainMenuKeys.Communication);

            if (findCommunication != null)
            {
                findCommunication.Command = new RelayCommand(() =>
                {
                    if (!EnsureEngineerPermission("通讯配置")) return;
                    using (var comm = new CommunicationFrm(
                               ViewModel.CommunicationFrmViewModel))
                        comm.ShowDialog(this);
                });
            }

            var findEditor = ViewModel.NavigationMenus.FirstOrDefault(e => e.Key == MainMenuKeys.Editor);

            if (findEditor != null)
            {
                findEditor.Command = new RelayCommand(ShowWorkflowEditor);
            }

            var findCameraConfig = ViewModel.NavigationMenus
                .SelectMany(item => item.Children)
                .FirstOrDefault(e => e.Key == MainMenuKeys.CameraConfig);

            if (findCameraConfig != null)
            {
                findCameraConfig.Command = new RelayCommand(() =>
                {
                    if (!EnsureEngineerPermission("图像窗口配置")) return;
                    using (var cameraConfig = new CameraConfig(ViewModel))
                        cameraConfig.ShowDialog(this);
                });
            }

            var findParameterConfig = ViewModel.NavigationMenus
                .SelectMany(item => item.Children)
                .FirstOrDefault(e => e.Key == MainMenuKeys.ParametersConfigDir);

            if (findParameterConfig != null)
            {
                findParameterConfig.Command = new RelayCommand(ShowParametersConfig);
            }

            BindMenuCommand(MainMenuKeys.HslCommunication,
                ShowHslAuthorization);

            BindMenuCommand(MainMenuKeys.Login, ShowUserLogin);
            BindMenuCommand(MainMenuKeys.Register, ShowUserRegister);
            BindMenuCommand(MainMenuKeys.UserManager, ShowUserManager);
            BindMenuCommand(MainMenuKeys.Parameters, ShowRecipeApplication);
            BindMenuCommand(MainMenuKeys.StorageSettings, ShowStorageSettings);
        }

        private void BindMenuCommand(string menuKey, Action action)
        {
            NavigationMenuItemViewModel menu = FindMenuItem(
                ViewModel.NavigationMenus, menuKey);
            if (menu != null) menu.Command = new RelayCommand(action);
        }

        private void ShowHslAuthorization()
        {
            if (!EnsureEngineerPermission("通讯秘钥配置")) return;
            using (var form = new HslAuthorizationFrm())
                form.ShowDialog(this);
        }

        private static NavigationMenuItemViewModel FindMenuItem(
            IEnumerable<NavigationMenuItemViewModel> items, string key)
        {
            foreach (NavigationMenuItemViewModel item in items)
            {
                if (string.Equals(item.Key, key, StringComparison.Ordinal))
                    return item;
                NavigationMenuItemViewModel child = FindMenuItem(item.Children, key);
                if (child != null) return child;
            }
            return null;
        }

        private void ShowUserLogin()
        {
            using (var form = new Login(
                       new LoginViewModel(UserService.Default)))
            {
                if (form.ShowDialog(this) != DialogResult.OK) return;
            }

            ApplyCurrentUser();
            ViewModel.StatusMessage = "用户登录成功";
        }

        private async Task TryAutoLoginAsync()
        {
            var loginViewModel = new LoginViewModel(UserService.Default);
            if (!loginViewModel.AutoLogin) return;

            bool succeeded = await loginViewModel.TryAutoLoginAsync();
            if (IsDisposed || Disposing) return;

            ApplyCurrentUser();
            ViewModel.StatusMessage = succeeded
                ? "用户已自动登录"
                : "自动登录失败：" + loginViewModel.StatusMessage;
        }

        private void ShowUserRegister()
        {
            UserInfo registeredUser;
            using (var form = new Register(
                       new RegisterViewModel(UserService.Default)))
            {
                if (form.ShowDialog(this) != DialogResult.OK) return;
                registeredUser = form.RegisteredUser;
            }

            if (UserSession.CurrentUser == null && registeredUser != null &&
                string.Equals(registeredUser.Role, UserRoles.Administrator,
                    StringComparison.Ordinal))
                UserSession.SignIn(registeredUser);
            ApplyCurrentUser();
            ViewModel.StatusMessage = "用户注册成功";
        }

        private void ShowUserManager()
        {
            if (!UserSession.IsAdministrator)
            {
                MessageBox.Show(this, "请先使用管理员账户登录。", "用户管理",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new UserManager(
                       new UserManagerViewModel(UserService.Default)))
                form.ShowDialog(this);
            ApplyCurrentUser();
        }

        private void ApplyCurrentUser()
        {
            UserInfo user = UserSession.CurrentUser;
            ViewModel.CurrentUser = user == null
                ? "未登录"
                : user.DisplayName + " / " + user.Role;
            ApplyAccessPermissions();
        }

        private void ApplyAccessPermissions()
        {
            bool canOperate = UserSession.CanApplyRecipe;
            bool canConfigure = UserSession.CanConfigure;
            bool isLoggedIn = UserSession.CurrentUser != null;

            ViewModel.ApplyUserPermissions();
            navigationControl.ApplyPermissions(isLoggedIn, canOperate,
                canConfigure);
            parameterControl.SetCanApplyRecipe(canOperate);
        }

        private void NavigationOnLogoutRequested(object sender, EventArgs e)
        {
            UserInfo currentUser = UserSession.CurrentUser;
            if (currentUser == null) return;
            if (MessageBox.Show(this,
                    "确定退出当前账户“" + currentUser.DisplayName + "”吗？",
                    "退出登录", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            if (_editorFrm != null && !_editorFrm.IsDisposed)
                _editorFrm.Close();

            try
            {
                LoginPreferences preferences =
                    LoginPreferenceService.Default.Load();
                LoginPreferenceService.Default.Save(preferences.UserName,
                    preferences.Password, preferences.RememberPassword, false);
            }
            catch (Exception ex)
            {
                AppLog.Warning("退出登录时关闭自动登录失败：" +
                               ex.GetBaseException().Message,
                    nameof(MainFrm));
            }

            UserSession.SignOut();
            ViewModel.StatusMessage = "已退出登录";
        }

        private void UserSessionOnCurrentUserChanged(object sender,
            EventArgs e)
        {
            DispatchViewModelUpdate(ApplyCurrentUser);
        }

        private void NavigationOnOpenConfigDirectoryRequested(object sender,
            EventArgs e)
        {
            if (!EnsureEngineerPermission("打开配置目录")) return;

            string directory = GlobalConfig.Instance?.ConfigCommonDir;
            if (string.IsNullOrWhiteSpace(directory))
            {
                MessageBox.Show(this, "尚未配置视觉文件根目录，请先进行参数路径设置。",
                    "打开配置目录", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!Directory.Exists(directory))
            {
                MessageBox.Show(this, "配置目录不存在：" + directory,
                    "打开配置目录", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = directory,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "打开配置目录失败：" + ex.GetBaseException().Message,
                    "打开配置目录", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool EnsureEngineerPermission(string operation)
        {
            if (UserSession.CanConfigure) return true;
            MessageBox.Show(this,
                operation + "需要工程师或管理员权限。", "权限不足",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private bool EnsureEmployeePermission(string operation)
        {
            if (UserSession.CanApplyRecipe) return true;
            MessageBox.Show(this,
                operation + "需要员工、工程师或管理员权限。", "权限不足",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private void ShowRecipeApplication()
        {
            if (!EnsureEmployeePermission("配方应用")) return;
            ViewModel.SelectedPageIndex = 1;
        }

        private void ShowStorageSettings()
        {
            if (!EnsureEngineerPermission("存储设置")) return;
            ViewModel.SelectedPageIndex = 0;
            ViewModel.SelectedLogPage = 2;
        }

        private async void ShowParametersConfig()
        {
            if (!EnsureEngineerPermission("参数路径设置")) return;
            using (var parametersConfigFrm = new ParametersConfigFrm())
            {
                if (parametersConfigFrm.ShowDialog(this) != DialogResult.OK)
                    return;
            }

            await _workflowControlLock.WaitAsync();
            try
            {
                bool restartWorkflow = _workflowExecutionTask != null &&
                                       !_workflowExecutionTask.IsCompleted &&
                                       _workflowCancellation != null &&
                                       !_workflowCancellation.IsCancellationRequested;

                if (restartWorkflow)
                {
                    _workflowCancellation.Cancel();
                    await _workflowExecutionTask;
                }

                ViewModel.StatusMessage = "正在加载新的参数路径";
                await Task.Run(ViewModel.LoadTools);
                if (IsDisposed || Disposing) return;

                ViewModel.RefreshToolMenuStates();
                if (restartWorkflow && _workflowRuntime != null &&
                    _workflowRuntime.IsLoaded)
                    StartWorkflow("参数路径更新后任务流已重启");
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = "参数路径更新失败";
                AppLog.Error("参数路径更新失败" + Environment.NewLine + ex,
                    nameof(MainFrm));
            }
            finally
            {
                _workflowControlLock.Release();
            }
        }

        private void ShowWorkflowEditor()
        {
            if (!EnsureEngineerPermission("流程编辑")) return;
            if (_editorFrm != null && !_editorFrm.IsDisposed)
            {
                if (_editorFrm.WindowState == FormWindowState.Minimized)
                    _editorFrm.WindowState = FormWindowState.Normal;
                _editorFrm.Show();
                _editorFrm.BringToFront();
                _editorFrm.Activate();
                return;
            }

            Volatile.Write(ref _workflowNodeTransitionDelayMilliseconds, 1000);
            AppLog.Info("打开流程编辑界面，节点切换和流程轮次间隔调整为 1 秒。", nameof(MainFrm));

            _editorFrm = new EditorFrm(_workflowRuntime,
                ViewModel.CommunicationFrmViewModel.ResolveLightSource)
            {
                DataContext = ViewModel.EditorViewModel
            };
            _editorFrm.WorkflowSaved += EditorFrmOnWorkflowSaved;
            _editorFrm.FormClosed += EditorFrmOnFormClosed;
            _editorFrm.Show(this);
        }

        private void EditorFrmOnFormClosed(object sender, FormClosedEventArgs e)
        {
            var editor = sender as EditorFrm;
            if (editor != null)
            {
                editor.WorkflowSaved -= EditorFrmOnWorkflowSaved;
                editor.FormClosed -= EditorFrmOnFormClosed;
            }

            if (ReferenceEquals(_editorFrm, editor))
                _editorFrm = null;

            Volatile.Write(ref _workflowNodeTransitionDelayMilliseconds, 0);
            AppLog.Info("关闭流程编辑界面，节点切换和流程轮次间隔恢复正常。", nameof(MainFrm));
        }

        private async Task LoadToolsAsync()
        {
            try
            {
                await Task.Run(ViewModel.LoadTools);
                if (IsDisposed || Disposing) return;

                ViewModel.RefreshToolMenuStates();
                await Task.Run(() => ViewModel.CommunicationFrmViewModel
                    .BuildRuntimeCommunications());
                if (IsDisposed || Disposing)
                {
                    ViewModel.CommunicationFrmViewModel.DisposeRuntimeCommunications();
                    return;
                }

                if (_workflowRuntime == null || !_workflowRuntime.IsLoaded)
                    return;

                StartWorkflow("任务流启动");
                await _workflowExecutionTask;
            }
            catch (Exception ex)
            {
                AppLog.Error("工具加载任务失败" + Environment.NewLine + ex, nameof(MainFrm));
            }
        }

        private void InitializeWorkflowRuntime()
        {
            try
            {
                _workflowRuntime = new WorkflowRuntime(ViewModel.EditorViewModel);
                TryLoadWorkflow();
            }
            catch (Exception ex)
            {
                AppLog.Error("流程运行时初始化失败" + Environment.NewLine + ex,
                    nameof(MainFrm));
            }
        }

        private bool TryLoadWorkflow()
        {
            if (_workflowRuntime == null) return false;
            try
            {
                bool loaded = _workflowRuntime.Load();
                if (!loaded)
                    AppLog.Info("未找到已保存的流程，跳过自动执行。", nameof(MainFrm));
                return loaded;
            }
            catch (Exception ex)
            {
                AppLog.Error("流程加载失败：" + ViewModel.EditorViewModel.EditorDataFilePath +
                             Environment.NewLine + ex, nameof(MainFrm));
                return false;
            }
        }

        private async Task RunWorkflowAsync(CancellationToken cancellationToken)
        {
            const int successDelayMilliseconds = 50;
            const int errorDelayMilliseconds = 5000;
            var communication = ViewModel.CommunicationFrmViewModel;
            var context = new EditorExecutionContext(cancellationToken)
            {
                PlcResolver = communication.ResolvePlc,
                LightSourceResolver = communication.ResolveLightSource,
                NodeTransitionDelayMillisecondsProvider = () =>
                    Volatile.Read(ref _workflowNodeTransitionDelayMilliseconds),
                SuccessfulCycleDelayMillisecondsProvider = () =>
                    Volatile.Read(ref _workflowNodeTransitionDelayMilliseconds) > 0
                        ? 1000
                        : successDelayMilliseconds,
                FailedCycleDelayMillisecondsProvider = () =>
                    errorDelayMilliseconds,
                RecipeChanged = UpdateCameraRecipe,
                FlowCycleCompleted = HandleWorkflowCycleCompleted
            };

            EditorExecutionResult terminalResult =
                await _workflowRuntime.ExecuteAsync(context);
            if (!terminalResult.IsCanceled && !terminalResult.IsSuccess)
            {
                ViewModel.StatusMessage = "流程执行失败";
                AppLog.Error(terminalResult.Message, nameof(MainFrm));
            }
            else if (cancellationToken.IsCancellationRequested)
            {
                ViewModel.StatusMessage = "流程执行已取消";
            }
        }

        private void HandleWorkflowCycleCompleted(EditorExecutionResult result)
        {
            if (result == null || Volatile.Read(ref _closePending)) return;
            foreach (EditorExecutionStep step in result.Steps)
            {
                if (!step.EnableExecutionLog) continue;

                string state = step.IsSuccess ? "成功" : "失败";
                AppLog.Info("流程节点[" + state + "] " + step.NodeTitle + "：" +
                            step.Message + "，耗时 " +
                            step.Elapsed.TotalMilliseconds.ToString("F0") +
                            " ms", nameof(MainFrm));
            }

            lock (_workflowUiRoot)
            {
                if (result.IsSuccess)
                {
                    _pendingSuccessfulWorkflowCycles++;
                    _pendingWorkflowStatus = result.Message;
                }
                else if (!result.IsCanceled)
                {
                    _pendingWorkflowStatus = "流程执行失败";
                }
            }

            if (!result.IsSuccess && !result.IsCanceled)
                AppLog.Error(result.Message, nameof(MainFrm));
            ScheduleWorkflowUiUpdate();
        }

        private void ScheduleWorkflowUiUpdate()
        {
            if (Volatile.Read(ref _closePending) || IsDisposed || Disposing ||
                !IsHandleCreated)
                return;
            if (Interlocked.CompareExchange(ref _workflowUiUpdateScheduled, 1, 0) != 0)
                return;

            try
            {
                BeginInvoke(new MethodInvoker(ApplyPendingWorkflowUiUpdate));
            }
            catch (ObjectDisposedException)
            {
                Interlocked.Exchange(ref _workflowUiUpdateScheduled, 0);
            }
            catch (InvalidOperationException)
            {
                Interlocked.Exchange(ref _workflowUiUpdateScheduled, 0);
            }
        }

        private void ApplyPendingWorkflowUiUpdate()
        {
            int successfulCycles;
            string status;
            lock (_workflowUiRoot)
            {
                successfulCycles = _pendingSuccessfulWorkflowCycles;
                _pendingSuccessfulWorkflowCycles = 0;
                status = _pendingWorkflowStatus;
                _pendingWorkflowStatus = null;
            }

            Interlocked.Exchange(ref _workflowUiUpdateScheduled, 0);
            if (Volatile.Read(ref _closePending) || IsDisposed || Disposing)
                return;

            if (successfulCycles > 0)
                ViewModel.CycleCount += successfulCycles;
            if (!string.IsNullOrWhiteSpace(status))
                ViewModel.StatusMessage = status;

            lock (_workflowUiRoot)
            {
                if (_pendingSuccessfulWorkflowCycles == 0 &&
                    _pendingWorkflowStatus == null)
                    return;
            }

            ScheduleWorkflowUiUpdate();
        }

        private void UpdateCameraRecipe(string productionKey, string recipeName)
        {
            if (IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                if (!IsHandleCreated) return;
                BeginInvoke(new Action<string, string>(UpdateCameraRecipe),
                    productionKey, recipeName);
                return;
            }

            string normalizedKey = (productionKey ?? string.Empty).Trim();
            string normalizedRecipe = (recipeName ?? string.Empty).Trim();
            CameraPanelViewModel[] cameras = ViewModel.Cameras.Where(item =>
                string.Equals(item.ProductName, normalizedKey,
                    StringComparison.OrdinalIgnoreCase)).ToArray();
            if (cameras.Length == 0)
            {
                AppLog.Error("配方界面更新失败：未找到产品 Key 为 “" + normalizedKey +
                             "” 的相机。", nameof(MainFrm));
                return;
            }

            CameraPanelViewModel[] changedCameras = cameras.Where(camera =>
                !string.Equals(camera.ParameterName, normalizedRecipe,
                    StringComparison.Ordinal)).ToArray();
            ViewModel.SetRecipeLoaded(normalizedKey, true);
            if (changedCameras.Length == 0) return;

            foreach (CameraPanelViewModel camera in changedCameras)
                camera.ParameterName = normalizedRecipe;

            ViewModel.SaveCameraParameters();
            ViewModel.RefreshToolMenuStates();
            AppLog.Info("相机界面配方已更新：" +
                        string.Join("、", changedCameras.Select(camera => camera.Name)) +
                        "，产品 Key “" +
                        normalizedKey + "”，配方 “" + normalizedRecipe + "”。",
                nameof(MainFrm));
        }

        private async void NavigationOnWorkflowPauseRequested(object sender, EventArgs e)
        {
            if (!EnsureEmployeePermission("暂停任务流")) return;
            await _workflowControlLock.WaitAsync();
            try
            {
                if (_workflowCancellation == null || _workflowExecutionTask == null ||
                    _workflowExecutionTask.IsCompleted)
                {
                    AppLog.Info("暂停任务流：当前没有正在运行的任务流。", nameof(MainFrm));
                    return;
                }

                AppLog.Info("收到暂停任务流操作。", nameof(MainFrm));
                _workflowCancellation.Cancel();
                try
                {
                    await _workflowExecutionTask;
                    ViewModel.StatusMessage = "流程已暂停";
                    AppLog.Info("任务流已暂停。", nameof(MainFrm));
                }
                catch (Exception ex)
                {
                    AppLog.Error("暂停任务流失败" + Environment.NewLine + ex, nameof(MainFrm));
                }
            }
            finally
            {
                _workflowControlLock.Release();
            }
        }

        private async void NavigationOnWorkflowRestartRequested(object sender, EventArgs e)
        {
            if (!EnsureEmployeePermission("重启任务流")) return;
            await _workflowControlLock.WaitAsync();
            try
            {
                if (_workflowRuntime == null || !_workflowRuntime.IsLoaded)
                {
                    AppLog.Error("重启任务流失败：流程尚未加载。", nameof(MainFrm));
                    return;
                }

                if (_workflowExecutionTask != null && !_workflowExecutionTask.IsCompleted)
                {
                    if (_workflowCancellation != null && _workflowCancellation.IsCancellationRequested)
                    {
                        AppLog.Info("重启任务流：等待暂停中的任务流退出。", nameof(MainFrm));
                        try
                        {
                            await _workflowExecutionTask;
                        }
                        catch (Exception ex)
                        {
                            AppLog.Error("重启任务流等待旧任务失败" + Environment.NewLine + ex,
                                nameof(MainFrm));
                            return;
                        }
                    }
                    else
                    {
                        AppLog.Info("重启任务流已忽略：任务流仍在运行。", nameof(MainFrm));
                        return;
                    }
                }

                if (IsDisposed || Disposing)
                {
                    AppLog.Info("重启任务流已忽略：窗体正在关闭。", nameof(MainFrm));
                    return;
                }

                StartWorkflow("任务流重启");
                ViewModel.StatusMessage = "流程已重启";
            }
            finally
            {
                _workflowControlLock.Release();
            }
        }

        private async void EditorFrmOnWorkflowSaved(object sender, EventArgs e)
        {
            await _workflowControlLock.WaitAsync();
            try
            {
                bool restartAfterLoad = _workflowExecutionTask != null &&
                                        !_workflowExecutionTask.IsCompleted &&
                                        _workflowCancellation != null &&
                                        !_workflowCancellation.IsCancellationRequested;

                AppLog.Info("流程已保存，开始更新正在使用的任务流。", nameof(MainFrm));
                if (_workflowExecutionTask != null && !_workflowExecutionTask.IsCompleted)
                {
                    _workflowCancellation?.Cancel();
                    await _workflowExecutionTask;
                }

                if (!TryLoadWorkflow())
                {
                    ViewModel.StatusMessage = "流程更新失败";
                    AppLog.Error("任务流更新失败：无法加载已保存的流程。", nameof(MainFrm));
                    return;
                }

                if (restartAfterLoad && !IsDisposed && !Disposing)
                {
                    StartWorkflow("保存后任务流已更新并重启");
                    ViewModel.StatusMessage = "流程已更新";
                }
                else
                {
                    ViewModel.StatusMessage = "已更新暂停中的流程";
                    AppLog.Info("任务流已更新，保持暂停状态。", nameof(MainFrm));
                }
            }
            catch (Exception ex)
            {
                ViewModel.StatusMessage = "流程更新失败";
                AppLog.Error("保存后更新任务流失败" + Environment.NewLine + ex,
                    nameof(MainFrm));
            }
            finally
            {
                _workflowControlLock.Release();
            }
        }

        private void StartWorkflow(string operation)
        {
            _workflowCancellation?.Dispose();
            _workflowCancellation = new CancellationTokenSource();
            _workflowExecutionTask = RunWorkflowAsync(_workflowCancellation.Token);
            AppLog.Info(operation + "。", nameof(MainFrm));
        }

        private async void MainFrmOnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (_allowClose) return;
            e.Cancel = true;
            if (_closePending) return;

            _closePending = true;
            lock (_workflowUiRoot)
            {
                _pendingSuccessfulWorkflowCycles = 0;
                _pendingWorkflowStatus = null;
            }

            AppLog.Info("收到关闭软件操作，正在停止任务流。", nameof(MainFrm));

            if (_editorFrm != null && !_editorFrm.IsDisposed)
                _editorFrm.Close();

            _workflowCancellation?.Cancel();
            try
            {
                bool workflowStopped = await WaitForShutdownTaskAsync(
                    _workflowExecutionTask, WorkflowShutdownTimeoutMilliseconds);
                if (!workflowStopped)
                {
                    AppLog.Warning("关闭软件时任务流未在 3 秒内退出，" +
                                   "可能仍有硬件调用正在执行，将继续关闭软件。",
                        nameof(MainFrm));
                }
                else
                {
                    StartCommunicationDisposal();
                    bool communicationsDisposed = await WaitForShutdownTaskAsync(
                        _communicationDisposalTask,
                        CommunicationShutdownTimeoutMilliseconds);
                    if (!communicationsDisposed)
                        AppLog.Warning("关闭软件时通讯实例未在限定时间内释放，" +
                                       "将继续关闭软件。", nameof(MainFrm));
                }
            }
            catch (Exception ex)
            {
                AppLog.Error("关闭软件时停止后台任务失败" + Environment.NewLine + ex,
                    nameof(MainFrm));
            }
            finally
            {
                try
                {
                    await LogService.Default.FlushAsync(
                        TimeSpan.FromMilliseconds(LogFlushTimeoutMilliseconds));
                }
                catch
                {
                    // 日志存储异常不能阻止主程序退出。
                }
                _allowClose = true;
                if (!IsDisposed && !Disposing)
                    Close();
            }
        }

        private static async Task<bool> WaitForShutdownTaskAsync(Task task,
            int timeoutMilliseconds)
        {
            if (task == null) return true;
            if (!task.IsCompleted)
            {
                Task completed = await Task.WhenAny(task,
                    Task.Delay(timeoutMilliseconds));
                if (!ReferenceEquals(completed, task)) return false;
            }

            await task;
            return true;
        }

        private void StartCommunicationDisposal()
        {
            if (_communicationDisposalStarted) return;
            _communicationDisposalStarted = true;
            _communicationDisposalTask = Task.Run(() =>
                ViewModel.CommunicationFrmViewModel.DisposeRuntimeCommunications());
        }

        private void MainFrmOnDisposed(object sender, EventArgs e)
        {
            FormClosing -= MainFrmOnFormClosing;
            Disposed -= MainFrmOnDisposed;
            navigationControl.WorkflowPauseRequested -= NavigationOnWorkflowPauseRequested;
            navigationControl.WorkflowRestartRequested -= NavigationOnWorkflowRestartRequested;
            navigationControl.LogoutRequested -= NavigationOnLogoutRequested;
            navigationControl.OpenConfigDirectoryRequested -=
                NavigationOnOpenConfigDirectoryRequested;
            UserSession.CurrentUserChanged -= UserSessionOnCurrentUserChanged;
            if (_editorFrm != null && !_editorFrm.IsDisposed)
            {
                _editorFrm.WorkflowSaved -= EditorFrmOnWorkflowSaved;
                _editorFrm.FormClosed -= EditorFrmOnFormClosed;
                _editorFrm.Dispose();
                _editorFrm = null;
            }

            _workflowCancellation?.Dispose();
            if (_workflowRuntime != null && !_workflowRuntime.IsExecuting)
                _workflowRuntime.Dispose();
            if (!_communicationDisposalStarted &&
                (_workflowExecutionTask == null || _workflowExecutionTask.IsCompleted))
                StartCommunicationDisposal();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainFrmViewModel.SelectedLogPage))
                dashboardControl.ShowLogPage(ViewModel.SelectedLogPage);
            else if (e.PropertyName == nameof(MainFrmViewModel.SelectedPageIndex))
                navigationControl.SetActivePage(ViewModel.SelectedPageIndex);
        }

        private void NavigationOnHomeRequested(object sender, EventArgs e)
        {
            ViewModel.SelectedPageIndex = 0;
            ViewModel.SelectedLogPage = 0;
        }

        private void NavigationOnRecordsRequested(object sender, EventArgs e)
        {
            ViewModel.SelectedPageIndex = 0;
            ViewModel.SelectedLogPage = 1;
        }

        private void NavigationOnParametersRequested(object sender, EventArgs e)
        {
            ShowRecipeApplication();
        }
    }
}
