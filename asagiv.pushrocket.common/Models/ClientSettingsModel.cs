using asagiv.pushrocket.common.Interfaces;
using asagiv.pushrocket.common.Utilities;
using Grpc.Net.Client;
using ReactiveUI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Models
{
    public class ClientSettingsModel : ReactiveObject, IClientSettingsModel
    {
        #region Fields
        protected readonly ILogger _logger;
        private string _destinationNode;
        private IClientConnectionSettings _connectionSettings;
        #endregion

        #region Properties
        public IClientConnectionSettings ConnectionSettings
        {
            get => _connectionSettings;
            set => this.RaiseAndSetIfChanged(ref _connectionSettings, value);
        }
        public string DestinationNode
        {
            get => _destinationNode;
            set => this.RaiseAndSetIfChanged(ref _destinationNode, value);
        }
        public IGrpcClient Client { get; protected set; }
        #endregion

        #region Constructor
        public ClientSettingsModel(ILogger logger)
        {
            _logger = logger;

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
                var channel = GrpcChannel.ForAddress(ConnectionSettings.ConnectionString);

                Client = new GrpcClient(ConnectionSettings, channel, GrpcClientFactory.GetDeviceId(), _logger);

                // Register the current node and get the available pull nodes.
                var pullNodesToAdd = await Client.RegisterNodeAsync(false);

                // Add the pull nodes to the list.
                pullNodes.AddRange(pullNodesToAdd);

                _logger.Information("Successfully connected to {0}", ConnectionSettings.ConnectionString);

                return pullNodes;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Server Connection Error");

                throw;
            }
        }

        protected virtual bool IsPullNodeSelected()
        {
            return !string.IsNullOrWhiteSpace(DestinationNode);
        }

        protected virtual bool IsConnectionSettingSelected()
        {
            return _connectionSettings != null;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            Client.Dispose();
        }
        #endregion
    }
}
