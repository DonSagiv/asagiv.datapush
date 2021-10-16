using System;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Interfaces
{
    public interface IGrpcDataDownloader
    {
        #region Delegates
        event EventHandler<AcknowledgeDeliveryRequest> AcknowledgeDelivery;
        #endregion

        #region Methods
        Task OnDataRetrievedAsync(IResponseStreamContext<DataPullResponse> responseStreamContext);
        #endregion
    }
}
