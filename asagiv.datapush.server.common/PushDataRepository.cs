using asagiv.datapush.server.common.Interfaces;
using System;
using System.Collections.Generic;

namespace asagiv.datapush.server.common
{
    public class PushDataRepository
    {
        #region Statics
        private static readonly Lazy<PushDataRepository> _lazyInstance = new Lazy<PushDataRepository>(() => new PushDataRepository());
        public static PushDataRepository instance => _lazyInstance.Value;
        #endregion

        #region Properties
        public IList<IPushRepositoryItem> repository { get; }
        #endregion

        #region Constructor
        private PushDataRepository()
        {
            repository = new List<IPushRepositoryItem>();
        }
        #endregion
    }
}
