using System;
using System.Reactive;
using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Interfaces
{
    public interface IGrpcDataDownloader
    {
        #region Delegates
        event EventHandler<AcknowledgeDeliveryRequest> AcknowledgeDelivery;
        #endregion

        #region Methods
        Task<Unit> OnDataRetrievedAsync(IResponseStreamContext<DataPullResponse> responseStreamContext);
        #endregion
    }
}
