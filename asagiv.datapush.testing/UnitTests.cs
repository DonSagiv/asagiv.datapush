using asagiv.datapush.common;
using Google.Protobuf;
using Grpc.Net.Client;
using NUnit.Framework;
using System.Text;

namespace asagiv.datapush.testing
{
    public class UnitTests
    {
        private GrpcChannel _channel;
        private DataPush.DataPushClient _client;

        [SetUp]
        public void Setup()
        {
            _channel = GrpcChannel.ForAddress("http://localhost:80");
            _client = new DataPush.DataPushClient(_channel);
        }

        [TestCase("Hello World!")]
        [TestCase("")]
        [TestCase(null)]
        public void AssertRequestConfirmation(string payload)
        {
            var context = new PushDataContext
            {
                topic = "Hello World!",
                data = Encoding.UTF8.GetBytes(payload)
            };

            var request = new DataPushRequest
            {
                Topic = context.topic,
                Data = ByteString.CopyFrom(context.data)
            };

            var reply = _client.PushData(request);

            Assert.IsTrue(reply.Confirmation == 1);

            var replyData = reply.Data.ToStringUtf8();

            Assert.IsTrue(replyData == payload);
        }
    }
}