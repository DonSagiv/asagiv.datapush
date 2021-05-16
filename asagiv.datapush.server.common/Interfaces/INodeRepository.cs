using System.Collections.Generic;

namespace asagiv.datapush.server.common.Interfaces
{
    public interface INodeRepository
    {
        IList<IDeviceNode> nodeList { get; }
    }
}
