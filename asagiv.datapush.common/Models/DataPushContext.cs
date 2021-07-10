using Google.Protobuf;
using Serilog;
using System;
using System.ComponentModel;
using System.Linq;
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
        private int _numberOfBlocks;
        private double _dataPushProgress;
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
        public int NumberOfBlocks
        {
            get { return _numberOfBlocks; }
            set { _numberOfBlocks = value; RaisePropertyChanged(nameof(NumberOfBlocks)); }
        }
        public double DataPushProgress
        {
            get { return _dataPushProgress; }
            set { _dataPushProgress = value; RaisePropertyChanged(nameof(DataPushProgress)); }
        }
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

            DataPushProgress = 0;

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
                NumberOfBlocks = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Payload.Length) / blockSize));

                for (var i = 0; i < NumberOfBlocks; i++)
                {
                    await PushBlockAsync(i);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"Error Pushing Data: {ex.Message})");

                return false;
            }
        }

        private async Task PushBlockAsync(int iteration)
        {
            var start = iteration * blockSize;

            var end = start + blockSize >= Payload.Length
                ? Payload.Length
                : start + blockSize;

            var dataBlock = Payload[start..end];

            _logger?.Information($"Pushing Block {iteration + 1} of {_numberOfBlocks} to {DestinationNode} (Name: {Name}, Size: {dataBlock.Count()})");

            var request = new DataPushRequest
            {
                RequestId = RequestId.ToString(),
                SourceNode = SourceNode,
                DestinationNode = DestinationNode,
                Name = Name,
                BlockNumber = iteration + 1,
                TotalBlocks = NumberOfBlocks,
                Payload = ByteString.CopyFrom(dataBlock)
            };

            await _client.PushData().RequestStream.WriteAsync(request);

            DataPushProgress = Convert.ToDouble(iteration + 1) / _numberOfBlocks;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
