﻿using asagiv.datapush.server.common.Interfaces;
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
            _logger?.Information($"Register Node Request Received. (Node Name: {request.NodeName}, Device ID: {request.DeviceId}, Is Pull Node: {request.IsPullNode})");

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

            _logger?.Information($"Sending Response to {response.NodeName}. (Is Successful = {response.Successful})");

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
                        routeRequest = _routeRepository.GetRoutePushRequest(request.SourceNode, request.DestinationNode, request.Name);
                    }

                    _logger?.Information($"Adding Payload to Route Request. (Source: {request.SourceNode}, Destionation: {request.DestinationNode}, Name: {request.Name}, Size: {request.Payload.Length} bytes)");

                    routeRequest.AddPayload(request.Payload);
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
            var routeRequest = _routeRepository.GetRoutePullRequest(request.DestinationNode);

            if (routeRequest == null)
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
                _logger?.Information($"Route Request Found for {request.DestinationNode} from {routeRequest.SourceNode} (Name: {routeRequest.Name})");

                // Alert the user that a stream is avaiable.
                await responseStream.WriteAsync(new DataPullResponse
                {
                    SourceNode = routeRequest.SourceNode,
                    DestinationNode = request.DestinationNode,
                    Name = routeRequest.Name,
                    Payload = ByteString.Empty,
                });

                // Sends the payload data.
                while (routeRequest.PayloadQueue.Count > 0)
                {
                    var payload = routeRequest.GetFromPayload();

                    _logger?.Information($"Pushing Data from {routeRequest.SourceNode} to {routeRequest.DestinationNode} (Name: {routeRequest.Name}, Size: {payload.Length} bytes))");

                    await responseStream.WriteAsync(new DataPullResponse
                    {
                        SourceNode = routeRequest.SourceNode,
                        DestinationNode = routeRequest.DestinationNode,
                        Name = routeRequest.Name,
                        Payload = payload,
                    });
                }

                _routeRepository.CloseRouteRequest(routeRequest);
            }
        }
        #endregion
    }
}
