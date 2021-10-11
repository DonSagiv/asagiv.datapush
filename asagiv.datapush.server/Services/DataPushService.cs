using asagiv.datapush.common;
using asagiv.datapush.server.Interfaces;
using Grpc.Core;
using Serilog;
using System.Threading.Tasks;

namespace asagiv.datapush.server
{
    public class DataPushService : DataPush.DataPushBase
    {
        #region Fields
        private readonly IRequestHandler _requestHandler;
        #endregion

        #region Constructor
        public DataPushService(IRequestHandler requestHandler, ILogger logger)
        {
            _requestHandler = requestHandler;

            logger?.Debug("Data push service instantiated.");
        }
        #endregion

        #region Methods
        public override Task PushData(IAsyncStreamReader<DataPushRequest> requestStream, IServerStreamWriter<DataPushResponse> responseStream, ServerCallContext context)
        {
            return _requestHandler.HandlePushDataAsync(requestStream, responseStream);
        }

        public override Task PullData(DataPullRequest request, IServerStreamWriter<DataPullResponse> responseStream, ServerCallContext context)
        {
            return _requestHandler.HandlePullDataAsync(request, responseStream);
        }

        public override Task<RegisterNodeResponse> RegisterNode(RegisterNodeRequest request, ServerCallContext context)
        {
            return _requestHandler.HandleRegisterNodeRequest(request);
        }

        public override Task<AcknowledgeDataPullResponse> AcknowledgeDataPull(AcknowledgeDataPullRequest request, ServerCallContext context)
        {
            return _requestHandler.HandleAcknowledgeDataPull(request);
        }
        #endregion
    }
}