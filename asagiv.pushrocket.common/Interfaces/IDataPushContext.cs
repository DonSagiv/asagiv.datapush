using asagiv.pushrocket.common.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Interfaces
{
    public interface IDataPushContext
    {
        #region Properties
        Guid RequestId { get; }
        string SourceNode { get; }
        string DestinationNode { get; }
        string Name { get; }
        Stream Payload { get; }
        string Description { get; }
        int NumberOfBlocksPushed { get; }
        int TotalNumberOfBlocks { get; }
        DeliveryStatus Status { get; }
        IObservable<int> OnPushResponseReceived { get; }
        #endregion

        #region Methods
        Task<bool> PushDataAsync();
        #endregion
    }
}
