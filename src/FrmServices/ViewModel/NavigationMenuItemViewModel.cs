using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FrmServices.ViewModel
{
    public static class MainMenuKeys
    {
        public const string System = "System";
        public const string Parameters = "Parameters";
        public const string Editor = "Editor";
        public const string Records = "Records";
        public const string LiveLogs = "LiveLogs";
        public const string HistoryLogs = "HistoryLogs";
        public const string StorageSettings = "StorageSettings";
        public const string Registration = "Registration";
        public const string Communication = "Communication";
        public const string Users = "Users";
        public const string CameraTools = "CameraTools";
        public const string VisionTools = "VisionTools";
        public const string Camera1 = "Camera1";
        public const string Camera2 = "Camera2";
        public const string Camera3 = "Camera3";
        public const string Camera4 = "Camera4";
        public const string ManualTrigger = "ManualTrigger";
        public const string LightControl = "LightControl";
        public const string Light1 = "Light1";
        public const string Light2 = "Light2";
        public const string Light3 = "Light3";
        public const string Help = "Help";
        public const string CameraConfig = "CameraConfig";
        public const string ParametersConfigDir = "ParametersConfigDir";
        public const string Login = "Login";
        public const string Register = "Register";
        public const string UserManager = "UserManager";
        public const string HslCommunication = "HslCommunication";
    }

    public sealed class NavigationMenuItemViewModel : ObservableObject
    {
        private string _text;
        private bool _isVisible = true;
        private bool _isEnabled = true;
        private ICommand _command;

        public NavigationMenuItemViewModel(
            string key,
            string text,
            ICommand command = null,
            IEnumerable<NavigationMenuItemViewModel> children = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("菜单 Key 不能为空。", nameof(key));

            Key = key;
            _text = text ?? string.Empty;
            _command = command;
            Children = new ObservableCollection<NavigationMenuItemViewModel>(
                children ?? Enumerable.Empty<NavigationMenuItemViewModel>());
        }

        public string Key { get; }
        public ObservableCollection<NavigationMenuItemViewModel> Children { get; }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value ?? string.Empty);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public ICommand Command
        {
            get => _command;
            set => SetProperty(ref _command, value);
        }
    }
}
