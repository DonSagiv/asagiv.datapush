using asagiv.datapush.server.common.Models;
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
        int TotalBlocks { get; set; }
        DateTime PushDateTime { get; }
        Queue<PayloadItem> PayloadQueue { get; }
        bool IsRouteCompleted { get; set; }

        void AddPayload(int blockNumber, ByteString payloadItemToAdd);
        PayloadItem GetFromPayload();
    }
}
