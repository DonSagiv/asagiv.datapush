using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Utilities
{
    public class GrpcClient
    {
        #region Fields
        private readonly ChannelBase _channel;
        #endregion

        #region Delegates
        public event EventHandler<DataPullResponse> DataRetrieved;
        #endregion

        #region Properties
        public DataPush.DataPushClient Client { get; }
        public IList<DataPullSubscriber> PullSubscribers { get; }
        public string NodeName { get; set; }
        public string DeviceId { get; set; }
        #endregion

        #region Constructor
        public GrpcClient(string connectionString, string nodeName, string deviceId)
        {
            _channel = GrpcChannel.ForAddress(connectionString);

            Client = new DataPush.DataPushClient(_channel);

            PullSubscribers = new List<DataPullSubscriber>();

            NodeName = nodeName;

            DeviceId = deviceId;
        }

        public GrpcClient(ChannelBase channel, string nodeName, string deviceId)
        {
            _channel = channel;

            Client = new DataPush.DataPushClient(_channel);

            PullSubscribers = new List<DataPullSubscriber>();

            NodeName = nodeName;

            DeviceId = deviceId;
        }
        #endregion

        #region Methods
        public Task CreatePullSubscriberAsync()
        {
            var hasSubscriber = PullSubscribers
                .Any(x => x.DestinationNode == NodeName);

            if (hasSubscriber) return Task.CompletedTask;

            var subscriberToAdd = new DataPullSubscriber(Client, NodeName);

            subscriberToAdd.DataRetrieved += OnPullDataRetrieved;

            PullSubscribers.Add(subscriberToAdd);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<string>> RegisterNodeAsync(bool isPullNode)
        {
            var nodeRequest = new RegisterNodeRequest
            {
                DeviceId = DeviceId,
                NodeName = NodeName,
                IsPullNode = isPullNode,
            };

            var response = await Client.RegisterNodeAsync(nodeRequest);

            return response.PullNodeList;
        }

        private void OnPullDataRetrieved(object sender, DataPullResponse e)
        {
            DataRetrieved?.Invoke(sender, e);
        }

        public async Task<bool> PushFileAsync(string destinationNode, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            var data = await File.ReadAllBytesAsync(filePath);

            var name = Path.GetFileName(filePath);

            return await PushDataAsync(NodeName, destinationNode, name, data);
        }

        public async Task<bool> PushDataAsync(string sourceNode, string destinationNode, string name, byte[] data) 
        {
            var request = new DataPushRequest
            {
                SourceNode = NodeName,
                DestinationNode = destinationNode,
                Name = name,
                Payload = ByteString.CopyFrom(data)
            };

            try
            {
                await Client.PushDataAsync(request);

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
        #endregion
    }
}
