using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrmCommon;
using FrmCommon.ConfigUtils;
using FrmServices.LogServices;
using FrmServices.Services.UserManagement;
using FrmServices.Utils;
using FrmVpComponents.Services;

namespace FrmServices.ViewModel
{
    [System.Runtime.Serialization.DataContract]
    public sealed class CameraDisplayConfiguration
    {
        [System.Runtime.Serialization.DataMember]
        public int Index { get; set; }

        [System.Runtime.Serialization.DataMember]
        public string Name { get; set; }

        [System.Runtime.Serialization.DataMember]
        public string ProductName { get; set; }

        [System.Runtime.Serialization.DataMember]
        public string ParameterName { get; set; }
    }

    public sealed class MainFrmViewModel : ObservableObject
    {
        public EditorViewModel EditorViewModel { get; set; }
        public CommunicationFrmViewModel CommunicationFrmViewModel { get; set; }
        private int _selectedPageIndex;
        private int _selectedLogPage;
        private string _currentUser = "未登录";
        private string _operationMode = "联机模式";
        private string _statusMessage = "系统就绪";
        private int _cycleCount;
        private int _light1Brightness = 50;
        private int _light2Brightness = 50;
        private int _light3Brightness = 50;
        private readonly NavigationMenuItemViewModel _cameraToolsMenu;
        private readonly NavigationMenuItemViewModel _visionToolsMenu;
        private readonly NavigationMenuItemViewModel _manualTriggerMenu;
        private Action<Action> _uiDispatcher = action => action();

        private static readonly string CameraParameterFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "parameter", "camera-parameters.json");

        public MainFrmViewModel()
        {
            EditorViewModel = new EditorViewModel();
            CommunicationFrmViewModel = new CommunicationFrmViewModel();
            Cameras = new ObservableCollection<CameraPanelViewModel>();
            LoadCameraDisplays();
            PictureUtils.SetAllCamera(Cameras);
            Devices = new ObservableCollection<DeviceStatusViewModel>();
            CommunicationFrmViewModel.ConfigurationsChanged += RefreshConfiguredDevices;
            CommunicationFrmViewModel.DeviceConfigurations.ListChanged +=
                (sender, args) => RefreshConfiguredDevices();
            CommunicationFrmViewModel.DeviceConnectionStatusChanged +=
                UpdateDeviceConnectionStatus;
            RefreshConfiguredDevices();

            _cameraToolsMenu = new NavigationMenuItemViewModel(
                MainMenuKeys.CameraTools,
                "相机工具");
            _visionToolsMenu = new NavigationMenuItemViewModel(
                MainMenuKeys.VisionTools,
                "视觉工具");
            _manualTriggerMenu = new NavigationMenuItemViewModel(
                MainMenuKeys.ManualTrigger,
                "手动触发");
            RefreshCameraToolMenus();
            Cameras.CollectionChanged += (sender, args) => RefreshCameraToolMenus();

            NavigationMenus = new ObservableCollection<NavigationMenuItemViewModel>
            {
                new NavigationMenuItemViewModel(
                    MainMenuKeys.System,
                    "系统",
                    children: new[]
                    {
                        new NavigationMenuItemViewModel(
                            MainMenuKeys.Parameters,
                            "配方应用",
                            CreateNavigationCommand(1)),
                        new NavigationMenuItemViewModel(
                            MainMenuKeys.CameraConfig,
                            "图像窗口配置"
                        ),
                        new NavigationMenuItemViewModel(
                            MainMenuKeys.ParametersConfigDir,
                            "参数路径设置"
                        ),
                        new NavigationMenuItemViewModel(MainMenuKeys.HslCommunication,
                            "通讯秘钥")
                    }),
                new NavigationMenuItemViewModel(
                    MainMenuKeys.Editor, "流程编辑"
                ),
                new NavigationMenuItemViewModel(
                    MainMenuKeys.Records,
                    "查询记录",
                    children: new[]
                    {
                        new NavigationMenuItemViewModel(
                            MainMenuKeys.LiveLogs,
                            "实时日志",
                            CreateNavigationCommand(0, 0)),
                        new NavigationMenuItemViewModel(
                            MainMenuKeys.HistoryLogs,
                            "历史日志",
                            CreateNavigationCommand(0, 1)),
                        new NavigationMenuItemViewModel(
                            MainMenuKeys.StorageSettings,
                            "存储设置",
                            CreateNavigationCommand(0, 2))
                    }),
                // new NavigationMenuItemViewModel(MainMenuKeys.Registration, "软件注册"),
                new NavigationMenuItemViewModel(MainMenuKeys.Communication, "通讯"
                ),
                new NavigationMenuItemViewModel(MainMenuKeys.Users, "用户管理",
                    children: new[]
                    {
                        new NavigationMenuItemViewModel(MainMenuKeys.Login, "用户登录"),
                        new NavigationMenuItemViewModel(MainMenuKeys.Register, "用户注册"),
                        new NavigationMenuItemViewModel(MainMenuKeys.UserManager, "用户管理"),
                    }),
                _cameraToolsMenu,
                _visionToolsMenu,
                _manualTriggerMenu,
                // new NavigationMenuItemViewModel(
                //     MainMenuKeys.LightControl,
                //     "光源控制",
                //     children: new[]
                //     {
                //         new NavigationMenuItemViewModel(MainMenuKeys.Light1, "光源控制 1",
                //             children: new[]
                //             {
                //                 new NavigationMenuItemViewModel("OpenLight1", "打开光源1",
                //                     new RelayCommand(() => MessageBox.Show("打开光源1"))),
                //                 new NavigationMenuItemViewModel("CloseLight1", "关闭光源1"),
                //             }),
                //         new NavigationMenuItemViewModel(MainMenuKeys.Light2, "光源控制 2"),
                //         new NavigationMenuItemViewModel(MainMenuKeys.Light3, "光源控制 3")
                //     }),
                new NavigationMenuItemViewModel(MainMenuKeys.Help, "帮助")
            };
        }

