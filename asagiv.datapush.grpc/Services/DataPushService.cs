using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace asagiv.datapush.grpc
{
    public class DataPushService : DataPush.DataPushBase
    {
        private readonly ILogger<DataPushService> _logger;
        public DataPushService(ILogger<DataPushService> logger)
        {
            _logger = logger;
        }

        public override Task<DataPushResponse> PushData(DataPushRequest request, ServerCallContext context)
        {
            return Task.FromResult(new DataPushResponse
            {
                Confirmation = 1,
                Data = request.Data,
            });
        }
    }
}
