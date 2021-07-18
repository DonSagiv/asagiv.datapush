﻿using asagiv.datapush.common.Models;
using Grpc.Core;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using System.Collections.Generic;

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
            set 
            {
                _connectionString = value;
                RaisePropertyChanged(nameof(ConnectionString));
                Preferences.Set(nameof(_connectionString), _connectionString);
            }
        }
        public string NodeName
        {
            get { return _nodeName; }
            set 
            {
                _nodeName = value;
                RaisePropertyChanged(nameof(NodeName));
                Preferences.Set(nameof(_nodeName), _nodeName);
            }
        }
        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; RaisePropertyChanged(nameof(IsConnected)); }
        }
        public string SelectedDestinationNode
        {
            get { return _selectedDestinationNode; }
            set 
            {
                _selectedDestinationNode = value;
                RaisePropertyChanged(nameof(SelectedDestinationNode));
                Preferences.Set(nameof(_selectedDestinationNode), _selectedDestinationNode);
            }
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
            // Retrieves last-used connection settings.
            // Todo: create list of saved connection settings.
            ConnectionString = Preferences.Get(nameof(_connectionString), string.Empty);
            NodeName = Preferences.Get(nameof(_nodeName), string.Empty);

            ConnectToServerCommand = new DelegateCommand(async () => await ConnectToServerAsync());

            PushFilesCommand = new DelegateCommand(async () => await PushFilesAsync());

            DestinationNodeList = new ObservableCollection<string>();

            PushDataContextList = new ObservableCollection<DataPushContext>();
        }
        #endregion

        #region Methods
        public async Task ConnectToServerAsync()
        {
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
            var channel = new Channel(_connectionString, ChannelCredentials.Insecure);

            // Start the GRPC Client.
            _client = new GrpcClient(channel, _nodeName, deviceId);

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

        public async Task PushFilesAsync(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var context = await _client.CreatePushFileContextAsync(SelectedDestinationNode, file);

                PushDataContextList.Insert(0, context);

                await context.PushDataAsync();
            }
        }
        #endregion
    }
}
