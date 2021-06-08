using asagiv.datapush.server.common.Models;
using asagiv.datapush.server.Interfaces;
using Google.Protobuf;
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
        public int TotalBlocks { get; set; }
        public DateTime PushDateTime { get; }
        public ConcurrentQueue<PayloadItem> PayloadQueue { get; }
        public bool IsRouteCompleted { get; set; }
        #endregion

        #region Constructor
        public RouteRequest(DataPushRequest dataPushRequest)
        {
            RequestId = Guid.TryParse(dataPushRequest.RequestId, out var requestId)
                ? requestId
                : Guid.NewGuid();

            SourceNode = dataPushRequest.SourceNode;
            DestinationNode = dataPushRequest.DestinationNode;
            Name = dataPushRequest.Name;
            TotalBlocks = dataPushRequest.TotalBlocks;
            PushDateTime = DateTime.Now;

            PayloadQueue = new ConcurrentQueue<PayloadItem>();
            
            IsRouteCompleted = false;
        }
        #endregion

        #region Methods
        public void AddPayload(int blockNumber, ByteString payloadItemToAdd)
        {
            PayloadQueue.Enqueue(new PayloadItem(blockNumber, payloadItemToAdd));
        }

        public PayloadItem GetFromPayload()
        {
            return PayloadQueue.TryDequeue(out var payloadToReturn)
                ? payloadToReturn
                : null;
        }
        #endregion
    }
}
