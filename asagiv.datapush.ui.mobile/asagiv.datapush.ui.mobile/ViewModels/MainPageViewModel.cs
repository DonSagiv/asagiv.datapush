using asagiv.datapush.common.Models;
using asagiv.datapush.common.Utilities;
using Grpc.Core;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using System;
using System.Collections.ObjectModel;
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
        public ObservableCollection<DataPushContext> PushContextList { get; }
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
        public GrpcClient Client { get; private set; }
        #endregion

        #region Commands
        public ICommand ConnectCommand { get; }
        public ICommand SelectFileCommand { get; }
        #endregion

        #region Constructor
        public MainPageViewModel(ILogger logger, RaiseEventLogSink eventLogSink)
        {
            _logger = logger;

            IsConnected = false;

            DestinationNodeList = new ObservableCollection<string>();
            PushContextList = new ObservableCollection<DataPushContext>();

            _logger.Information("Initializing View Model.");

            ConnectCommand = new DelegateCommand(async () => await ConnectAsync());
            SelectFileCommand = new DelegateCommand(async () => await PushFilesAsync());
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

            IsConnected = true;
        }

        private async Task PushFilesAsync()
        {
            var files = await FilePicker.PickMultipleAsync();

            foreach(var file in files)
            {
                await PushFileAsync(file);
            }
        }

        private async Task PushFileAsync(FileResult file)
        {
            try
            {
                var contextToAdd = await GetPushContext(file.FullPath);

                PushContextList.Insert(0, contextToAdd);

                await contextToAdd.PushDataAsync();
            }
            catch (Exception ex)
            {
                _logger?.Error(ex.ToString());
            }
        }

        private async Task<DataPushContext> GetPushContext(string fileName)
        {
            return await Client.CreatePushFileContextAsync(SelectedDestinationNode, fileName);
        }
        #endregion
    }
}
