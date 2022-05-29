using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using asagiv.datapush.common.Utilities;
using asagiv.datapush.ui.common.Interfaces;
using ReactiveUI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.common.Models
{
    public abstract class ClientSettingsModelBase : ReactiveObject, IClientSettingsModel
    {
        #region Fields
        protected readonly ILogger _logger;
        private string _destinationNode;
        private IClientConnectionSettings _connectionSettings;
        #endregion

        #region Properties
        public IClientConnectionSettings ConnectionSettings
        {
            get { return _connectionSettings; }
            set { this.RaiseAndSetIfChanged(ref _connectionSettings, value); }
        }
        public string DestinationNode
        {
            get { return _destinationNode; }
            set { this.RaiseAndSetIfChanged(ref _destinationNode, value); }
        }
        public IGrpcClient Client { get; protected set; }
        #endregion

        #region Constructor
        protected ClientSettingsModelBase(ILogger logger)
        {
            _logger = logger;
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

            Client = new GrpcClient(ConnectionSettings, GrpcClientFactory.GetDeviceId(), _logger);

            // Register the current node and get the available pull nodes.
            var pullNodesToAdd = await Client.RegisterNodeAsync(false);

            // Add the pull nodes to the list.
            pullNodes.AddRange(pullNodesToAdd);

            return pullNodes;
        }

        public async Task<IDataPushContext> CreatePushContextAsync(string filePath)
        {
            if (!IsPullNodeSelected())
            {
                return null;
            }

            var contextToAdd = await Client.CreatePushFileContextAsync(DestinationNode, filePath);

            return contextToAdd;
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
