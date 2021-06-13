using asagiv.datapush.common;
using Google.Protobuf;
using NUnit.Framework;
using System;
using asagiv.datapush.common.Utilities;

namespace asagiv.datapush.testing
{
    public class UnitTests
    {
        [SetUp]
        public void Setup()
        {
            // Do nothing for now.
        }

        [Test]

        public void ShouldSuccessfullyCreateDataPushRequest(Guid requestId,
            string name, 
            int blockNumber, 
            int totalBlocks, 
            string destinationNode,
            string sourceNode,
            object payload)
        {
            Assert.DoesNotThrow(() => 
            {
                _ = new DataPushRequest
                {
                    RequestId = requestId.ToString(),
                    Name = name,
                    BlockNumber = blockNumber,
                    TotalBlocks = totalBlocks,
                    DestinationNode = destinationNode,
                    SourceNode = sourceNode,
                    Payload = ByteString.CopyFrom(payload.ToByteArray()),
                };
            });
        }
    }
}