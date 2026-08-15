using CommunityToolkit.Mvvm.ComponentModel;
using Cognex.VisionPro;

namespace FrmServices.ViewModel
{
    public sealed class CameraPanelViewModel : ObservableObject
    {
        private string _parameterName;
        private string _productName;
        private bool _isConnected;
        private string _resultText = "";
        private decimal _exposure = 10M;
        private CogRecordDisplay _cogRecordDisplay;

        public CameraPanelViewModel(int index, string name, string parameterName, string productName)
        {
            _productName = productName ?? string.Empty;
            Index = index;
            Name = name;
            _parameterName = parameterName;
        }

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value ?? string.Empty);
        }

        public int Index { get; }
        public string Name { get; }

        public string ParameterName
        {
            get => _parameterName;
            set => SetProperty(ref _parameterName, value ?? string.Empty);
        }

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (!SetProperty(ref _isConnected, value)) return;
                OnPropertyChanged(nameof(ConnectionText));
            }
        }

        public string ConnectionText => IsConnected ? "在线" : "离线";

        public string ResultText
        {
            get => _resultText;
            set => SetProperty(ref _resultText, value ?? string.Empty);
        }

        public decimal Exposure
        {
            get => _exposure;
            set => SetProperty(ref _exposure, value);
        }

        public CogRecordDisplay CogRecordDisplay
        {
            get => _cogRecordDisplay;
            set => SetProperty(ref _cogRecordDisplay, value);
        }
    }
}
