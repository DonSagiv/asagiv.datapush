using asagiv.pushrocket.ui.Utilities;
using System;
using System.Collections.Generic;

namespace asagiv.pushrocket.common.Interfaces
{
    public interface ISourceStreamPubSub
    {
        public IObservable<SourceStreamInfo[]> SourceStreamsObservable { get; }

        public void PublishSourceStreams(IEnumerable<SourceStreamInfo> pushDataContext);
        void PullSourceStreams();
    }
}
