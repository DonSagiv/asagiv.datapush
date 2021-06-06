using asagiv.datapush.server.common.Interfaces;
using Google.Protobuf;
using System;
using System.Collections.Generic;

namespace asagiv.datapush.server.common.Models
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
        public Queue<PayloadItem> PayloadQueue { get; }
        public bool IsRouteCompleted { get; set; }
        #endregion

        #region Constructor
        public RouteRequest(string sourceNode, string destinationNode, string name)
        {
            RequestId = Guid.NewGuid();
            SourceNode = sourceNode;
            DestinationNode = destinationNode;
            Name = name;
            PushDateTime = DateTime.Now;

            PayloadQueue = new Queue<PayloadItem>();
            
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
            return PayloadQueue.TryDequeue(out PayloadItem payloadToReturn)
                ? payloadToReturn
                : null;
        }
        #endregion
    }
}
