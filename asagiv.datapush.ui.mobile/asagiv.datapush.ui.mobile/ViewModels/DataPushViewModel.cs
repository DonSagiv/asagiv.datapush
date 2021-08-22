using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using asagiv.datapush.ui.mobile.Utilities;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace asagiv.datapush.ui.mobile.ViewModels
{
    public class DataPushViewModel : ReactiveObject
    {
        #region Fields
        private IGrpcClient _client;
        private IClientConnectionSettings _connectionSettings;
        private string _selectedDestinationNode;
        #endregion

        #region Properties
        public ObservableCollection<IClientConnectionSettings> ConnectionSettingsList { get; }
        public bool IsConnected { get; private set; }
        public IClientConnectionSettings ConnectionSettings
        {
            get => _connectionSettings;
            set => this.RaiseAndSetIfChanged(ref _connectionSettings, value);
        }
        public ObservableCollection<string> DestinationNodeList { get; }
        public string SelectedDestinationNode
        {
            get => _selectedDestinationNode;
            set => this.RaiseAndSetIfChanged(ref _selectedDestinationNode, value);
        }
        public ObservableCollection<IDataPushContext> PushDataContextList { get; }
        #endregion

        #region Commands
        public ICommand PushFilesCommand { get; }
        #endregion

        #region Constructor
        public DataPushViewModel()
        {
            LoggerInstance.Instance.Log.Information($"Initializing DataPushViewModel.");

            PushFilesCommand = ReactiveCommand.Create(async () => await PushFilesAsync());

            DestinationNodeList = new ObservableCollection<string>();

            PushDataContextList = new ObservableCollection<IDataPushContext>();

            this.WhenAnyValue(x => x.SelectedDestinationNode)
                .Where(x => x != null)
                .Subscribe(async x => await ConnectClientAsync());
        }
        #endregion

        #region Methods
        public async Task RefreshConnectionSettingsAsync()
        {
            ConnectionSettingsList.Clear();

            var connectionSettingsToAdd = await XFormsDataPusDbContext.Instance.ConnectionSettingsSet
                .OrderBy(x => x.ConnectionName)
                .ToListAsync();

            foreach(var item in connectionSettingsToAdd)
            {
                ConnectionSettingsList.Add(item);
            }
        }

        public async Task ConnectClientAsync()
        {
            LoggerInstance.Instance.Log.Information($"Connecting to PushRocket server: connection string {_connectionSettings.ConnectionName}.");

            // Allows app to use HTTP insecure connections.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            // Get the device ID.
            var deviceId = Preferences.Get("deviceId", string.Empty);

            // If there is no device ID, make one up and save it.
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();
                Preferences.Set("deviceId", deviceId);
            }

            // Create a new connection channel with HTTP insecure credentials.
            // Todo: consider TLS/SSL
            var channel = new Channel(_connectionSettings.ConnectionString, ChannelCredentials.Insecure);

            LoggerInstance.Instance.Log.Information($"Registering Node: {_connectionSettings.NodeName} for device {deviceId}.");

            // Start the GRPC Client.
            _client = new GrpcClient(_connectionSettings, deviceId, LoggerInstance.Instance.Log);

            var response = await _client.RegisterNodeAsync(false);

            // Populate the Destination Node list.
            DestinationNodeList.Clear();

            foreach (var item in response)
            {
                DestinationNodeList.Add(item);
            }

            // Retrieve the last known destination node.
            var previouslySelectedNode = Preferences.Get(nameof(_selectedDestinationNode), string.Empty);

            SelectedDestinationNode = DestinationNodeList.FirstOrDefault(x => x == previouslySelectedNode);

            IsConnected = true;

            LoggerInstance.Instance.Log.Information($"Node Registration and Connection Successful.");
        }

        private async Task PushFilesAsync()
        {
            var files = await FilePicker.PickMultipleAsync();

            if (files == null || !files.Any())
            {
                return;
            }

            var fileNames = files.Select(x => x.FullPath);

            await PushFilesAsync(fileNames);
        }

        public async Task PushShareStreamContexts(IEnumerable<ShareStreamContext> shareStreamContexts)
        {
            foreach(var streamContext in shareStreamContexts)
            {
                var shareName = $"{streamContext.ShareFileName ?? Guid.NewGuid().ToString()}.{streamContext.Extension}";

                LoggerInstance.Instance.Log.Information($"Creating Context for File: {shareName}.");

                using var ms = new MemoryStream();
                using (streamContext.InputStream)
                {
                    await streamContext.InputStream.CopyToAsync(ms);

                    var data = ms.ToArray();

                    await streamContext.InputStream.FlushAsync();

                    var pushContext = _client.CreatePushDataContext(SelectedDestinationNode, shareName, data);

                    await PushDataAsync(pushContext);
                }
            }
        }

        public async Task PushFilesAsync(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                LoggerInstance.Instance.Log.Information($"Creating Context for File: {file}.");

                var context = await _client.CreatePushFileContextAsync(SelectedDestinationNode, file);

                await PushDataAsync(context);
            }
        }

        private async Task PushDataAsync(IDataPushContext context)
        {
            LoggerInstance.Instance.Log.Information($"Pushing Data for Context: {context.Name}.");

            try
            {
                Device.BeginInvokeOnMainThread(() => 
                {
                    PushDataContextList.Insert(0, context);
                });
                
            }
            catch(Exception e)
            {
                LoggerInstance.Instance.Log.Error(e, e.Message);
            }

            await context.PushDataAsync();

            LoggerInstance.Instance.Log.Information($"Data Push Successful.");
        }
        #endregion
    }
}
