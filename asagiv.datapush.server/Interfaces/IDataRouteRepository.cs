using asagiv.datapush.common;
using Grpc.Core;
using System.Collections.Generic;

namespace asagiv.datapush.server.Interfaces
{
    public interface IDataRouteRepository
    {
        /// <summary>
        /// The route request repository.
        /// </summary>
        IEnumerable<IRouteRequest> Repository { get; }

        /// <summary>
        /// Add a new route request to the data repository.
        /// </summary>
        /// <param name="dataPushRequest">The data push request</param>
        /// <returns>The reated route request.</returns>
        IRouteRequest AddRouteRequest(DataPushRequest dataPushRequest, IServerStreamWriter<DataPushResponse> responseStream);
        /// <summary>
        /// Get a route request belonging to a destination node, and mark it as connected.
        /// </summary>
        /// <param name="destinationNode">The destination node of the route request.</param>
        /// <returns>The route request with the selected destination node.</returns>
        IRouteRequest ConnectRouteRequest(string destinationNode);
        /// <summary>
        /// Get a the route request for the destination node.
        /// </summary>
        /// <param name="destinationNode">The destination node of the route request.</param>
        /// <returns>The route request with the selected destination node.</returns>
        IRouteRequest GetRouteRequest(string destinationNode);
        /// <summary>
        /// Closes the input route request.
        /// </summary>
        /// <param name="routeRequest">The route request to close.</param>
        void CloseRouteRequest(IRouteRequest routeRequest);
    }
}
