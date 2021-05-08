using asagiv.datapush.server.common;
using asagiv.datapush.server.common.Models;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Linq;
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

            var pushRepositoryItem = new PushRepositoryItem(request.Topic, request.Data);

            PushDataRepository.instance.repository.Add(pushRepositoryItem);

            return Task.FromResult(new DataPushResponse
            {
                Confirmation = 1
            });
        }

        public override Task<DataPullResponse> PullData(DataPullRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Routing topic topic: {request.Topic}");

            var repositoryItem = PushDataRepository.instance.repository
                .FirstOrDefault(x => x.Topic == request.Topic);

            if(repositoryItem == null)
            {
                return Task.FromResult(new DataPullResponse
                {
                    Topic = request.Topic,
                    Data = ByteString.CopyFrom(new byte[0]),
                });
            }
            else
            {
                return Task.FromResult(new DataPullResponse
                {
                    Topic = request.Topic,
                    Data = repositoryItem.Data,
                });
            }
        }
        #endregion
    }
}
