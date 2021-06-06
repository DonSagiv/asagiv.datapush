using System.Collections.Generic;

namespace asagiv.datapush.server.Interfaces
{
    public interface IDataRouteRepository
    {
        IEnumerable<IRouteRequest> Repository { get; }

        IRouteRequest AddRouteRequest(DataPushRequest dataPushRequest);
        IRouteRequest GetRouteRequest(string destinationNode);
        void CloseRouteRequest(IRouteRequest routeRequest);
    }
}
