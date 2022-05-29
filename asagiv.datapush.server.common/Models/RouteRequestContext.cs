using asagiv.datapush.common;
using asagiv.datapush.server.common.Interfaces;
using Grpc.Core;

namespace asagiv.datapush.server.common.Models
{
    public record RouteRequestContext(DataPushRequest DataPushRequest, IRouteRequest RouteRequest, IServerStreamWriter<DataPushResponse> ResponseStream);
}
