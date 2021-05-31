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
        public DateTime PushDateTime { get; }
        public Queue<ByteString> PayloadQueue { get; }
        public bool isRouteCompleted { get; set; }
        #endregion

        #region Constructor
        public RouteRequest(string sourceNode, string destinationNode, string name)
        {
            RequestId = Guid.NewGuid();
            SourceNode = sourceNode;
            DestinationNode = destinationNode;
            Name = name;
            PushDateTime = DateTime.Now;

            PayloadQueue = new Queue<ByteString>();
            
            isRouteCompleted = false;
        }

        public void AddPayload(ByteString payloadItemToAdd)
        {
            PayloadQueue.Enqueue(payloadItemToAdd);
        }

        public ByteString GetFromPayload()
        {
            return PayloadQueue.TryDequeue(out ByteString payloadToReturn)
                ? payloadToReturn
                : ByteString.Empty;
        }
        #endregion
    }
}
