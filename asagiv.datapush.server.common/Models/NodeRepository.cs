using asagiv.datapush.server.common.Interfaces;
using System.Collections.Generic;

namespace asagiv.datapush.server.common.Models
{
    public class NodeRepository : INodeRepository
    {
        #region Properties
        public IList<IDeviceNode> nodeList { get; }
        #endregion

        #region Constructor
        public NodeRepository()
        {
            nodeList = new List<IDeviceNode>();
        }
        #endregion
    }
}
