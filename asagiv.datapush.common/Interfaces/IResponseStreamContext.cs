using Grpc.Core;

namespace asagiv.datapush.common.Interfaces
{
    public interface IResponseStreamContext<T>
    {
        T ResponseData { get; }
        IAsyncStreamReader<T> ResponseStream { get; }
    }
}
