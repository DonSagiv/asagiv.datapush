using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace asagiv.datapush.grpc
{
    public class DataPushService : DataPush.DataPushBase
    {
        #region Fields
        private readonly ILogger<DataPushService> _logger;
        #endregion

        #region Constructor
        public DataPushService() { }

        public DataPushService(ILogger<DataPushService> logger) : this()
        {
            _logger = logger;
        }
        #endregion

        #region Methods
        public override Task<DataPushResponse> PushData(DataPushRequest request, ServerCallContext context)
        {
            return Task.FromResult(new DataPushResponse
            {
                Confirmation = 1,
                Data = request.Data,
            });
        }
        #endregion
    }
}
