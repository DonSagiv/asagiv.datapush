﻿namespace asagiv.pushrocket.server.common.Interfaces
{
    public interface IDeviceNode
    {
        string NodeName { get; set; }
        string DeviceId { get; set; }
        bool IsPullNode { get; set; }
    }
}
