using System;

namespace asagiv.pushrocket.common.Interfaces
{
    public interface IDataPullSubscriber : IDisposable
    {
        #region Properties
        DataPush.DataPushClient Client { get; }
        string DestinationNode { get; }
        bool IsDisposed { get; }
        IObservable<IResponseStreamContext<DataPullResponse>> DataRetrievedObservable { get; }
        #endregion
    }
}
