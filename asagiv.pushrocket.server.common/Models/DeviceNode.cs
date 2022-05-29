using asagiv.pushrocket.server.common.Interfaces;

namespace asagiv.pushrocket.server.common.Models
{
    public class DeviceNode : IDeviceNode
    {
        #region Properties
        public string NodeName { get; set; }
        public string DeviceId { get; set; }
        public bool IsPullNode { get; set; }
        #endregion

        #region Constructor
        public DeviceNode(string nodeName, string deviceId, bool isPullNode)
        {
            NodeName = nodeName;
            DeviceId = deviceId;
            IsPullNode = isPullNode;
        }
        #endregion
    }
}
