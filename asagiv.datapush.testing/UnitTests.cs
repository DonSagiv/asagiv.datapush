using asagiv.datapush.common;
using Google.Protobuf;
using NUnit.Framework;
using System;

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
                    Name = name,
                    BlockNumber = blockNumber,
                    TotalBlocks = totalBlocks,
                    DestinationNode = destinationNode,
                    SourceNode = sourceNode,
                    Payload = payload,
                    RequestId = requestId,
                };
            });
        }
    }
}