        private void RefreshCameraToolMenus()
        {
            RefreshCameraToolMenu(_cameraToolsMenu, MainMenuKeys.CameraTools, "相机工具",
                BlockToolUtils.OpenCogAcqFifo, IsCameraToolLoaded);
            RefreshCameraToolMenu(_visionToolsMenu, MainMenuKeys.VisionTools, "视觉工具",
                BlockToolUtils.OpenCogToolBlock, IsVisionToolLoaded);
            RefreshCameraToolMenu(_manualTriggerMenu, MainMenuKeys.ManualTrigger, "手动触发");
        }

        public void RefreshToolMenuStates()
        {
            RefreshToolMenuStates(_cameraToolsMenu, MainMenuKeys.CameraTools,
                "相机工具", IsCameraToolLoaded);
            RefreshToolMenuStates(_visionToolsMenu, MainMenuKeys.VisionTools,
                "视觉工具", IsVisionToolLoaded);
        }

        public void ApplyUserPermissions()
        {
            bool canOperate = UserSession.CanApplyRecipe;
            bool canConfigure = UserSession.CanConfigure;

            RefreshToolMenuStates();
            SetMenuEnabled(MainMenuKeys.System, canOperate);
            SetMenuEnabled(MainMenuKeys.Parameters, canOperate);
            SetMenuEnabled(MainMenuKeys.CameraConfig, canConfigure);
            SetMenuEnabled(MainMenuKeys.ParametersConfigDir, canConfigure);
            SetMenuEnabled(MainMenuKeys.HslCommunication, canConfigure);
            SetMenuEnabled(MainMenuKeys.Editor, canConfigure);
            SetMenuEnabled(MainMenuKeys.Records, canOperate);
            SetMenuEnabled(MainMenuKeys.LiveLogs, canOperate);
            SetMenuEnabled(MainMenuKeys.HistoryLogs, canOperate);
            SetMenuEnabled(MainMenuKeys.StorageSettings, canConfigure);
            SetMenuEnabled(MainMenuKeys.Communication, canConfigure);
            SetMenuEnabled(MainMenuKeys.UserManager,
                UserSession.IsAdministrator);
            SetMenuEnabled(MainMenuKeys.CameraTools, canConfigure);
            SetMenuEnabled(MainMenuKeys.VisionTools, canConfigure);
            SetMenuEnabled(MainMenuKeys.ManualTrigger, canOperate, true);
        }

