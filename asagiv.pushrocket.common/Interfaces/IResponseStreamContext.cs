using Grpc.Core;

namespace asagiv.pushrocket.common.Interfaces
{
    public interface IResponseStreamContext<out T>
    {
        T ResponseData { get; }
        IAsyncStreamReader<T> ResponseStream { get; }
    }
}
