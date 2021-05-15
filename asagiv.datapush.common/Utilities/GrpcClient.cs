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
        public event EventHandler<byte[]> DataRetrieved;
        #endregion

        #region Properties
        public DataPush.DataPushClient Client { get; }
        public IList<DataPullSubscriber> PullSubscribers { get; }
        #endregion

        #region Constructor
        public GrpcClient(string address, uint port) : this($"http://{address}:{port}") { }

        public GrpcClient(string connectionString)
        {
            _channel = GrpcChannel.ForAddress(connectionString);

            Client = new DataPush.DataPushClient(_channel);

            PullSubscribers = new List<DataPullSubscriber>();
        }

        public GrpcClient(ChannelBase channel)
        {
            _channel = channel;

            Client = new DataPush.DataPushClient(_channel);

            PullSubscribers = new List<DataPullSubscriber>();
        }
        #endregion

        #region Methods
        public void CreatePullSubscriber(string topic)
        {
            var hasSubscriber = PullSubscribers
                .Any(x => x.Topic == topic);

            if (hasSubscriber) return;

            var subscriberToAdd = new DataPullSubscriber(Client, topic);

            subscriberToAdd.DataRetrieved += OnPullDataRetrieved;

            PullSubscribers.Add(subscriberToAdd);
        }

        private void OnPullDataRetrieved(object sender, ByteString e)
        {
            var byteArray = e.ToByteArray();

            DataRetrieved?.Invoke(sender, byteArray);
        }

        public async Task<bool> PushFileAsync(string topic, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            var data = await File.ReadAllBytesAsync(filePath);

            return await PushDataAsync(topic, data);
        }
        public async Task<bool> PushDataAsync(string topic, byte[] data) 
        {
            var request = new DataPushRequest
            {
                Topic = topic,
                Data = ByteString.CopyFrom(data)
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