        private void SetMenuEnabled(string key, bool enabled,
            bool includeChildren = false)
        {
            NavigationMenuItemViewModel menu = FindMenuItem(
                NavigationMenus, key);
            if (menu == null) return;
            menu.IsEnabled = enabled;
            if (!includeChildren) return;
            SetChildrenEnabled(menu.Children, enabled);
        }

        private static void SetChildrenEnabled(
            IEnumerable<NavigationMenuItemViewModel> items, bool enabled)
        {
            foreach (NavigationMenuItemViewModel item in items)
            {
                item.IsEnabled = enabled;
                SetChildrenEnabled(item.Children, enabled);
            }
        }

        private static NavigationMenuItemViewModel FindMenuItem(
            IEnumerable<NavigationMenuItemViewModel> items, string key)
        {
            foreach (NavigationMenuItemViewModel item in items)
            {
                if (string.Equals(item.Key, key, StringComparison.Ordinal))
                    return item;
                NavigationMenuItemViewModel child = FindMenuItem(
                    item.Children, key);
                if (child != null) return child;
            }
            return null;
        }


        public void SaveCameraParameters()
        {
            try
            {
                CreateCameraDisplayConfigurations(Cameras)
                    .WriteJson(CameraParameterFilePath);
            }
            catch (Exception ex)
            {
                StatusMessage = "图像窗口配置保存失败：" + ex.Message;
            }
        }

        public bool TryConfigureCameraDisplays(
            IEnumerable<CameraDisplayConfiguration> configurations,
            out string errorMessage)
        {
            try
            {
                List<CameraDisplayConfiguration> normalized =
                    NormalizeCameraDisplayConfigurations(configurations, true);
                normalized.WriteJson(CameraParameterFilePath);

                Cameras.Clear();
                foreach (CameraDisplayConfiguration configuration in normalized)
                {
                    Cameras.Add(new CameraPanelViewModel(configuration.Index,
                        configuration.Name, configuration.ParameterName,
                        configuration.ProductName));
                }

                PictureUtils.SetAllCamera(Cameras);
                StatusMessage = "图像窗口配置已保存";
                errorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.GetBaseException().Message;
                StatusMessage = "图像窗口配置保存失败：" + errorMessage;
                return false;
            }
        }

        private void LoadCameraDisplays()
        {
            try
            {
                List<CameraDisplayConfiguration> settings =
                    CameraParameterFilePath.ReadJson<
                        List<CameraDisplayConfiguration>>();
                List<CameraDisplayConfiguration> normalized =
                    NormalizeCameraDisplayConfigurations(settings, false);
                AddCameraDisplays(normalized.Count == 0
                    ? CreateDefaultCameraDisplayConfigurations()
                    : normalized);
            }
            catch (Exception ex)
            {
                AddCameraDisplays(CreateDefaultCameraDisplayConfigurations());
                AppLog.Error("图像窗口配置读取失败，已使用默认配置：" +
                             ex.GetBaseException().Message,
                    nameof(MainFrmViewModel));
            }
        }

        private void AddCameraDisplays(
            IEnumerable<CameraDisplayConfiguration> configurations)
        {
            foreach (CameraDisplayConfiguration configuration in configurations)
            {
                Cameras.Add(new CameraPanelViewModel(configuration.Index,
                    configuration.Name, configuration.ParameterName,
                    configuration.ProductName));
            }
        }

