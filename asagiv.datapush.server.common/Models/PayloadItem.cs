using Google.Protobuf;

namespace asagiv.datapush.server.common.Models
{
    public record PayloadItem(int BlockNumber, ByteString Payload);
}
