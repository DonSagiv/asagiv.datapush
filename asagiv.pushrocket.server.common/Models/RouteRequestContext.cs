using asagiv.pushrocket.common;
using asagiv.pushrocket.server.common.Interfaces;
using Grpc.Core;

namespace asagiv.pushrocket.server.common.Models
{
    public record RouteRequestContext(DataPushRequest DataPushRequest, IRouteRequest RouteRequest, IServerStreamWriter<DataPushResponse> ResponseStream);
}
