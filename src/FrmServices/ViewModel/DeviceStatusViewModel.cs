using CommunityToolkit.Mvvm.ComponentModel;

namespace FrmServices.ViewModel
{
    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
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
