using asagiv.datapush.server.common.Models;
using Google.Protobuf;
using System;
using System.Collections.Concurrent;

namespace asagiv.datapush.server.Interfaces
{
    public interface IRouteRequest
    {
        Guid RequestId { get; }
        string SourceNode { get; }
        string DestinationNode { get; }
        string Name { get; }
        int TotalBlocks { get; }
        DateTime PushDateTime { get; }
        ConcurrentQueue<PayloadItem> PayloadQueue { get; }
        bool IsRouteCompleted { get; set; }

        void AddPayload(int blockNumber, ByteString payloadItemToAdd);
        PayloadItem GetFromPayload();
    }
}
