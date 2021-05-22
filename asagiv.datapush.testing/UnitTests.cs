using asagiv.datapush.common;
using Grpc.Net.Client;
using NUnit.Framework;

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

        }
    }
}