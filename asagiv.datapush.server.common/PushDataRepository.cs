using asagiv.datapush.server.common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace asagiv.datapush.server.common
{
    public class PushDataRepository
    {
        #region Statics
        private const int numMinutesPurge = 1;
        private static readonly Lazy<PushDataRepository> _lazyInstance = new Lazy<PushDataRepository>(() => new PushDataRepository());
        public static PushDataRepository Instance => _lazyInstance.Value;
        #endregion

        #region Fields
        private IObservable<long> _purgeRepositoryObservable;
        private IDisposable _purgeRepositoryDispoasable;
        #endregion

        #region Properties
        public IList<IPushRepositoryItem> Repository { get; }
        #endregion

        #region Constructor
        private PushDataRepository()
        {
            _purgeRepositoryObservable = Observable.Interval(TimeSpan.FromMinutes(1));

            _purgeRepositoryDispoasable = _purgeRepositoryObservable
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(purgeRepository);

            Repository = new List<IPushRepositoryItem>();
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
