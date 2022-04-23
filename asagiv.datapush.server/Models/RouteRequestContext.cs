using asagiv.datapush.common;
using asagiv.datapush.server.Interfaces;
using Grpc.Core;

namespace asagiv.datapush.server.Models
{
    public record RouteRequestContext(DataPushRequest DataPushRequest, IRouteRequest RouteRequest, IServerStreamWriter<DataPushResponse> ResponseStream);
}