        private static List<CameraDisplayConfiguration>
            CreateCameraDisplayConfigurations(
                IEnumerable<CameraPanelViewModel> cameras)
        {
            return cameras.Select((camera, index) =>
                new CameraDisplayConfiguration
                {
                    Index = index + 1,
                    Name = camera.Name,
                    ProductName = camera.ProductName,
                    ParameterName = camera.ParameterName
                }).ToList();
        }

        private static List<CameraDisplayConfiguration>
            NormalizeCameraDisplayConfigurations(
                IEnumerable<CameraDisplayConfiguration> configurations,
                bool requireItems)
        {
            var source = (configurations ??
                          Enumerable.Empty<CameraDisplayConfiguration>())
                .Where(item => item != null)
                .OrderBy(item => item.Index)
                .ToList();
            if (requireItems && source.Count == 0)
                throw new InvalidOperationException("至少需要配置一个图像窗口。");

            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var result = new List<CameraDisplayConfiguration>();
            for (int index = 0; index < source.Count; index++)
            {
                CameraDisplayConfiguration item = source[index];
                string name = (item.Name ?? string.Empty).Trim();
                if (name.Length == 0)
                    name = "相机 " + (index + 1);
                string productName = (item.ProductName ?? string.Empty).Trim();

                if (requireItems && productName.Length == 0)
                    throw new InvalidOperationException("图像窗口“" + name +
                                                        "”的产品 Key 不能为空。");
                if (!names.Add(name))
                    throw new InvalidOperationException("图像窗口名称不能重复：" +
                                                        name + "。");

                result.Add(new CameraDisplayConfiguration
                {
                    Index = index + 1,
                    Name = name,
                    ProductName = productName,
                    ParameterName = (item.ParameterName ?? string.Empty).Trim()
                });
            }

            return result;
        }

        private static List<CameraDisplayConfiguration>
            CreateDefaultCameraDisplayConfigurations()
        {
            return Enumerable.Range(1, 4).Select(index =>
                new CameraDisplayConfiguration
                {
                    Index = index,
                    Name = "相机 " + index,
                    ProductName = "产品" + index,
                    ParameterName = string.Empty
                }).ToList();
        }

        public void LoadTools()
        {
            foreach (CameraPanelViewModel camera in Cameras)
                SetRecipeLoaded(camera.ProductName, false);

            var config = GlobalConfig.Instance;
            if (config == null || string.IsNullOrWhiteSpace(config.ConfigCommonDir))
            {
                StatusMessage = "工具加载失败：未配置 ConfigCommonDir";
                return;
            }

            var blockTool = BlockTool.Instance;
            var loadedCameraCount = 0;
            var loadedVisionCount = 0;
            foreach (var camera in Cameras)
            {
                if (string.IsNullOrWhiteSpace(camera.ProductName) ||
                    string.IsNullOrWhiteSpace(camera.ParameterName))
                    continue;

                var parameterDirectory = Path.Combine(
                    config.ConfigCommonDir,
                    "产品型录",
                    camera.ProductName,
                    camera.ParameterName);
                var cameraToolDirectory = Path.Combine(parameterDirectory, "相机工具");
                var visionToolDirectory = Path.Combine(parameterDirectory, "视觉工具");
                var visionLoaded = false;
                var cameraLoaded = false;


                try
                {
                    blockTool.LoadCogToolBlock(visionToolDirectory, camera.ProductName);
                    visionLoaded = blockTool.GetCogToolBlock(
                        camera.ProductName).Count > 0;
                    if (!visionLoaded)
                        throw new InvalidDataException("视觉工具目录中没有 VPP 文件。");
                    loadedVisionCount++;
                    AppLog.Info(camera.Name + " 视觉工具加载成功");
                }
                catch (Exception ex)
                {
                    AppLog.Error(camera.Name + " 视觉工具加载失败" + Environment.NewLine + ex,
                        nameof(MainFrmViewModel));
                }

                try
                {
                    blockTool.LoadCogAcqFifo(cameraToolDirectory, camera.ProductName);
                    cameraLoaded = blockTool.GetCogAcqFifo(
                        camera.ProductName).Count > 0;
                    if (!cameraLoaded)
                        throw new InvalidDataException("相机工具目录中没有 VPP 文件。");
                    loadedCameraCount++;
                    AppLog.Info(camera.Name + " 相机工具加载成功");
                }
                catch (Exception ex)
                {
                    AppLog.Error(camera.Name + " 相机工具加载失败" + Environment.NewLine + ex,
                        nameof(MainFrmViewModel));
                }

                SetRecipeLoaded(camera.ProductName, visionLoaded && cameraLoaded);
            }

            StatusMessage = "已加载 " + loadedCameraCount + " 组相机工具、" +
                            loadedVisionCount + " 组视觉工具";
        }

