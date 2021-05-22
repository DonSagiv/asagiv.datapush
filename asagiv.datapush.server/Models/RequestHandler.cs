using asagiv.datapush.server.common;
using asagiv.datapush.server.common.Interfaces;
using asagiv.datapush.server.common.Models;
using asagiv.datapush.server.Interfaces;
using Google.Protobuf;
using Grpc.Core;
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

            var node = _nodeRepository.nodeList.FirstOrDefault(x => x.DeviceId == request.DeviceId);

            if(node == null)
            {
                _nodeRepository.nodeList.Add(new DeviceNode(request.NodeName, request.DeviceId, request.IsPullNode));
            }
            else
            {
                node.NodeName = request.NodeName;
                node.IsPullNode = request.IsPullNode;
            }

            var response = new RegisterNodeResponse
            {
                NodeName = request.NodeName,
                Successful = true
            };

            var pullNodes = _nodeRepository
                .nodeList
                .Where(x => x.IsPullNode)
                .Select(x => x.NodeName)
                .ToList();

            response.PullNodeList.AddRange(pullNodes);

            return Task.FromResult(response);
        }

        public async Task<DataPushResponse> HandlePushDataAsync(IAsyncStreamReader<DataPushRequest> requestStream)
        {
            IRouteRequest repositoryItem = null;
            DataPushRequest currentRequest = null;

            try
            {
                while (await requestStream.MoveNext())
                {
                    currentRequest = requestStream.Current;

                    if (repositoryItem == null)
                    {
                        _logger.LogInformation($"Push Request Received (Source: {currentRequest.SourceNode}, Destination: {currentRequest.DestinationNode})");

                        repositoryItem = DataRouteRepository.Instance.Repository
                            .Where(x => x.SourceNode == currentRequest.SourceNode)
                            .Where(x => x.DestinationNode == currentRequest.DestinationNode)
                            .FirstOrDefault(x => x.Name == currentRequest.Name);

                        if (repositoryItem == null)
                        {
                            repositoryItem = new RouteRequest(currentRequest.SourceNode, currentRequest.DestinationNode, currentRequest.Name);

                            DataRouteRepository.Instance.Repository.Add(repositoryItem);
                        }
                    }

                    repositoryItem.AddToPayload(currentRequest.Payload);
                }

                return await Task.FromResult(new DataPushResponse
                {
                    Confirmation = 1
                });
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Push Request Failed (Source: {currentRequest?.SourceNode}, Destination: {currentRequest.DestinationNode}, Error: {e.Message})");

                return await Task.FromResult(new DataPushResponse
                {
                    Confirmation = -1
                });
            }
        }

        public async Task HandlePullDataAsync(DataPullRequest request, IServerStreamWriter<DataPullResponse> responseStream)
        {
            var repositoryItem = DataRouteRepository.Instance.Repository
                .Where(x => !x.isRouteCompleted)
                .FirstOrDefault(x => x.DestinationNode == request.DestinationNode);

            if (repositoryItem == null)
            {
                await responseStream.WriteAsync(new DataPullResponse
                {
                    SourceNode = string.Empty,
                    DestinationNode = request.DestinationNode,
                    Name = string.Empty,
                    Payload = ByteString.Empty,
                });
            }
            else
            {
                _logger.LogInformation($"Data Pull Item Found (Source: {repositoryItem.SourceNode}, Destination: {repositoryItem.DestinationNode})");

                repositoryItem.isRouteCompleted = true;

                // Alert the user that a stream is avaiable.
                await responseStream.WriteAsync(new DataPullResponse
                {
                    SourceNode = repositoryItem.SourceNode,
                    DestinationNode = request.DestinationNode,
                    Name = repositoryItem.Name,
                    Payload = ByteString.Empty,
                });

                // Sends the payload data.
                while (repositoryItem.PayloadQueue.Count > 0)
                {
                    var response = repositoryItem.GetFromPayload();

                    await responseStream.WriteAsync(new DataPullResponse
                    {
                        SourceNode = repositoryItem.SourceNode,
                        DestinationNode = repositoryItem.DestinationNode,
                        Name = repositoryItem.Name,
                        Payload = response,
                    });
                }
            }
        }
        #endregion
    }
}
