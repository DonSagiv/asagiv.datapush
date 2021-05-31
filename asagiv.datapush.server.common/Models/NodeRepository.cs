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
        public NodeRepository(ILogger logger)
        {
            _nodeList = new List<IDeviceNode>();
            _logger = logger;

            _logger?.Debug("Node Repository Instantiated.");
        }

        public IDeviceNode GetNode(string nodeName, string deviceId, bool isPullNode)
        {
            var node = _nodeList.FirstOrDefault(x => x.DeviceId == deviceId);

            if (node == null)
            {
                node = new DeviceNode(nodeName, deviceId, isPullNode);

                _nodeList.Add(node);

                _logger?.Information($"Added New Node to Repository. (Node Name: {nodeName}, Device ID: {deviceId}, Is Pull Node: {isPullNode})");
            }
            else
            {
                node.NodeName = nodeName;

                node.IsPullNode = isPullNode;

                _logger?.Information($"Set Node {deviceId} to {nodeName}.");
            }

            return node;
        }
        #endregion
    }
}
