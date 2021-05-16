using System.Collections.Generic;

namespace asagiv.datapush.server.common.Interfaces
{
    public interface INodeRepository
    {
        IDictionary<string, string> nodeDictionary { get; }
    }
}
