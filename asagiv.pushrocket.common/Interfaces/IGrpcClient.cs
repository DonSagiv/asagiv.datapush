using asagiv.pushrocket.common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Interfaces
{
    public interface IGrpcClient : IDisposable
    {
        #region Delegates
        event EventHandler Disposed;
        #endregion

        #region Properties
        DataPush.DataPushClient Client { get; }
        IList<IDataPullSubscriber> PullSubscribers { get; }
        string DeviceId { get; set; }
        bool IsDisposed { get; }
        IObservable<IResponseStreamContext<DataPullResponse>> DataRetrievedObservable { get; }
        #endregion

        #region Methods
        public Task CreatePullSubscriberAsync();
        Task<IEnumerable<string>> RegisterNodeAsync(bool isPullNode);
        Task<IDataPushContext> CreatePushFileContextAsync(string destinationNode, string filePath, Task<Stream> stream);
        IDataPushContext CreatePushDataContext(string destinationNode, string name, Stream stream);
        Task AcknowledgeDeliveryAsync(AcknowledgeDeliveryRequest acknowledgeDataPullRequest);
        #endregion
    }
}
