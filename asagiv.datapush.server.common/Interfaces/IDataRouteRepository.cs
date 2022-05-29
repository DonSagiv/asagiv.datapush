using asagiv.datapush.common;
using Grpc.Core;
using System.Collections.Generic;

namespace asagiv.datapush.server.common.Interfaces
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
        /// Get a route request belonging to a destination node.
        /// </summary>
        /// <param name="destinationNode">The destination node of the route request.</param>
        /// <returns>The route request with the selected destination node.</returns>
        IRouteRequest ConnectRouteRequest(string destinationNode);
        /// <summary>
        /// Find a route request with the destination node and confirm its delivery.
        /// </summary>
        /// <param name="destinationNode">The destination node of the route request</param>
        /// <param name="errorMessage">An error message for delivery, if applicable</param>
        /// <returns></returns>
        IRouteRequest ConfirmRequestDelivery(string destinationNode, string errorMessage = "");
        /// <summary>
        /// Closes the input route request.
        /// </summary>
        /// <param name="routeRequest">The route request to close.</param>
        void CloseRouteRequest(IRouteRequest routeRequest);
    }
}
