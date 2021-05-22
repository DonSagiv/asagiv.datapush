using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace asagiv.datapush.common.Utilities
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
