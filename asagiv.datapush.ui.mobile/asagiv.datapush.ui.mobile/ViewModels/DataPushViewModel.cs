using asagiv.datapush.common.Models;
using Grpc.Core;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class DataPushViewModel : BindableBase
    {
        #region Fields
        private GrpcClient _client;
        private string _connectionString;
        private string _nodeName;
        private bool _isConnected;
        private string _selectedDestinationNode;
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
        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; RaisePropertyChanged(nameof(IsConnected)); }
        }
        public string SelectedDestinationNode
        {
            get { return _selectedDestinationNode; }
            set { _selectedDestinationNode = value; RaisePropertyChanged(nameof(SelectedDestinationNode)); }
        }
        public ObservableCollection<string> DestinationNodeList { get; }
        public ObservableCollection<DataPushContext> PushDataContextList { get; }
        #endregion

        #region Commands
        public ICommand ConnectToServerCommand { get; }
        public ICommand PushFilesCommand { get; }
        #endregion

        #region Constructor
        public DataPushViewModel()
        {
            ConnectToServerCommand = new DelegateCommand(async () => await ConnectToServerAsync());

            PushFilesCommand = new DelegateCommand(async () => await PushFilesAsync());

            DestinationNodeList = new ObservableCollection<string>();

            PushDataContextList = new ObservableCollection<DataPushContext>();
        }
        #endregion

        #region Methods
        private async Task ConnectToServerAsync()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var deviceId = Preferences.Get("deviceId", string.Empty);

            if (string.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                Preferences.Set("deviceId", deviceId);
            }

            var channel = new Channel(_connectionString, ChannelCredentials.Insecure);

            _client = new GrpcClient(channel, _nodeName, deviceId);

            var response = await _client.RegisterNodeAsync(false);

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

            if(files == null || !files.Any())
            {
                return;
            }

            foreach (var file in files)
            {
                var context = await _client.CreatePushFileContextAsync(SelectedDestinationNode, file.FullPath);

                PushDataContextList.Insert(0, context);

                await context.PushDataAsync();
            }
        }
        #endregion
    }
}
