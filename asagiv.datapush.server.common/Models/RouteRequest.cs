using asagiv.datapush.server.common.Interfaces;
using Google.Protobuf;
using System;
using System.Collections.Generic;

namespace asagiv.datapush.server.common.Models
{
    public class RouteRequest : IRouteRequest
    {
        #region Properties
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
            this.SourceNode = sourceNode;
            this.DestinationNode = destinationNode;
            this.Name = name;

            PayloadQueue = new Queue<ByteString>();

            this.PushDateTime = DateTime.Now;
            isRouteCompleted = false;
        }

        public void AddToPayload(ByteString payloadItemToAdd)
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
