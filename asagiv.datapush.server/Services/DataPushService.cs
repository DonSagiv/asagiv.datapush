using asagiv.datapush.server.Interfaces;
using Grpc.Core;
using System.Threading.Tasks;

namespace asagiv.datapush.server
{
    public class DataPushService : DataPush.DataPushBase
    {
        #region Fields
        private readonly IRequestHandler _requestHandler;
        #endregion

        #region Constructor
        public DataPushService(IRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }
        #endregion

        #region Methods
        public override Task<DataPushResponse> PushData(IAsyncStreamReader<DataPushRequest> requestStream, ServerCallContext context)
        {
            return _requestHandler.HandlePushDataAsync(requestStream);
        }

        public override Task PullData(DataPullRequest request, IServerStreamWriter<DataPullResponse> responseStream, ServerCallContext context)
        {
            return _requestHandler.HandlePullDataAsync(request, responseStream);
        }

        public override Task<RegisterNodeResponse> RegisterNode(RegisterNodeRequest request, ServerCallContext context)
        {
            return _requestHandler.HandleRegisterNodeRequest(request);
        }
        #endregion
    }
}
