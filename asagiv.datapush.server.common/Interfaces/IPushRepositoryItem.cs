using Google.Protobuf;

namespace asagiv.datapush.server.common.Interfaces
{
    public interface IPushRepositoryItem
    {
        string Topic { get; }
        ByteString Data { get; }
    }
}
