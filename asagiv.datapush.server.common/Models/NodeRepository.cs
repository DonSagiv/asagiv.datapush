using asagiv.datapush.server.common.Interfaces;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace asagiv.datapush.server.common.Models
{
    public class NodeRepository : INodeRepository
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly IList<IDeviceNode> _nodeList;
        #endregion

        #region Properties
        public IEnumerable<IDeviceNode> Nodes => _nodeList;
        public IEnumerable<IDeviceNode> PullNodes => _nodeList.Where(x => x.IsPullNode);
        #endregion

        #region Constructor
        public NodeRepository(ILogger logger = null)
        {
            _nodeList = new List<IDeviceNode>();
            _logger = logger;

            _logger?.Debug("Node Repository Instantiated.");
        }

        /// <summary>
        /// Get a node from the repository
        /// </summary>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="deviceId">Device ID of the node.</param>
        /// <param name="isPullNode">True of node can pull data.</param>
        /// <returns>The node to return.</returns>
        public IDeviceNode GetNode(string nodeName, string deviceId, bool isPullNode)
        {
            // Find if the node with the device ID already existrs.
            var node = _nodeList.FirstOrDefault(x => x.DeviceId == deviceId);

            if (node == null)
            {
                // If not, create a new node.
                node = new DeviceNode(nodeName, deviceId, isPullNode);

                // Add that node to the list.
                _nodeList.Add(node);

                _logger?.Information($"Added New Node to Repository. (Node Name: {nodeName}, Device ID: {deviceId}, Is Pull Node: {isPullNode})");
            }
            else
            {
                // If yes, update the node name, and whether or not it's a pull node.
                node.NodeName = nodeName;

                node.IsPullNode = isPullNode;

                _logger?.Information($"Set Node {deviceId} to {nodeName}.");
            }
            
            return node;
        }
        #endregion
    }
}
