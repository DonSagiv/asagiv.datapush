using System.Threading.Tasks;

namespace asagiv.datapush.common.Interfaces
{
    public interface IGrpcDataDownloader
    {
        #region Methods
        Task OnDataRetrievedAsync(IResponseStreamContext<DataPullResponse> responseStreamContext);
        #endregion
    }
}
