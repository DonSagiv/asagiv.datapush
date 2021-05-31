using Google.Protobuf;
using System;
using System.Collections.Generic;

namespace asagiv.datapush.server.common.Interfaces
{
    public interface IRouteRequest
    {
        string SourceNode { get; }
        string DestinationNode { get; }
        string Name { get; }
        DateTime PushDateTime { get; }
        Queue<ByteString> PayloadQueue { get; }
        bool isRouteCompleted { get; set; }

        void AddPayload(ByteString payloadItemToAdd);
        ByteString GetFromPayload();
    }
}
