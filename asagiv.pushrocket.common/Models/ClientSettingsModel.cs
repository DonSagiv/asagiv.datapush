using asagiv.pushrocket.common.Interfaces;
using asagiv.pushrocket.common.Utilities;
using asagiv.common;
using Grpc.Net.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace asagiv.pushrocket.common.Models
{
    public class ClientSettingsModel : PropertyChangedBase, IClientSettingsModel
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly IGrpcDataDownloader _dataDownloader;
        private string _destinationNode;
        private IClientConnectionSettings _connectionSettings;
        #endregion

        #region Properties
        public IClientConnectionSettings ConnectionSettings
        {
            get => _connectionSettings;
            set => RaiseAndSetIfChanged(ref _connectionSettings, value, OnConnectionSettingsChanged);
        }
        public string DestinationNode
        {
            get => _destinationNode;
            set => RaiseAndSetIfChanged(ref _destinationNode, value);
        }
        public IGrpcClient Client { get; protected set; }
        #endregion

        #region Constructor
        public ClientSettingsModel(ILogger logger, IGrpcDataDownloader dataDownloader)
        {
            _logger = logger;
            _dataDownloader = dataDownloader;

            _logger.Debug("Client settings model instantiated.");
        }
        #endregion

        #region Methods
        public async virtual Task<IList<string>> ConnectToServerAsync()
        {
            var pullNodes = new List<string>();

            if (!IsConnectionSettingSelected())
            {
                return pullNodes;
            }

            _logger.Information("Appempting to connect to {ConnectionString}", ConnectionSettings.ConnectionString);

            try
            {
                // Create a channel for the gRPC address
                var channel = GrpcChannel.ForAddress(ConnectionSettings.ConnectionString);

                // Create a new client w/ the channel.
                Client = new GrpcClient(ConnectionSettings, channel, GrpcClientFactory.GetDeviceId(), _logger);

                // Create a subscriber for pulling data.
                await Client.CreatePullSubscriberAsync();

                // Register the current node and get the available pull nodes (including this one).
                var pullNodesToAdd = await Client.RegisterNodeAsync(true);

                // Subscribe the data pull observable to the downloader.
                Client.DataRetrievedObservable
                    .SelectMany(x => _dataDownloader.OnDataRetrievedAsync(x))
                    .SelectMany(x => Client.AcknowledgeDeliveryAsync(x))
                    .Subscribe();

                // Add the pull nodes to the list.
                pullNodes.AddRange(pullNodesToAdd);

                _logger.Information("Successfully connected to {0}", ConnectionSettings.ConnectionString);

                return pullNodes;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Server Connection Error");

                throw;
            }
        }

        private void _dataDownloader_AcknowledgeDelivery(object sender, AcknowledgeDeliveryRequest e) => throw new NotImplementedException();

        protected virtual bool IsPullNodeSelected()
        {
            return !string.IsNullOrWhiteSpace(DestinationNode);
        }

        protected virtual bool IsConnectionSettingSelected()
        {
            return _connectionSettings != null;
        }

        private void OnConnectionSettingsChanged(IClientConnectionSettings _)
        {
            // Clear the client and destination node (i.e. Disconnect).
            Client = null;
            DestinationNode = null;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            Client.Dispose();
        }
        #endregion
    }
}
