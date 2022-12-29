using asagiv.pushrocket.common.Interfaces;
using Grpc.Net.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Models
{
    public sealed class GrpcClient : IGrpcClient
    {
        #region Statics
        public const string timeoutMessage = "Connection to Server Timed Out";
        #endregion

        #region Fields
        private readonly ILogger _logger;
        private readonly IClientConnectionSettings _clientConnectionSettings;
        private readonly Subject<IResponseStreamContext<DataPullResponse>> _dataRetrievedSubject = new();
        #endregion

        #region Delegates
        public event EventHandler Disposed;
        #endregion

        #region Properties
        public DataPush.DataPushClient Client { get; }
        public IList<IDataPullSubscriber> PullSubscribers { get; }
        public string DeviceId { get; set; }
        public bool IsDisposed { get; private set; }
        public IObservable<IResponseStreamContext<DataPullResponse>> DataRetrievedObservable => _dataRetrievedSubject.AsObservable();
        #endregion

        #region Constructor
        public GrpcClient(IClientConnectionSettings clientConnectionSettings, string deviceId, ILogger logger = null) : this(deviceId, logger)
        {
            _clientConnectionSettings = clientConnectionSettings;

            Client = new DataPush.DataPushClient(GrpcChannel.ForAddress(clientConnectionSettings.ConnectionString));
        }

        public GrpcClient(IClientConnectionSettings clientConnectionSettings, GrpcChannel channel, string deviceId, ILogger logger = null) : this(deviceId, logger)
        {
            _clientConnectionSettings = clientConnectionSettings;

            Client = new DataPush.DataPushClient(channel);
        }

        private GrpcClient(string deviceId, ILogger logger = null)
        {
            _logger = logger;

            DeviceId = deviceId;

            PullSubscribers = new List<IDataPullSubscriber>();
        }
        #endregion

        #region Methods
        public Task CreatePullSubscriberAsync()
        {
            // Check and see if pull subscriber for node exists.
            var subscriber = PullSubscribers
                .FirstOrDefault(x => x.DestinationNode == _clientConnectionSettings.NodeName);

            if (subscriber is not null)
            {
                _logger?.Information($"Pull Subscriber for {subscriber.DestinationNode} Already Exists.");

                return Task.CompletedTask;
            }

            // Create a new pull subscriber.
            subscriber = new DataPullSubscriber(Client, _clientConnectionSettings.NodeName);

            subscriber.DataRetrievedObservable.Subscribe(_dataRetrievedSubject.OnNext);

            _logger?.Information($"Creating Pull Subscriber for {subscriber.DestinationNode}.");

            PullSubscribers.Add(subscriber);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<string>> RegisterNodeAsync(bool isPullNode)
        {
            _logger?.Information($"Creating Register Node Request for {DeviceId}. (Name: {_clientConnectionSettings.NodeName}, IsPullNode: {isPullNode})");

            // Wait 5 seconds to establish connection, otherwise give up.
            var getPullNodeTask = GetPullNodeListAsync(isPullNode);
            var timeoutTask = Task.Delay(5000);

            await Task.WhenAny(getPullNodeTask, timeoutTask);

            if (!getPullNodeTask.IsCompletedSuccessfully)
            {
                // Throw a time-out exception if it takes longer than 5 seconds to connect.
                throw new TimeoutException(timeoutMessage);
            }

            return getPullNodeTask.Result.PullNodeList;
        }

        public async Task<RegisterNodeResponse> GetPullNodeListAsync(bool isPullNode)
        {
            var nodeRequest = new RegisterNodeRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                DeviceId = DeviceId,
                NodeName = _clientConnectionSettings.NodeName,
                IsPullNode = isPullNode,
            };

            return await Client.RegisterNodeAsync(nodeRequest);
        }

        public async Task<IDataPushContext> CreatePushFileContextAsync(string destinationNode, string filePath, Task<Stream> stream)
        {
            var streamResult = await stream;

            return CreatePushDataContext(destinationNode, filePath, streamResult);
        }

        public IDataPushContext CreatePushDataContext(string destinationNode, string name, Stream stream)
        {
            return new DataPushContext(Client, _clientConnectionSettings.NodeName, destinationNode, name, stream, _logger);
        }

        public void Dispose()
        {
            _logger?.Information("Disposing Grpc Client.");

            IsDisposed = true;

            Disposed?.Invoke(this, EventArgs.Empty);

            foreach (var pullSubscriber in PullSubscribers)
            {
                pullSubscriber.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        public async Task<Unit> AcknowledgeDeliveryAsync(AcknowledgeDeliveryRequest acknowledgeDataPullRequest)
        {
            await Client.AcknowledgeDeliveryAsync(acknowledgeDataPullRequest);

            return Unit.Default;
        }
        #endregion
    }
}