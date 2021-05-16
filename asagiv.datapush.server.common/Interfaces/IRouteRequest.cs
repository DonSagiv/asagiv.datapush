using Google.Protobuf;
using System;

namespace asagiv.datapush.server.common.Interfaces
{
    public interface IRouteRequest
    {
        string SourceNode { get; }
        string DestinationNode { get; }
        string Name { get; }
        DateTime PushDateTime { get; }
        ByteString Payload { get; }
        bool isRouteCompleted { get; set; }
    }
}
