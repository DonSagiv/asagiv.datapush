using asagiv.datapush.common.Interfaces;
using Grpc.Core;
using Grpc.Net.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Models
{
    public sealed class GrpcClient : IGrpcClient
    {
        #region Fields
        private readonly ILogger _logger;
        #endregion

        #region Delegates
        public event EventHandler Disposed;
        public event EventHandler<IResponseStreamContext<DataPullResponse>> DataRetrieved;
        #endregion

        #region Properties
        public DataPush.DataPushClient Client { get; }
        public IList<IDataPullSubscriber> PullSubscribers { get; }
        public string NodeName { get; set; }
        public string DeviceId { get; set; }
        public bool IsDisposed { get; private set; }
        #endregion

        #region Constructor
        public GrpcClient(ChannelBase channel, string nodeName, string deviceId, ILogger logger = null) : this(nodeName, deviceId, logger)
        {
            Client = new DataPush.DataPushClient(channel);
        }

        public GrpcClient(string connectionString, string nodeName, string deviceId, ILogger logger = null) : this(nodeName, deviceId, logger)
        {
            Client = new DataPush.DataPushClient(GrpcChannel.ForAddress(connectionString));
        }

        private GrpcClient(string nodeName, string deviceId, ILogger logger = null)
        {
            _logger = logger;

            NodeName = nodeName;

            DeviceId = deviceId;

            PullSubscribers = new List<IDataPullSubscriber>();
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

        private void OnPullDataRetrieved(object sender, IResponseStreamContext<DataPullResponse> e)
        {
            DataRetrieved?.Invoke(sender, e);
        }

        public async Task<IDataPushContext> CreatePushFileContextAsync(string destinationNode, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return null;
            }

            var data = await File.ReadAllBytesAsync(filePath);

            var name = Path.GetFileName(filePath);

            return CreatePushDataContext(destinationNode, name, data);
        }

        public IDataPushContext CreatePushDataContext(string destinationNode, string name, byte[] data)
        {
            return new DataPushContextBase(Client, NodeName, destinationNode, name, data);
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
