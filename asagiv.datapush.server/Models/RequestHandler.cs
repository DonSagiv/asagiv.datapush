using asagiv.datapush.server.common.Interfaces;
using asagiv.datapush.server.Interfaces;
using Google.Protobuf;
using Grpc.Core;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace asagiv.datapush.server.Models
{
    public class RequestHandler : IRequestHandler
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly INodeRepository _nodeRepository;
        private readonly IDataRouteRepository _routeRepository;
        #endregion

        #region Constructor
        public RequestHandler(INodeRepository nodeRepository, IDataRouteRepository routeRepository, ILogger logger)
        {
            _nodeRepository = nodeRepository;
            _routeRepository = routeRepository;
            _logger = logger;

            _logger?.Debug("Request Handler Instantiated.");
        }
        #endregion

        #region Methods
        public Task<RegisterNodeResponse> HandleRegisterNodeRequest(RegisterNodeRequest request)
        {
            _logger?.Information($"Register Node Request Received " +
                $"(Node Name: {request.NodeName}, " +
                $"Device ID: {request.DeviceId}, " +
                $"Is Pull Node: {request.IsPullNode}).");

            var node = _nodeRepository.GetNode(request.NodeName, request.DeviceId, request.IsPullNode);

            var response = new RegisterNodeResponse
            {
                RequestId = request.RequestId,
                NodeName = node.NodeName,
                Successful = true
            };

            var pullNodes = _nodeRepository
                .PullNodes
                .Select(x => x.NodeName)
                .ToList();

            response.PullNodeList.AddRange(pullNodes);

            _logger?.Information($"Sending Response to {response.NodeName} " +
                $"(Is Successful = {response.Successful}).");

            return Task.FromResult(response);
        }

        public async Task<DataPushResponse> HandlePushDataAsync(IAsyncStreamReader<DataPushRequest> requestStream)
        {
            IRouteRequest routeRequest = null;
            DataPushRequest request;

            try
            {
                while (await requestStream.MoveNext())
                {
                    request = requestStream.Current;

                    if (routeRequest == null)
                    {
                        routeRequest = _routeRepository.AddRouteRequest(request);
                    }

                    _logger?.Information($"Adding Payload to Route Request " +
                        $"({request.BlockNumber} of {request.TotalBlocks} " +
                        $"Source: {request.SourceNode}, " +
                        $"Destionation: {request.DestinationNode}, " +
                        $"Name: {request.Name}, " +
                        $"Size: {request.Payload.Length} bytes).");

                    routeRequest.AddPayload(request.BlockNumber, request.Payload);
                }

                return await Task.FromResult(new DataPushResponse
                {
                    Confirmation = 1
                });
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, ex.Message);

                return await Task.FromResult(new DataPushResponse
                {
                    Confirmation = -1
                });
            }
        }

        public async Task HandlePullDataAsync(DataPullRequest request, IServerStreamWriter<DataPullResponse> responseStream)
        {
            var routeRequest = _routeRepository.GetRouteRequest(request.DestinationNode);

            if (routeRequest == null)
            {
                await responseStream.WriteAsync(new DataPullResponse
                {
                    RequestId = request.RequestId,
                    SourceRequestId = string.Empty,
                    SourceNode = string.Empty,
                    DestinationNode = request.DestinationNode,
                    BlockNumber = 0,
                    TotalBlocks = 0,
                    Name = string.Empty,
                    Payload = ByteString.Empty,
                });
            }
            else
            {
                _logger?.Information($"Route Request Found for {request.DestinationNode} from {routeRequest.SourceNode} " +
                    $"(Name: {routeRequest.Name}).");

                // Alert the user that a stream is avaiable.
                await responseStream.WriteAsync(new DataPullResponse
                {
                    RequestId = request.RequestId,
                    SourceRequestId = routeRequest.RequestId.ToString(),
                    SourceNode = routeRequest.SourceNode,
                    DestinationNode = request.DestinationNode,
                    BlockNumber = 0,
                    TotalBlocks = routeRequest.TotalBlocks,
                    Name = routeRequest.Name,
                    Payload = ByteString.Empty,
                });

                // Sends the payload data.
                while (!routeRequest.PayloadQueue.IsEmpty)
                {
                    var payload = routeRequest.GetFromPayload();

                    _logger?.Information($"Pushing Data from {routeRequest.SourceNode} to {routeRequest.DestinationNode} " +
                        $"(Block {payload.BlockNumber} of {routeRequest.TotalBlocks}, " +
                        $"Name: {routeRequest.Name}, " +
                        $"Size: {payload.Payload.Length} bytes).");

                    await responseStream.WriteAsync(new DataPullResponse
                    {
                        SourceNode = routeRequest.SourceNode,
                        SourceRequestId = routeRequest.RequestId.ToString(),
                        DestinationNode = routeRequest.DestinationNode,
                        Name = routeRequest.Name,
                        BlockNumber = payload.BlockNumber,
                        TotalBlocks = routeRequest.TotalBlocks,
                        Payload = payload.Payload,
                    });
                }

                _routeRepository.CloseRouteRequest(routeRequest);
            }
        }
        #endregion
    }
}
