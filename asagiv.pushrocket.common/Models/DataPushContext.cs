using asagiv.pushrocket.common.Interfaces;
using asagiv.common.Extensions;
using Google.Protobuf;
using Grpc.Core;
using Serilog;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Models
{
    public class DataPushContext : IDataPushContext, INotifyPropertyChanged
    {
        #region Statics
        public const int blockSize = 2500000;
        #endregion

        #region Fields
        private readonly ILogger _logger;
        private readonly DataPush.DataPushClient _client;
        private readonly Subject<Unit> _onDataPushSubject = new();
        private readonly Subject<int> _onPushResponseReceived = new();
        private IDisposable _pushDataDisposable;
        private DeliveryStatus _status;
        #endregion

        #region Delegates
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public Guid RequestId { get; }
        public string SourceNode { get; }
        public string DestinationNode { get; }
        public string Name { get; }
        public Stream Payload { get; }
        public string Description => $"{Name} to {DestinationNode}";
        public int NumberOfBlocksPushed { get; private set; }
        public int TotalNumberOfBlocks { get; private set; }
        public DeliveryStatus Status
        {
            get => _status;
            private set { _status = value; RaisePropertyChanged(nameof(Status)); }
        }
        public IObservable<int> OnPushResponseReceived => _onPushResponseReceived.AsObservable();
        #endregion

        #region Constructor
        public DataPushContext(DataPush.DataPushClient client, string sourceNode, string destinationNode, string name, Stream payload, ILogger logger = null)
        {
            SourceNode = sourceNode;
            DestinationNode = destinationNode;
            Name = name;
            Payload = payload;

            _client = client;
            _logger = logger;

            RequestId = Guid.NewGuid();

            Status = DeliveryStatus.Pending;
        }
        #endregion

        #region Methods
        public async Task<bool> PushDataAsync()
        {
            _logger?.Information($"Pushing Data to {DestinationNode}. (Name: {Name}, Size: {Payload.Length})");

            try
            {
                Status = DeliveryStatus.InProgress;

                // Convert data length / block size to double, round up, and then convert back to int.
                TotalNumberOfBlocks = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Payload.Length) / blockSize));

                var streamDuplex = _client.PushData();

                _pushDataDisposable = _onDataPushSubject
                    .SelectMany(_ => HandleDataPushResponse(streamDuplex))
                    .Subscribe();

                var blocks = Enumerable.Range(0, TotalNumberOfBlocks)
                    .Select(x => GetPayloadPushRequest(x))
                    .ToAsync();

                await foreach (var block in blocks)
                {
                    await PushBlockAsync(block, streamDuplex);
                }

                return await ConfirmDeliveryAsync();
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"Error Pushing Data: {ex.Message})");

                Status = DeliveryStatus.Failed;

                return false;
            }
        }

        private async Task<DataPushRequest> GetPayloadPushRequest(int blockNumber)
        {
            // Get the start index of the payload byte array.
            var start = blockNumber * blockSize;

            // Get the end index of the payload byte array.
            var length = blockSize >= Payload.Length
                ? Payload.Length - start // In case the index exceeds the payload length
                : blockSize;

            // Get the data block from the payload.
            var dataBlock = new byte[length];

            await Payload.ReadAsync(dataBlock);

            // Return the data push request with the payload block.
            return new DataPushRequest
            {
                RequestId = RequestId.ToString(),
                SourceNode = SourceNode,
                DestinationNode = DestinationNode,
                Name = Name,
                BlockNumber = blockNumber + 1,
                TotalBlocks = TotalNumberOfBlocks,
                Payload = ByteString.CopyFrom(dataBlock)
            };
        }

        private async Task<Unit> PushBlockAsync(DataPushRequest request, AsyncDuplexStreamingCall<DataPushRequest, DataPushResponse> streamDuplex)
        {
            _logger?.Information($"Pushing Block: {request.BlockNumber})");

            try
            {
                // Add the request to the stream.
                await streamDuplex.RequestStream.WriteAsync(request);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }

            _onDataPushSubject?.OnNext(Unit.Default);

            return Unit.Default;
        }

        private async Task<Unit> HandleDataPushResponse(AsyncDuplexStreamingCall<DataPushRequest, DataPushResponse> streamDuplex)
        {
            // Get the next item from the response stream.
            if (!await streamDuplex.ResponseStream.MoveNext(CancellationToken.None))
            {
                return Unit.Default;
            }

            var response = streamDuplex.ResponseStream.Current;

            // Check if response has an error.
            if (response.Confirmation < 0)
            {
                _logger?.Information($"Retrieved Error: {response.ErrorMessage})");

                // Stop streaming the data.
                _pushDataDisposable.Dispose();

                return Unit.Default;
            }

            _logger?.Information($"Retrieved Confirmation for Block: {response.BlockNumber})");

            // Notify that a block has successfully been pushed.
            _onPushResponseReceived?.OnNext(++NumberOfBlocksPushed);

            // Update the data push progress.
            return Unit.Default;
        }

        private async Task<bool> ConfirmDeliveryAsync()
        {
            // Attempt 100 times (~10 seconds) to see if delivery status is available.
            for (var i = 0; i < 100; i++)
            {
                // Request delivery status from server.
                var request = new ConfirmDeliveryRequest
                {
                    RequestId = RequestId.ToString(),
                    Name = Name,
                    DestinationNode = DestinationNode,
                };

                // Await response from the server.
                var response = await _client.ConfirmDeliveryAsync(request);

                if (response.IsRouteCompleted)
                {
                    // If the route is completed, determine if the delivery is successful.
                    Status = response.IsDeliverySuccessful ? DeliveryStatus.Successful : DeliveryStatus.Failed;

                    return response.IsDeliverySuccessful;
                }

                // If not, wait 100 milliseconds and try again.
                Thread.Sleep(100);
            }

            // If fails after 100 times, make staus failed.
            Status = DeliveryStatus.Failed;

            return false;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
