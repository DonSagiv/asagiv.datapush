using asagiv.pushrocket.common;
using asagiv.pushrocket.server.common.Interfaces;
using Google.Protobuf;
using Grpc.Core;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace asagiv.pushrocket.server.common.Models
{
    public class RequestHandler : IRequestHandler
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly INodeRepository _nodeRepository;
        private readonly IDataRouteRepository _routeRepository;
        private readonly Subject<RouteRequestContext> _dataPushRequestReceived = new();
        #endregion

        #region Constructor
        public RequestHandler(INodeRepository nodeRepository, IDataRouteRepository routeRepository, ILogger logger)
        {
            _nodeRepository = nodeRepository;
            _routeRepository = routeRepository;
            _logger = logger;

            _dataPushRequestReceived
                .SelectMany(x => OnPushRequestReceivedAsync(x))
                .Subscribe();

            _logger?.Debug("Request Handler Instantiated.");
        }
        #endregion

        #region Methods
        public Task<RegisterNodeResponse> HandleRegisterNodeRequest(RegisterNodeRequest request)
        {
            _logger?.Information("Register Node Request Received " +
                $"(Node Name: {request.NodeName}, " +
                $"Device ID: {request.DeviceId}, " +
                $"Is Pull Node: {request.IsPullNode}).");

            // Create or Get Node for Device ID.
            var node = _nodeRepository.GetNode(request.NodeName, request.DeviceId, request.IsPullNode);

            // Create registger node response object.
            var response = new RegisterNodeResponse
            {
                RequestId = request.RequestId,
                NodeName = node.NodeName,
                Successful = true
            };

            // Get list of pull nodes.
            var pullNodes = _nodeRepository
                .PullNodes
                .Select(x => x.NodeName)
                .ToList();

            // Add pull nodes to response pul node list.
            response.PullNodeList.AddRange(pullNodes);

            _logger?.Information($"Sending Response to {response.NodeName} " +
                $"(Is Successful = {response.Successful}).");

            // Return register node request with available pull nodes.
            return Task.FromResult(response);
        }

        public async Task HandlePushDataAsync(IAsyncStreamReader<DataPushRequest> requestStream, IServerStreamWriter<DataPushResponse> responseStream)
        {
            DataPushRequest request = null;
            IRouteRequest routeRequest = null;

            try
            {
                while (await requestStream.MoveNext())
                {
                    request = requestStream.Current;

                    // Add route request to repository if none available.
                    if (routeRequest == null)
                    {
                        routeRequest = _routeRepository.AddRouteRequest(request, responseStream);
                    }

                    // Push Route Request Context.
                    _dataPushRequestReceived.OnNext(new RouteRequestContext(request, routeRequest, responseStream));
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, ex.Message);

                var response = new DataPushResponse
                {
                    RequestId = request.RequestId,
                    DestinationNode = request.DestinationNode,
                    Confirmation = -1,
                    BlockNumber = -1,
                    ErrorMessage = ex.Message
                };

                await responseStream.WriteAsync(response);
            }
        }

        private async Task<Unit> OnPushRequestReceivedAsync(RouteRequestContext routeRequestContext)
        {
            var dataPushRequest = routeRequestContext.DataPushRequest;
            var routeRequest = routeRequestContext.RouteRequest;
            var responseStream = routeRequestContext.ResponseStream;

            _logger?.Information("Adding Payload to Route Request " +
                $"({dataPushRequest.BlockNumber} of {dataPushRequest.TotalBlocks} " +
                $"Source: {dataPushRequest.SourceNode}, " +
                $"Destionation: {dataPushRequest.DestinationNode}, " +
                $"Name: {dataPushRequest.Name}, " +
                $"Size: {dataPushRequest.Payload.Length} bytes).");

            // Add payload to route request.
            routeRequest.AddPayload(dataPushRequest.BlockNumber, dataPushRequest.Payload);

            var response = new DataPushResponse
            {
                RequestId = dataPushRequest.RequestId,
                DestinationNode = dataPushRequest.DestinationNode,
                Confirmation = 1,
                BlockNumber = dataPushRequest.BlockNumber,
                ErrorMessage = string.Empty
            };

            await responseStream.WriteAsync(response);

            return Unit.Default;
        }

        public async Task HandlePullDataAsync(DataPullRequest request, IServerStreamWriter<DataPullResponse> responseStream)
        {
            // Get route request for selected destination node.
            var routeRequest = _routeRepository.ConnectRouteRequest(request.DestinationNode);

            if (routeRequest == null)
            {
                await SendEmptyResponse(request, responseStream);
            }
            else
            {
                await HandleRouteRequest(request, responseStream, routeRequest);
            }
        }

        private async static Task SendEmptyResponse(DataPullRequest request, IServerStreamWriter<DataPullResponse> responseStream)
        {
            // If no route request with destination node found, return empty response.
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

        private async Task HandleRouteRequest(DataPullRequest request, IServerStreamWriter<DataPullResponse> responseStream, IRouteRequest routeRequest)
        {
            _logger?.Information($"Route Request Found for {request.DestinationNode} from {routeRequest.SourceNode} " +
                $"(Name: {routeRequest.Name}).");

            // Alert the client that a stream is avaiable.
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
            while (!routeRequest.IsRouteCompleted)
            {
                await PushPayloadToDestination(responseStream, routeRequest);
            }

            _logger?.Information($"Route Completed: (Destination Node: {routeRequest.DestinationNode})");
        }

        private async Task PushPayloadToDestination(IServerStreamWriter<DataPullResponse> responseStream, IRouteRequest routeRequest)
        {
            // Get payload to push to destination node.
            var payload = routeRequest.GetFromPayload();

            if (payload == null)
            {
                // Sometimes dequeue is faster than enqueue
                return;
            }

            _logger?.Information($"Pushing Data from {routeRequest.SourceNode} to {routeRequest.DestinationNode} " +
                $"(Block {payload.BlockNumber} of {routeRequest.TotalBlocks}, " +
                $"Name: {routeRequest.Name}, " +
                $"Size: {payload.Payload.Length} bytes).");

            try
            {
                // Push payload to destination node.
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

                payload.SetPayloadConsumed();
            }
            catch(Exception ex)
            {
                payload.SetPayloadPushError(ex.Message);
            }
        }

        public Task<AcknowledgeDeliveryResponse> HandleAcknowledgeDelivery(AcknowledgeDeliveryRequest request)
        {
            _routeRepository.ConfirmRequestDelivery(request.DestinationNode);

            return Task.FromResult(new AcknowledgeDeliveryResponse
            {
                RequestId = request.RequestId
            });
        }

        public Task<ConfirmDeliveryResponse> HandleConfirmDelivery(ConfirmDeliveryRequest request)
        {
            var acnowledgedRequest = _routeRepository.Repository
                .Where(x => x.IsDeliveryAcknowledged)
                .FirstOrDefault(x => x.DestinationNode == request.DestinationNode);

            if(acnowledgedRequest == null)
            {
                return Task.FromResult(new ConfirmDeliveryResponse
                {
                    RequestId = request.RequestId,
                    Name = request.Name,
                    DestinationNode = request.DestinationNode,
                    IsRouteCompleted = false,
                    IsDeliverySuccessful = false,
                    ErrorMessage = string.Empty
                });
            }

            _logger.Information($"Sending Delivery Confirmation for {request.Name}. " +
                $"(Request ID: {request.RequestId}, Is Delivery Successful: {string.IsNullOrWhiteSpace(acnowledgedRequest.ErrorMessage)})");

            // Close the route request.
            _routeRepository.CloseRouteRequest(acnowledgedRequest);

            return Task.FromResult(new ConfirmDeliveryResponse
            {
                RequestId = request.RequestId,
                Name = request.Name,
                DestinationNode = request.DestinationNode,
                IsRouteCompleted = acnowledgedRequest.IsRouteCompleted,
                IsDeliverySuccessful = string.IsNullOrWhiteSpace(acnowledgedRequest.ErrorMessage),
                ErrorMessage = acnowledgedRequest.ErrorMessage
            });
        }
        #endregion
    }
}