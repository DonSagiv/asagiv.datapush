using asagiv.datapush.common.Utilities;
using Grpc.Core;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class MainPageViewModel : BindableBase
    {
        #region Fields
        private readonly ILogger _logger;
        private string _connectionString;
        private string _nodeName;
        private string _selectedDestinationNode;
        private bool _isConnected;
        #endregion

        #region Properties
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; RaisePropertyChanged(nameof(ConnectionString)); }
        }
        public string NodeName
        {
            get { return _nodeName; }
            set { _nodeName = value; RaisePropertyChanged(nameof(NodeName)); }
        }
        public ObservableCollection<string> DestinationNodeList { get; }
        public string SelectedDestinationNode
        {
            get { return _selectedDestinationNode; }
            set { _selectedDestinationNode = value; RaisePropertyChanged(nameof(SelectedDestinationNode)); }
        }
        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; RaisePropertyChanged(nameof(IsConnected)); }
        }
        public ObservableCollection<string> LogEntries { get; }
        public GrpcClient Client { get; private set; }
        #endregion

        #region Commands
        public ICommand ConnectCommand { get; }
        public ICommand SelectFileCommand { get; }
        #endregion

        #region Constructor
        public MainPageViewModel(ILogger logger)
        {
            _logger = logger;

            _logger.Information("Initializing View Model.");

            DestinationNodeList = new ObservableCollection<string>();

            LogEntries = new ObservableCollection<string>();

            ConnectCommand = new DelegateCommand(async () => await ConnectAsync());
            SelectFileCommand = new DelegateCommand(async () => await SelectFileAsync());
        }
        #endregion

        #region Methods
        private async Task ConnectAsync()
        {
            var deviceId = Preferences.Get("deviceId", string.Empty);

            if (string.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                Preferences.Set("deviceId", deviceId);
            }

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var channel = new Channel(_connectionString, ChannelCredentials.Insecure);

            Client = new GrpcClient(channel, _nodeName, deviceId, _logger);

            var response = await Client.RegisterNodeAsync(false);

            DestinationNodeList.Clear();

            foreach (var item in response)
            {
                DestinationNodeList.Add(item);
            }
        }

        private async Task SelectFileAsync()
        {
            var files = await FilePicker.PickMultipleAsync();

            foreach(var file in files)
            {
                var filePath = file.FullPath;

                _logger.Information($"Uploading File: {filePath} to {SelectedDestinationNode}");

                var data = await File.ReadAllBytesAsync(filePath);

                var fileName = Path.GetFileName(filePath);

                await Client.PushDataAsync(SelectedDestinationNode, fileName, data);
            }
        }
        #endregion
    }
}
