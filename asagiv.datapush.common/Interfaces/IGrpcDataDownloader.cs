using System;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Interfaces
{
    public interface IGrpcDataDownloader
    {
        #region Delegates
        event EventHandler<AcknowledgeDataPullRequest> AcknowledgeDataPush;
        #endregion

        #region Methods
        Task OnDataRetrievedAsync(IResponseStreamContext<DataPullResponse> responseStreamContext);
        #endregion
    }
}
