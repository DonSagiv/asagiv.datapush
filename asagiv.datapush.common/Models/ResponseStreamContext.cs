using Grpc.Core;

namespace asagiv.datapush.common.Models
{
    public class ResponseStreamContext<T>
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
