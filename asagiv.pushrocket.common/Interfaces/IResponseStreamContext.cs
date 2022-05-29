using Grpc.Core;

namespace asagiv.pushrocket.common.Interfaces
{
    public interface IResponseStreamContext<T>
    {
        T ResponseData { get; }
        IAsyncStreamReader<T> ResponseStream { get; }
    }
}
