using asagiv.datapush.server.common.Interfaces;
using asagiv.datapush.server.common.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace asagiv.datapush.server.common
{
    public class DataRouteRepository : IDataRouteRepository
    {
        #region Statics
        private const int numMinutesPurge = 1;
        #endregion

        #region Fields
        private readonly ILogger _logger;
        private readonly IList<IRouteRequest> _repository;
        private readonly IObservable<long> _purgeRepositoryObservable;
        private readonly IDisposable _purgeRepositoryDisposable;
        #endregion

        #region Properties
        public IEnumerable<IRouteRequest> Repository => _repository;
        #endregion

        #region Constructor
        public DataRouteRepository(ILogger logger)
        {
            _logger = logger;

            _purgeRepositoryObservable = Observable.Interval(TimeSpan.FromMinutes(1));

            _purgeRepositoryDisposable = _purgeRepositoryObservable
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(purgeRepository);

            _repository = new List<IRouteRequest>();
        }

        public IRouteRequest GetRoutePushRequest(string sourceNode, string destinationNode, string name)
        {
            var routeRequest = _repository
                            .Where(x => x.SourceNode == sourceNode)
                            .Where(x => x.DestinationNode == destinationNode)
                            .FirstOrDefault(x => x.Name == name);

            if (routeRequest == null)
            {
                routeRequest = new RouteRequest(sourceNode, destinationNode, name);

                _logger.Information($"New Route Request Added. (Source: {sourceNode}, Destionation: {destinationNode}, Name: {name}, ID)");

                _repository.Add(routeRequest);
            }
            
            return routeRequest;
        }

        public IRouteRequest GetRoutePullRequest(string destinationNode)
        {
            var routeRequest = _repository
                .Where(x => !x.isRouteCompleted)
                .FirstOrDefault(x => x.DestinationNode == destinationNode);

            if (routeRequest != null)
            {
                routeRequest.isRouteCompleted = true;
            }

            return routeRequest;
        }

        public void CloseRouteRequest(IRouteRequest routeRequest)
        {
            _logger.Information($"Closing Route Request. (Source: {routeRequest.SourceNode}, Destination: {routeRequest.DestinationNode}, Name: {routeRequest.Name})");

            _repository.Remove(routeRequest);
        }

        private void purgeRepository(long obj)
        {
            var itemsToPurge = _repository
                .Where(x => x.PushDateTime > DateTime.Now.AddMinutes(-1 * numMinutesPurge));

            foreach (var itemToPurge in itemsToPurge)
            {
                _logger.Warning($"Purging Route Request from Repository. (Source: {itemToPurge.SourceNode}, Destination: {itemToPurge.DestinationNode}, Name:{itemToPurge.Name})");
                _repository.Remove(itemToPurge);
            }
        }
        #endregion
    }
}
