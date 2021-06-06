using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Utilities
{
    public sealed class GrpcClient : IDisposable
    {
        #region Fields
        private readonly ILogger _logger;
        #endregion

        #region Delegates
        public event EventHandler Disposed;
        public event EventHandler<ResponseStreamContext<DataPullResponse>> DataRetrieved;
        #endregion

        #region Properties
        public DataPush.DataPushClient Client { get; }
        public IList<DataPullSubscriber> PullSubscribers { get; }
        public string NodeName { get; set; }
        public string DeviceId { get; set; }
        public bool IsDisposed { get; private set; }
        #endregion

        #region Constructor
        public GrpcClient(string nodeName, string deviceId, ILogger logger = null)
        {
            _logger = logger;

            NodeName = nodeName;

            DeviceId = deviceId;

            PullSubscribers = new List<DataPullSubscriber>();
        }

        public GrpcClient(ChannelBase channel, string nodeName, string deviceId, ILogger logger = null) : this(nodeName, deviceId, logger)
        {
            Client = new DataPush.DataPushClient(channel);
        }

        public GrpcClient(string connectionString, string nodeName, string deviceId, ILogger logger = null) : this(nodeName, deviceId, logger)
        {
            Client = new DataPush.DataPushClient(GrpcChannel.ForAddress(connectionString));
        }
        #endregion

        #region Methods
        public Task CreatePullSubscriberAsync()
        {
            var subscriber = PullSubscribers
                .FirstOrDefault(x => x.DestinationNode == NodeName);

            if (subscriber != null)
            {
                _logger?.Information($"Pull Subscriber for {subscriber.DestinationNode} Already Exists.");

                return Task.CompletedTask;
            }

            subscriber = new DataPullSubscriber(Client, NodeName);

            _logger?.Information($"Creating Pull Subscriber for {subscriber.DestinationNode}.");

            subscriber.DataRetrieved += OnPullDataRetrieved;

            PullSubscribers.Add(subscriber);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<string>> RegisterNodeAsync(bool isPullNode)
        {
            var nodeRequest = new RegisterNodeRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                DeviceId = DeviceId,
                NodeName = NodeName,
                IsPullNode = isPullNode,
            };

            _logger?.Information($"Creating Register Node Request for {DeviceId}. (Name: {NodeName}, IsPullNode: {isPullNode})");

            try
            {
                var response = await Client.RegisterNodeAsync(nodeRequest);

                if (response.Successful)
                {
                    _logger?.Information($"Register Node Request Successful.");
                }
                else
                {
                    _logger?.Error($"Register Node Request Not Successful.");
                }

                return response.PullNodeList;
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        private void OnPullDataRetrieved(object sender, ResponseStreamContext<DataPullResponse> e)
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

            return await PushDataAsync(destinationNode, name, data);
        }

        public async Task<bool> PushDataAsync(string destinationNode, string name, byte[] data)
        {
            _logger?.Information($"Pushing Data to {destinationNode}. (Name: {name}, Size: {data.Count()})");

            var requestId = Guid.NewGuid().ToString();

            try
            {
                var blockSize = 2500000;

                // Convert data length / block size to double, round up, and then convert back to int.
                var blockIterations = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(data.Length) / blockSize));

                for (var i = 0; i < blockIterations; i++)
                {
                    var start = i * blockSize;

                    var end = start + blockSize >= data.Length
                        ? data.Length
                        : start + blockSize;

                    var dataBlock = data[start..end];

                    _logger?.Information($"Pushing Block {i + 1} of {blockIterations} to {destinationNode} (Name: {name}, Size: {dataBlock.Count()})");

                    var request = new DataPushRequest
                    {
                        RequestId = requestId,
                        SourceNode = NodeName,
                        DestinationNode = destinationNode,
                        Name = name,
                        BlockNumber = i + 1,
                        TotalBlocks = blockIterations,
                        Payload = ByteString.CopyFrom(dataBlock)
                    };

                    await Client.PushData().RequestStream.WriteAsync(request);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"Error Pushing Data: {ex.Message})");

                return false;
            }
        }

        public void Dispose()
        {
            _logger?.Information($"Disposing Grpc Client.");

            IsDisposed = true;

            Disposed?.Invoke(this, EventArgs.Empty);

            foreach (var pullSubscriber in PullSubscribers)
            {
                pullSubscriber.Dispose();
            }

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
