using System.Collections.Generic;

namespace asagiv.pushrocket.common.Interfaces
{
    public interface IClientConnectionManager
    {
        IList<string> ConnectionStringList { get; }
        string DeviceName { get; }
    }
}
