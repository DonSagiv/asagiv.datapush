using asagiv.pushrocket.common;
using asagiv.pushrocket.server.common.Interfaces;
using Grpc.Core;
using Serilog;
using System.Threading.Tasks;

namespace asagiv.pushrocket.server
{
    public class DataPushService : DataPush.DataPushBase
    {
        #region Fields
        private readonly IRequestHandler _requestHandler;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public DataPushService(IRequestHandler requestHandler, ILogger logger)
        {
            _logger = logger;

            _requestHandler = requestHandler;

            _logger?.Debug("Data push service instantiated.");
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

        public override Task<AcknowledgeDeliveryResponse> AcknowledgeDelivery(AcknowledgeDeliveryRequest request, ServerCallContext context)
        {
            return _requestHandler.HandleAcknowledgeDelivery(request);
        }

        public override Task<ConfirmDeliveryResponse> ConfirmDelivery(ConfirmDeliveryRequest request, ServerCallContext context)
        {
            return _requestHandler.HandleConfirmDelivery(request);
        }
        #endregion
    }
}