using System.Collections.Generic;

namespace asagiv.datapush.server.common.Interfaces
{
    public interface INodeRepository
    {
        IEnumerable<IDeviceNode> Nodes { get; }
        IEnumerable<IDeviceNode> PullNodes { get; }

        IDeviceNode GetNode(string nodeName, string deviceId, bool isPullNode);
    }
}