        public void SetRecipeLoaded(string productName, bool loaded)
        {
            string key = (productName ?? string.Empty).Trim();
            if (key.Length == 0) return;

            DispatchToUi(() =>
            {
                foreach (CameraPanelViewModel camera in Cameras.Where(item =>
                             string.Equals(item.ProductName, key,
                                 StringComparison.OrdinalIgnoreCase)))
                    camera.IsConnected = loaded;
            });
        }

        private void RefreshCameraToolMenu(
            NavigationMenuItemViewModel menu,
            string keyPrefix, string keyName = "",
            Action<string, string, string> commandAction = null,
            Func<string, string, bool> isToolLoaded = null)
        {
            menu.Children.Clear();
            foreach (var camera in Cameras)
            {
                var nai = new NavigationMenuItemViewModel(
                    keyPrefix + ".Camera" + camera.Index,
                    camera.Name + " " + keyName);

                if (keyPrefix == MainMenuKeys.ManualTrigger)
                {
                    nai.Command = new RelayCommand(
                        () =>
                        {
                            if (!UserSession.CanApplyRecipe) return;
                            ManualTriggerUtils.SetTrigger(
                                camera.ProductName, true);
                        },
                        () => UserSession.CanApplyRecipe);
                }
                else if (commandAction != null)
                {
                    // var dir = System.IO.Path.Combine(GlobalConfig.Instance.ConfigCommonDir,
                    //     "产品型录", camera.ProductName, camera.ParameterName, keyName, camera.ParameterName
                    // );
                    var key = keyName + "1";
                    if (key.Contains("相机"))
                    {
                        key = key.Replace("工具", "");
                    }

                    nai.IsEnabled = UserSession.CanConfigure &&
                                    (isToolLoaded == null ||
                                     isToolLoaded(camera.ProductName, key));
                    nai.Command = new RelayCommand(() =>
                        {
                            var dir = System.IO.Path.Combine(GlobalConfig.Instance.ConfigCommonDir,
                                "产品型录", camera.ProductName, camera.ParameterName, keyName, camera.ParameterName
                            );
                            var key = keyName + "1";
                            if (key.Contains("相机"))
                            {
                                key = key.Replace("工具", "");
                            }
                            if (!UserSession.CanConfigure) return;
                            commandAction(dir, camera.ProductName, key);
                        },
                        () => UserSession.CanConfigure);
                }

                menu.Children.Add(nai);
            }
        }

        private void RefreshToolMenuStates(
            NavigationMenuItemViewModel menu,
            string keyPrefix,
            string keyName,
            Func<string, string, bool> isToolLoaded)
        {
            foreach (var camera in Cameras)
            {
                var menuKey = keyPrefix + ".Camera" + camera.Index;
                var menuItem = menu.Children.FirstOrDefault(item => item.Key == menuKey);
                if (menuItem == null) continue;

                var toolName = keyName + "1";
                if (toolName.Contains("相机"))
                    toolName = toolName.Replace("工具", "");

                menuItem.IsEnabled = UserSession.CanConfigure &&
                                     isToolLoaded(camera.ProductName, toolName);
            }
        }

