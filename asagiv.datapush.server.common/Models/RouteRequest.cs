using asagiv.datapush.server.common.Interfaces;
using Google.Protobuf;
using System;

namespace asagiv.datapush.server.common.Models
{
    public class RouteRequest : IRouteRequest
    {
        #region Properties
        public string SourceNode { get; }
        public string DestinationNode { get; }
        public string Name { get; }
        public DateTime PushDateTime { get; }
        public ByteString Payload { get; }
        public bool isRouteCompleted { get; set; }
        #endregion

        #region Constructor
        public RouteRequest(string fromNode, string toNode, string name, ByteString payload)
        {
            this.SourceNode = fromNode;
            this.DestinationNode = toNode;
            this.Name = name;
            this.Payload = payload;

            this.PushDateTime = DateTime.Now;
            isRouteCompleted = false;
        }
        #endregion
    }
}
