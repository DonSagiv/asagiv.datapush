using System;

namespace asagiv.pushrocket.common.Interfaces
{
    public interface IDataPullSubscriber : IDisposable
    {
        #region Delegates
        event EventHandler<IResponseStreamContext<DataPullResponse>> DataRetrieved;
        #endregion

        #region Properties
        DataPush.DataPushClient Client { get; }
        string DestinationNode { get; }
        bool IsDisposed { get; }
        #endregion
    }
}
