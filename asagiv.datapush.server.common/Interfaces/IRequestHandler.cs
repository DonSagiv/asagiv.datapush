using asagiv.datapush.common;
using Grpc.Core;
using System.Threading.Tasks;

namespace asagiv.datapush.server.common.Interfaces
{
    public interface IRequestHandler
    {
        /// <summary>
        /// Registers the pull node and adds the node to the list of pull nodes.
        /// </summary>
        /// <param name="request">The node request to handle.</param>
        /// <returns>The response from the register node request.</returns>
        Task<RegisterNodeResponse> HandleRegisterNodeRequest(RegisterNodeRequest request);
        /// <summary>
        /// Handles a push data request.
        /// </summary>
        /// <param name="requestStream">The push data stream.</param>
        /// <returns>The push data response.</returns>
        Task HandlePushDataAsync(IAsyncStreamReader<DataPushRequest> requestStream, IServerStreamWriter<DataPushResponse> responseStream);
        /// <summary>
        /// Handles a data pull request.
        /// </summary>
        /// <param name="request">The data pull request.</param>
        /// <param name="responseStream">The response stream that pushes the data to the client.</param>
        /// <returns>A System.Threading.Tasks object. Async is suggested.</returns>
        Task HandlePullDataAsync(DataPullRequest request, IServerStreamWriter<DataPullResponse> responseStream);
        /// <summary>
        /// Triggers when a destination confirms the delivery of a route request.
        /// </summary>
        /// <param name="request">Acknowledge Delivery Notification by the Destination Node.</param>
        /// <returns>Response to the Destination Node.</returns>
        Task<AcknowledgeDeliveryResponse> HandleAcknowledgeDelivery(AcknowledgeDeliveryRequest request);
        /// <summary>
        /// Request made by a source node to determine if a route request has been completed.
        /// </summary>
        /// <param name="request">Request made by the source node.</param>
        /// <returns>Respnse with the delivery confirmation.</returns>
        Task<ConfirmDeliveryResponse> HandleConfirmDelivery(ConfirmDeliveryRequest request);
    }
}
