using asagiv.datapush.common;
using Grpc.Core;
using System.Threading.Tasks;

namespace asagiv.datapush.server.Interfaces
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
        Task<AcknowledgeDeliveryResponse> HandleAcknowledgeDataPull(AcknowledgeDeliveryRequest request);
    }
}
