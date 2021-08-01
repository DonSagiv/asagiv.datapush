using System;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Interfaces
{
    public interface IDataPushContext
    {
        #region Properties
        Guid RequestId { get; }
        string SourceNode { get; }
        string DestinationNode { get; }
        string Name { get; }
        byte[] Payload { get; }
        string Description { get; }
        int NumberOfBlocks { get; }
        double DataPushProgress { get; }
        #endregion

        #region Methods
        Task<bool> PushDataAsync();
        #endregion
    }
}
