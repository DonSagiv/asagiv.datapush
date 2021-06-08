using asagiv.datapush.server.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace asagiv.datapush.server.Models
{
    public class DataRouteRepository : IDataRouteRepository
    {
        #region Statics
        private const int numMinutesPurge = 1;
        #endregion

        #region Fields
        private readonly ILogger _logger;
        private readonly IList<IRouteRequest> _repository;
        #endregion

        #region Properties
        public IEnumerable<IRouteRequest> Repository => _repository;
        #endregion

        #region Constructor
        public DataRouteRepository(ILogger logger)
        {
            _logger = logger;

            var purgeRepositoryObservable = Observable.Interval(TimeSpan.FromMinutes(1));

            purgeRepositoryObservable
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(PurgeRepository);

            _repository = new List<IRouteRequest>();
        }

        public IRouteRequest AddRouteRequest(DataPushRequest dataPushRequest)
        {
            var routeRequest = _repository
                            .Where(x => x.SourceNode == dataPushRequest.SourceNode)
                            .Where(x => x.DestinationNode == dataPushRequest.DestinationNode)
                            .FirstOrDefault(x => x.Name == dataPushRequest.Name);

            if (routeRequest == null)
            {
                routeRequest = new RouteRequest(dataPushRequest);

                _logger?.Information($"New Route Request Added (Source: {routeRequest.SourceNode}, " +
                    $"Destionation: {routeRequest.DestinationNode}, " +
                    $"Name: {routeRequest.Name}, " +
                    $"ID: {routeRequest.RequestId}).");

                _repository.Add(routeRequest);
            }
            
            return routeRequest;
        }

        public IRouteRequest GetRouteRequest(string destinationNode)
        {
            var routeRequest = _repository
                .Where(x => !x.IsRouteCompleted)
                .FirstOrDefault(x => x.DestinationNode == destinationNode);

            if (routeRequest != null)
            {
                routeRequest.IsRouteCompleted = true;
            }

            return routeRequest;
        }

        public void CloseRouteRequest(IRouteRequest routeRequest)
        {
            _logger?.Information($"Closing Route Request " +
                $"(Source: {routeRequest.SourceNode}, " +
                $"Destination: {routeRequest.DestinationNode}, " +
                $"Name: {routeRequest.Name}).");

            _repository.Remove(routeRequest);
        }

        private void PurgeRepository(long obj)
        {
            var itemsToPurge = _repository
                .Where(x => x.PushDateTime > DateTime.Now.AddMinutes(-1 * numMinutesPurge));

            foreach (var itemToPurge in itemsToPurge)
            {
                _logger?.Warning($"Purging Route Request from Repository " +
                    $"(Source: {itemToPurge.SourceNode}, " +
                    $"Destination: {itemToPurge.DestinationNode}, " +
                    $"Name:{itemToPurge.Name}).");

                _repository.Remove(itemToPurge);
            }
        }
        #endregion
    }
}
