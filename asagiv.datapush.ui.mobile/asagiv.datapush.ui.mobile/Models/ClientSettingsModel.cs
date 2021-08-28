using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Models;
using asagiv.datapush.ui.common.Models;
using asagiv.datapush.ui.mobile.Utilities;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace asagiv.datapush.ui.mobile.Models
{
    public class ClientSettingsModel : ClientSettingsModelBase
    {
        public override async Task<IList<string>> ConnectClientAsync()
        {
            LoggerInstance.Instance.Log.Information($"Connecting to PushRocket server: connection string {ConnectionSettings.ConnectionName}.");

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

            if (!IsConnectionSettingSelected())
            {
                return destinationNodes;
            }

            var channel = new Channel(ConnectionSettings.ConnectionString, ChannelCredentials.Insecure);

            Client = new GrpcClient(ConnectionSettings, channel, deviceId, LoggerInstance.Instance.Log);

            var destinationNodesToAdd = await Client.RegisterNodeAsync(false);

            destinationNodes.AddRange(destinationNodesToAdd);

            LoggerInstance.Instance.Log.Information($"Node Registration and Connection Successful.");

            return destinationNodes;
        }

        public IDataPushContext CreatePushDataContext(string shareName, byte[] data)
        {
            return Client.CreatePushDataContext(DestinationNode, shareName, data);
        }
    }
}