        private static bool IsCameraToolLoaded(string productName, string toolName)
        {
            return !string.IsNullOrWhiteSpace(productName) &&
                   BlockTool.Instance.GetCogAcqFifo(productName).ContainsKey(toolName);
        }

        private static bool IsVisionToolLoaded(string productName, string toolName)
        {
            return !string.IsNullOrWhiteSpace(productName) &&
                   BlockTool.Instance.GetCogToolBlock(productName).ContainsKey(toolName);
        }

        private RelayCommand CreateNavigationCommand(int pageIndex, int? logPageIndex = null)
        {
            return new RelayCommand(() =>
            {
                SelectedPageIndex = pageIndex;
                if (logPageIndex.HasValue) SelectedLogPage = logPageIndex.Value;
            });
        }

        public ObservableCollection<CameraPanelViewModel> Cameras { get; }
        public ObservableCollection<DeviceStatusViewModel> Devices { get; }
        public ObservableCollection<NavigationMenuItemViewModel> NavigationMenus { get; }

        public void SetUiDispatcher(Action<Action> dispatcher)
        {
            _uiDispatcher = dispatcher ?? (action => action());
        }

        private void RefreshConfiguredDevices()
        {
            DispatchToUi(() =>
            {
                var configurations = CommunicationFrmViewModel.DeviceConfigurations
                    .Where(item => item != null && item.Enabled &&
                                   !string.IsNullOrWhiteSpace(item.Name))
                    .OrderBy(item => GetDeviceTypeOrder(item.DeviceType))
                    .ToList();

                Devices.Clear();
                foreach (var configuration in configurations)
                {
                    string key = configuration.Name.Trim();
                    Devices.Add(new DeviceStatusViewModel(key, key,
                        configuration.DeviceType)
                    {
                        IsConnected = CommunicationFrmViewModel.IsDeviceConnected(
                            key, configuration.DeviceType)
                    });
                }
            });
        }

        private void UpdateDeviceConnectionStatus(string key,
            CommunicationDeviceType deviceType, bool connected)
        {
            DispatchToUi(() =>
            {
                foreach (DeviceStatusViewModel device in Devices.Where(device =>
                             device.DeviceType == deviceType &&
                             string.Equals(device.Key, key,
                                 StringComparison.OrdinalIgnoreCase)))
                    device.IsConnected = connected;
            });
        }

        private void DispatchToUi(Action action)
        {
            Action<Action> dispatcher = _uiDispatcher;
            dispatcher(action);
        }

        private static int GetDeviceTypeOrder(CommunicationDeviceType deviceType)
        {
            switch (deviceType)
            {
                case CommunicationDeviceType.Plc: return 0;
                case CommunicationDeviceType.Camera: return 1;
                default: return 2;
            }
        }

        public int SelectedPageIndex
        {
            get => _selectedPageIndex;
            set => SetProperty(ref _selectedPageIndex, value);
        }

        public int SelectedLogPage
        {
            get => _selectedLogPage;
            set => SetProperty(ref _selectedLogPage, value);
        }

        public string CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value ?? string.Empty);
        }

        public string OperationMode
        {
            get => _operationMode;
            set => SetProperty(ref _operationMode, value ?? string.Empty);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value ?? string.Empty);
        }

        public int CycleCount
        {
            get => _cycleCount;
            set => SetProperty(ref _cycleCount, value);
        }

        public int Light1Brightness
        {
            get => _light1Brightness;
            set => SetProperty(ref _light1Brightness, value);
        }

        public int Light2Brightness
        {
            get => _light2Brightness;
            set => SetProperty(ref _light2Brightness, value);
        }

        public int Light3Brightness
        {
            get => _light3Brightness;
            set => SetProperty(ref _light3Brightness, value);
        }
    }
}
