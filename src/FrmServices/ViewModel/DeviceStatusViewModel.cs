using CommunityToolkit.Mvvm.ComponentModel;

namespace FrmServices.ViewModel
{
    public sealed class DeviceStatusViewModel : ObservableObject
    {
        private bool _isConnected;

        public DeviceStatusViewModel(string key, string name,
            CommunicationDeviceType deviceType)
        {
            Key = key;
            Name = name;
            DeviceType = deviceType;
        }

        public string Key { get; }
        public string Name { get; }
        public CommunicationDeviceType DeviceType { get; }

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (!SetProperty(ref _isConnected, value)) return;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public string StatusText => IsConnected ? "已连接" : "未连接";
    }
}
