using asagiv.datapush.server.common.Interfaces;
using asagiv.datapush.server.common.Models;
using System;
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

            // Assertion -- Node has been added to the list of nodes.
            Assert.True(repository.Nodes.Count() == 1);
            Assert.True(repository.Nodes.FirstOrDefault().NodeName == nodeName);
        }

        // Setup Methods
        private INodeRepository createEmptyRepository()
        {
            return new NodeRepository();
        }
    }
}
