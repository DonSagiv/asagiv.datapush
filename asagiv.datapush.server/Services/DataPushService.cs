using asagiv.datapush.server.common.Interfaces;
using asagiv.datapush.server.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace asagiv.datapush.server
{
    public class DataPushService : DataPush.DataPushBase
    {
        #region Fields
        private readonly ILogger<DataPushService> _logger;
        private readonly IRequestHandler _requestHandler;

        #endregion

        #region Constructor
        public DataPushService(ILogger<DataPushService> logger, IRequestHandler requestHandler)
        {
            _logger = logger;
            _requestHandler = requestHandler;
        }
        #endregion

        #region Methods
        public override Task<DataPushResponse> PushData(DataPushRequest request, ServerCallContext context)
        {
            return _requestHandler.HandlePushDataAsync(request);
        }

        public override Task<DataPullResponse> PullData(DataPullRequest request, ServerCallContext context)
        {
            return _requestHandler.HandlePullDataAsync(request);
        }

        public override Task<RegisterNodeResponse> RegisterNode(RegisterNodeRequest request, ServerCallContext context)
        {
            return _requestHandler.HandleRegisterNodeRequest(request);
        }
        #endregion
    }
}
