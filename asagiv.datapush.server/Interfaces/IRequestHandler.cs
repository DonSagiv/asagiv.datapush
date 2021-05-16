using System.Threading.Tasks;

namespace asagiv.datapush.server.Interfaces
{
    public interface IRequestHandler
    {
        Task<RegisterNodeResponse> HandleRegisterNodeRequest(RegisterNodeRequest request);
        Task<DataPushResponse> HandlePushDataAsync(DataPushRequest request);
        Task<DataPullResponse> HandlePullDataAsync(DataPullRequest request);
    }
}
