﻿using asagiv.pushrocket.common;
using asagiv.pushrocket.server.common.Models;
using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Concurrent;

namespace asagiv.pushrocket.server.common.Interfaces
{
    public interface IRouteRequest
    {
        /// <summary>
        /// The GUID of the route request.
        /// </summary>
        Guid RequestId { get; }
        /// <summary>
        /// The name of the source of the data in the route request.
        /// </summary>
        string SourceNode { get; }
        /// <summary>
        /// The name of the destination of the data in the route request.
        /// </summary>
        string DestinationNode { get; }
        /// <summary>
        /// The name of the route request.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Number of payload blocks retrieved.
        /// </summary>
        int BlocksRetrieved { get; }
        /// <summary>
        /// The total number of blocks of data.
        /// </summary>
        int TotalBlocks { get; }
        /// <summary>
        /// The route request inception date and time.
        /// </summary>
        DateTime PushDateTime { get; }
        /// <summary>
        /// The queue of payload items that are being pushed.
        /// </summary>
        ConcurrentQueue<PayloadItem> PayloadQueue { get; }
        /// <summary>
        /// Error message raised by the route request.
        /// </summary>
        string ErrorMessage { get; }
        /// <summary>
        /// true if the route has been connected to its destination node.
        /// </summary>
        bool IsRouteConnected { get; set; }
        /// <summary>
        /// True if all data has been successfully pushed.
        /// </summary>
        bool IsRouteCompleted { get; }
        /// <summary>
        /// True if an error has occurred during routing.
        /// </summary>
        bool IsDeliveryAcknowledged { get; }
        /// <summary>
        /// Response Stream of the Request
        /// </summary>
        IServerStreamWriter<DataPushResponse> ResponseStream { get; }

        /// <summary>
        /// Adds a new payload item to the payload queue.
        /// </summary>
        /// <param name="blockNumber">The block number of the payload item.</param>
        /// <param name="payloadItemToAdd">The acutal payload data.</param>
        PayloadItem AddPayload(int blockNumber, ByteString payloadByteString);
        /// <summary>
        /// Dequeues a payload item from the payload queue.
        /// </summary>
        /// <returns></returns>
        PayloadItem GetFromPayload();
        /// <summary>
        /// Confirm that the payloadf for the route requests have been delivered successfully.
        /// </summary>
        /// <param name="errorMessage">Error message if there is one</param>
        void ConfirmRouteDelivery(string errorMessage);
    }
}
