using asagiv.datapush.server.common.Interfaces;
using System.Collections.Generic;

namespace asagiv.datapush.server.common.Models
{
    public class NodeRepository : INodeRepository
    {
        #region Properties
        public IDictionary<string, string> nodeDictionary { get; }
        #endregion

        #region Constructor
        public NodeRepository()
        {
            nodeDictionary = new Dictionary<string, string>();
        }
        #endregion
    }
}
