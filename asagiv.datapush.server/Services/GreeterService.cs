using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace asagiv.datapush.server
{
    public class DataPushService : DataPush.DataPushBase
    {
        #region Fields
        private readonly ILogger<DataPushService> _logger;
        #endregion

        #region Constructor
        public DataPushService(ILogger<DataPushService> logger)
        {
            _logger = logger;
        }
        #endregion

        #region Methods
        public override Task<DataPushResponse> PushData(DataPushRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Request Received for topic: {request.Topic}");

            return Task.FromResult(new DataPushResponse
            {
                Confirmation = 1,
                Data = request.Data
            });
        }
        #endregion
    }
}
