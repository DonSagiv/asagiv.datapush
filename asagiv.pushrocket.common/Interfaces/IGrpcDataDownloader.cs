using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Interfaces
{
    public interface IGrpcDataDownloader
    {
        #region Methods
        Task<AcknowledgeDeliveryRequest> OnDataRetrievedAsync(IResponseStreamContext<DataPullResponse> responseStreamContext);
        #endregion
    }
}
