using Google.Protobuf;
using System;

namespace asagiv.datapush.server.common.Interfaces
{
    public interface IPushRepositoryItem
    {
        string Topic { get; }
        DateTime PushDateTime { get; }
        ByteString Data { get; }
    }
}
