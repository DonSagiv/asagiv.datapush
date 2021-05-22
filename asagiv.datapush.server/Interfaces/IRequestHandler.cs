using Grpc.Core;
using System.Threading.Tasks;

namespace asagiv.datapush.server.Interfaces
{
    public interface IRequestHandler
    {
        Task<RegisterNodeResponse> HandleRegisterNodeRequest(RegisterNodeRequest request);
        Task<DataPushResponse> HandlePushDataAsync(IAsyncStreamReader<DataPushRequest> requestStream);
        Task HandlePullDataAsync(DataPullRequest request, IServerStreamWriter<DataPullResponse> responseStream);
    }
}
