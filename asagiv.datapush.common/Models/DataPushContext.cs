using asagiv.datapush.common.Interfaces;
using Google.Protobuf;
using Grpc.Core;
using Serilog;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Models
{
    public class DataPushContext : INotifyPropertyChanged
    {
        #region Statics
        public const int blockSize = 2500000;
        #endregion

        #region Fields
        private readonly ILogger _logger;
        private readonly DataPush.DataPushClient _client;
        private Subject<Unit> _onDataPushSubject = new Subject<Unit>();
        private Subject<double> _onPushResponseReceived = new Subject<double>();
        #endregion

        #region Delegates
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public Guid RequestId { get; }
        public string SourceNode { get; }
        public string DestinationNode { get; }
        public string Name { get; }
        public byte[] Payload { get; }
        public string Description => $"{Name} to {DestinationNode}";
        public int NumberOfBlocksPushed { get; private set; }
        public int TotalNumberOfBlocks { get; private set; }
        public IObservable<double> OnPushResponseReceived => _onPushResponseReceived.AsObservable();
        #endregion

        #region Constructor
        public DataPushContext(DataPush.DataPushClient client, string sourceNode, string destinationNode, string name, byte[] payload, ILogger logger = null)
        {
            SourceNode = sourceNode;
            DestinationNode = destinationNode;
            Name = name;
            Payload = payload;

            _client = client;
            _logger = logger;

            RequestId = Guid.NewGuid();
        }
        #endregion

        #region Methods
        public async Task<bool> PushDataAsync()
        {
            _logger?.Information($"Pushing Data to {DestinationNode}. (Name: {Name}, Size: {Payload.Length})");

            try
            {
                // Convert data length / block size to double, round up, and then convert back to int.
                TotalNumberOfBlocks = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Payload.Length) / blockSize));

                var streamDuplex = _client.PushData();

                _onDataPushSubject
                    .SelectMany(x => HandleDataPushResponse(streamDuplex))
                    .Subscribe();

                var blocks = Enumerable.Range(0, TotalNumberOfBlocks)
                    .Select(x => GetPayloadPushRequest(x));

                foreach (var block in blocks)
                {
                    await PushBlockAsync(block, streamDuplex);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"Error Pushing Data: {ex.Message})");

                return false;
            }
        }

        private DataPushRequest GetPayloadPushRequest(int blockNumber)
        {
            // Get the start index of the payload byte array.
            var start = blockNumber * blockSize;

            // Get the end index of the payload byte array.
            var end = start + blockSize >= Payload.Length
                ? Payload.Length // In case the index exceeds the payload length
                : start + blockSize;

            // Get the data block from the payload.
            var dataBlock = Payload[start..end];

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

            _logger?.Information($"Retrieved Confirmation for Block: {response.BlockNumber})");

            var progress = ++NumberOfBlocksPushed / Convert.ToDouble(TotalNumberOfBlocks);

            _onPushResponseReceived?.OnNext(progress);

            // Update the data push progress.
            return Unit.Default;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
