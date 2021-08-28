using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using asagiv.datapush.common.Utilities;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.common.Models
{
    public abstract class ClientSettingsModelBase : ReactiveObject, IDisposable
    {
        #region Fields
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
        protected IGrpcClient Client { get; set; }
        #endregion

        #region Methods
        public virtual async Task<IList<string>> ConnectClientAsync()
        {
            var pullNodes = new List<string>();

            if (!IsConnectionSettingSelected())
            {
                return pullNodes;
            }

            try
            {
                Client = new GrpcClient(ConnectionSettings, GrpcClientFactory.GetDeviceId());

                var pullNodesToAdd = await Client.RegisterNodeAsync(false);

                pullNodes.AddRange(pullNodesToAdd);
            }
            catch (Exception ex)
            {
                // TODO: Log Error.
            }

            return pullNodes;
        }

        public async Task<IDataPushContext> CreatePushContextAsync(string filePath)
        {
            if (!IsPullNodeSelected())
            {
                return null;
            }

            try
            {
                var contextToAdd = await Client.CreatePushFileContextAsync(DestinationNode, filePath);

                return contextToAdd;
            }
            catch (Exception ex)
            {
                // TODO: Log Error.
            }

            return null;
        }

        /// <summary></summary>
        /// <returns>True if a pull node is selected.</returns>
        protected virtual bool IsPullNodeSelected()
        {
            return !string.IsNullOrWhiteSpace(DestinationNode);
        }

        /// <summary></summary>
        /// <returns>True if a connection setting is selected.</returns>
        protected virtual bool IsConnectionSettingSelected()
        {
            return _connectionSettings != null;
        }

        public void Dispose()
        {
            Client.Dispose();
        }
        #endregion
    }
}
