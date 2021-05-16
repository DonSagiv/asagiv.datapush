using asagiv.datapush.server.common;
using asagiv.datapush.server.common.Interfaces;
using asagiv.datapush.server.common.Models;
using asagiv.datapush.server.Interfaces;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace asagiv.datapush.server.Models
{
    public class RequestHandler : IRequestHandler
    {
        #region Fields
        private readonly ILogger<DataPushService> _logger;
        private readonly INodeRepository _nodeRepository;
        #endregion

        #region Constructor
        public RequestHandler(ILogger<DataPushService> logger, INodeRepository nodeRepository)
        {
            _logger = logger;
            _nodeRepository = nodeRepository;
        }
        #endregion

        #region Methods
        public Task<RegisterNodeResponse> HandleRegisterNodeRequest(RegisterNodeRequest request)
        {
            _logger.LogInformation($"Regiser Node Received (Node Name: {request.NodeName}, Device ID: {request.DeviceId})");

            _nodeRepository.nodeDictionary[request.DeviceId] = request.NodeName;

            var response = new RegisterNodeResponse
            {
                NodeName = request.NodeName,
                Successful = true
            };

            response.PullNodeList.AddRange(_nodeRepository.nodeDictionary.Select(x => x.Value));

            return Task.FromResult(response);
        }

        public Task<DataPushResponse> HandlePushDataAsync(DataPushRequest request)
        {
            _logger.LogInformation($"Push Request Received (Source: {request.SourceNode}, Destination: {request.DestinationNode})");

            try
            {
                var repositoryItem = new RouteRequest(request.SourceNode, request.DestinationNode, request.Name, request.Payload);

                DataRouteRepository.Instance.Repository.Add(repositoryItem);

                _logger.LogInformation($"Push Request Processed (Source: {request.SourceNode}, Destination: {request.DestinationNode})");

                return Task.FromResult(new DataPushResponse 
                {
                    Confirmation = 1
                });
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Push Request Failed (Source: {request.SourceNode}, Destination: {request.DestinationNode}, Error: {e.Message})");

                return Task.FromResult(new DataPushResponse
                {
                    Confirmation = -1
                });
            }
        }

        public Task<DataPullResponse> HandlePullDataAsync(DataPullRequest request)
        {
            var repositoryItem = DataRouteRepository.Instance.Repository
                .Where(x => !x.isRouteCompleted)
                .FirstOrDefault(x => x.DestinationNode == request.DestinationNode);

            if (repositoryItem == null)
            {
                return Task.FromResult(new DataPullResponse
                {
                    SourceNode = string.Empty,
                    DestinationNode = request.DestinationNode,
                    Name = string.Empty,
                    Payload = ByteString.CopyFrom(Array.Empty<byte>()),
                });
            }
            else
            {
                repositoryItem.isRouteCompleted = true;

                _logger.LogInformation($"Data Pull Item Found (Source: {repositoryItem.SourceNode}, Destination: {repositoryItem.DestinationNode})");

                DataRouteRepository.Instance.Repository.Remove(repositoryItem);

                return Task.FromResult(new DataPullResponse
                {
                    SourceNode = repositoryItem.SourceNode,
                    DestinationNode = repositoryItem.DestinationNode,
                    Name = repositoryItem.Name,
                    Payload = repositoryItem.Payload,
                });
            }
        }
        #endregion
    }
}
