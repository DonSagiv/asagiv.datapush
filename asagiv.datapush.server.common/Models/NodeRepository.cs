using asagiv.datapush.server.common.Interfaces;
using Serilog;
using System.Collections.Generic;

namespace asagiv.datapush.server.common.Models
{
    public class NodeRepository : INodeRepository
    {
        #region Fields
        private readonly ILogger _logger;
        #endregion

        #region Properties
        public IList<IDeviceNode> nodeList { get; }
        #endregion

        #region Constructor
        public NodeRepository(ILogger logger)
        {
            nodeList = new List<IDeviceNode>();
            _logger = logger;

            _logger.Debug("Node Repository Instantiated.");
        }
        #endregion
    }
}
