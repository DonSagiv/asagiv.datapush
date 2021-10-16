using asagiv.datapush.common;
using asagiv.datapush.server.common.Models;
using asagiv.datapush.server.Interfaces;
using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Concurrent;

namespace asagiv.datapush.server.Models
{
    public class RouteRequest : IRouteRequest
    {
        #region Properties
        public Guid RequestId { get; }
        public string SourceNode { get; }
        public string DestinationNode { get; }
        public string Name { get; }
        public int BlocksRetrieved { get; private set; }
        public int TotalBlocks { get; private set; }
        public DateTime PushDateTime { get; }
        public ConcurrentQueue<PayloadItem> PayloadQueue { get; }
        public string ErrorMessage { get; private set; }
        public bool IsRouteConnected { get; set; }
        public bool IsRouteCompleted => BlocksRetrieved >= TotalBlocks;
        public bool IsRouteErrorRaised => !string.IsNullOrWhiteSpace(ErrorMessage);
        public IServerStreamWriter<DataPushResponse> ResponseStream { get; }
        #endregion

        #region Constructor
        public RouteRequest(DataPushRequest dataPushRequest, IServerStreamWriter<DataPushResponse> responseStream)
        {
            RequestId = Guid.TryParse(dataPushRequest.RequestId, out var requestId)
                ? requestId
                : Guid.NewGuid();

            SourceNode = dataPushRequest.SourceNode;
            DestinationNode = dataPushRequest.DestinationNode;
            Name = dataPushRequest.Name;
            TotalBlocks = dataPushRequest.TotalBlocks;
            PushDateTime = DateTime.Now;

            ResponseStream = responseStream;

            PayloadQueue = new ConcurrentQueue<PayloadItem>();
            
            BlocksRetrieved = 0;
        }
        #endregion

        #region Methods
        public PayloadItem AddPayload(int blockNumber, ByteString payloadByteString)
        {
            var payloadItemToAdd = new PayloadItem(blockNumber, payloadByteString);

            PayloadQueue.Enqueue(payloadItemToAdd);

            return payloadItemToAdd;
        }

        public PayloadItem GetFromPayload()
        {
            var payload = PayloadQueue.TryDequeue(out var payloadToReturn)
                ? payloadToReturn
                : null;

            if(payload != null)
            {
                BlocksRetrieved++;
            }

            return payload;
        }

        public void SetRouteError(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
        #endregion
    }
}
