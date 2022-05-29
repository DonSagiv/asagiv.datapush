using asagiv.pushrocket.common.Interfaces;
using Grpc.Core;

namespace asagiv.pushrocket.common.Models
{
    internal class ResponseStreamContext<T> : IResponseStreamContext<T>
    {
        public T ResponseData { get; }
        public IAsyncStreamReader<T> ResponseStream { get; }

        public ResponseStreamContext(T responseData, IAsyncStreamReader<T> responseStream)
        {
            ResponseData = responseData;
            ResponseStream = responseStream;
        }
    }
}
