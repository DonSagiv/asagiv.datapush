using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using asagiv.datapush.ui.common.Models;
using Grpc.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace asagiv.datapush.ui.mobile.Models
{
    public class ClientSettingsModel : ClientSettingsModelBase
    {
        #region Constructor
        public ClientSettingsModel(ILogger logger) : base(logger) { }
        #endregion

        #region Methods
        public override async Task<IList<string>> ConnectToServerAsync()
        {
            _logger.Information($"Connecting to PushRocket server: connection string {ConnectionSettings.ConnectionName}.");

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

            var destinationNodes = new List<string>();

            // Retruns and empty list if no connection setting is selected.
            if (!IsConnectionSettingSelected())
            {
                return destinationNodes;
            }

            // Creates a new channel.
            var channel = new Channel(ConnectionSettings.ConnectionString, ChannelCredentials.Insecure);

            // Creates a new GRPC client w/ the channel.
            Client = new GrpcClient(ConnectionSettings, channel, deviceId, _logger);

            // Gets the destination nodes from the GRPC server.
            var destinationNodesToAdd = await Client.RegisterNodeAsync(false);

            destinationNodes.AddRange(destinationNodesToAdd);

            _logger.Information($"Node Registration and Connection Successful.");

            return destinationNodes;
        }

        public DataPushContext CreatePushDataContext(string shareName, byte[] data)
        {
            return Client.CreatePushDataContext(DestinationNode, shareName, data);
        }
        #endregion
    }
}
