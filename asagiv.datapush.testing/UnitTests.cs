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
        public void AssertRequestConfirmation(string payload)
        {
            var request = new DataPushRequest
            {
                SourceNode = "My Source",
                DestinationNode = "My Destination",
                Payload = ByteString.CopyFrom(Encoding.UTF8.GetBytes(payload))
            };

            var reply = _client.PushData(request);

            Assert.IsTrue(reply.Confirmation == 1);
        }
    }
}