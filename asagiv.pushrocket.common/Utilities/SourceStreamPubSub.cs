using asagiv.pushrocket.common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace asagiv.pushrocket.ui.Utilities
{
    public class SourceStreamPubSub : ISourceStreamPubSub
    {
        #region Fields
        private readonly Subject<SourceStreamInfo[]> _pushDataContextSubject = new();
        private readonly Queue<SourceStreamInfo[]> _sourceStreamCache;
        #endregion

        #region Properties
        public IObservable<SourceStreamInfo[]> SourceStreamsObservable => _pushDataContextSubject.AsObservable();
        #endregion

        #region Constructor
        public SourceStreamPubSub()
        {
            _sourceStreamCache = new Queue<SourceStreamInfo[]>();
        }
        #endregion

        #region Methods
        public void PublishSourceStreams(IEnumerable<SourceStreamInfo> pushDataContext)
        {
            // Add contextst to cache.
            _sourceStreamCache.Enqueue(pushDataContext.ToArray());

            PullSourceStreams();
        }

        public void PullSourceStreams()
        {
            // If no observers detected keep data in cache.
            if (!_pushDataContextSubject.HasObservers)
            {
                return;
            }

            // Dequeue entire cahce.
            while(_sourceStreamCache.TryDequeue(out var sourceStreamInfo))
            {
                _pushDataContextSubject.OnNext(sourceStreamInfo);
            }
        }
        #endregion
    }
}
