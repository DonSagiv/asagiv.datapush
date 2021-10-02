using asagiv.datapush.server.common.Interfaces;
using asagiv.datapush.server.common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace asagiv.datapush.testing
{
    public class NodeRepositoryTest
    {
        // Tests
        [Fact]
        public void GetNewNodeEmptyRepository()
        {
            // Arrange -- createEmptyRepository.
            var nodeName = "Test Node";
            var repository = createEmptyRepository();

            // Action -- "Get" a node from a repository
            var node = repository.GetNode(nodeName, Guid.NewGuid().ToString(), false);

            // Assertion -- Retrieved node is not null.
            Assert.NotNull(node);

            // Assertion -- Returned node is the same as the input nodename.
            Assert.True(node.NodeName == nodeName);

            // Assertion -- Number of nodes in repository is 1.
            Assert.True(repository.Nodes.Count() == 1);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(7)]
        public void GetNewNodePopulatedRepository(int numNodes)
        {
            // -- Arrange

            // Create pre-populated node
            var repository = createPopulatedRepository(numNodes);

            // Use "Device 2", populate different node, pull node status
            var newNodeName = "Test 2 New";
            var deviceName = "Device 2";
            var newIsPullNode = false;

            // -- Action

            //"Get" a node from the repository. Device is the same, but name and IsPullNode are different.
            var node = repository.GetNode(newNodeName, deviceName, newIsPullNode);

            // -- Assert

            // Returned node is not null.
            Assert.NotNull(node);

            // Since this device already exists, number of nodes should be the same.
            Assert.True(repository.Nodes.Count() == numNodes);

            // The returned node name is the new node name.
            Assert.True(node.NodeName == newNodeName);

            // The returned node device ID is the queried device ID.
            Assert.True(node.DeviceId == deviceName);

            // The return node is a not a pull.
            Assert.True(node.IsPullNode == newIsPullNode);
        }

        // Setup Methods
        private INodeRepository createEmptyRepository()
        {
            return new NodeRepository();
        }

        private INodeRepository createPopulatedRepository(int numberOfNodes = 3)
        {
            var repository = createEmptyRepository();

            // Create nodes
            for (var i = 0; i < numberOfNodes; i++)
            {
                repository.GetNode($"Test {i + 1}", $"Device {i + 1}", i % 2 == 0);
            }

            return repository;
        }
    }
}
