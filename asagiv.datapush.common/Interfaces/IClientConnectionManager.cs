using System;
using System.Collections.Generic;

namespace asagiv.datapush.common.Interfaces
{
    public interface IClientConnectionManager
    {
        IList<string> ConnectionStringList { get; }
        string DeviceName { get; }
    }
}
