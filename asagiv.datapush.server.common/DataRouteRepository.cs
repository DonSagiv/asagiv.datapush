using asagiv.datapush.server.common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace asagiv.datapush.server.common
{
    public class DataRouteRepository
    {
        #region Statics
        private const int numMinutesPurge = 1;
        private static readonly Lazy<DataRouteRepository> _lazyInstance = new Lazy<DataRouteRepository>(() => new DataRouteRepository());
        public static DataRouteRepository Instance => _lazyInstance.Value;
        #endregion

        #region Fields
        private IObservable<long> _purgeRepositoryObservable;
        private IDisposable _purgeRepositoryDispoasable;
        #endregion

        #region Properties
        public IList<IRouteRequest> Repository { get; }
        #endregion

        #region Constructor
        private DataRouteRepository()
        {
            _purgeRepositoryObservable = Observable.Interval(TimeSpan.FromMinutes(1));

            _purgeRepositoryDispoasable = _purgeRepositoryObservable
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(purgeRepository);

            Repository = new List<IRouteRequest>();
        }

        private void purgeRepository(long obj)
        {
            var itemsToPurge = Repository
                .Where(x => x.PushDateTime > DateTime.Now.AddMinutes(-1 * numMinutesPurge));

            foreach(var itemToPurge in itemsToPurge)
            {
                Repository.Remove(itemToPurge);
            }
        }
        #endregion
    }
}
