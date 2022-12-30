using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Interfaces
{
    public interface IGrpcDataDownloader
    {
        #region Properties
        string SaveDirectory { get; }
        #endregion

        #region Methods
        Task<AcknowledgeDeliveryRequest> OnDataRetrievedAsync(IResponseStreamContext<DataPullResponse> responseStreamContext);
        #endregion
    }
}
