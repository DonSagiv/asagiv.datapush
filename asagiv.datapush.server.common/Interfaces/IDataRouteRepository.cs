using System.Collections.Generic;

namespace asagiv.datapush.server.common.Interfaces
{
    public interface IDataRouteRepository
    {
        IEnumerable<IRouteRequest> Repository { get; }

        IRouteRequest GetRoutePushRequest(string sourceNode, string destinationNode, string name);
        IRouteRequest GetRoutePullRequest(string destinationNode);
        void CloseRouteRequest(IRouteRequest routeRequest);
    }
}
