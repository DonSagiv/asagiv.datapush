using asagiv.datapush.common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Interfaces
{
    public interface IGrpcClient : IDisposable
    {
        #region Delegates
        event EventHandler Disposed;
        event EventHandler<IResponseStreamContext<DataPullResponse>> DataRetrieved;
        #endregion

        #region Properties
        DataPush.DataPushClient Client { get; }
        IList<IDataPullSubscriber> PullSubscribers { get; }
        string DeviceId { get; set; }
        bool IsDisposed { get; }
        #endregion

        #region Methods
        public Task CreatePullSubscriberAsync();
        Task<IEnumerable<string>> RegisterNodeAsync(bool isPullNode);
        Task<IDataPushContext> CreatePushFileContextAsync(string destinationNode, string filePath);
        IDataPushContext CreatePushDataContext(string destinationNode, string name, byte[] data);
        Task AcknowledgeDataPull(AcknowledgeDeliveryRequest acknowledgeDataPullRequest);
        #endregion
    }
}
