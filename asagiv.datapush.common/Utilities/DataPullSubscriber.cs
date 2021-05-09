using Google.Protobuf;
using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace asagiv.datapush.common.Utilities
{
    public class DataPullSubscriber
    {
        #region Fields
        private IObservable<long> _pullObservable;
        private IDisposable _pullSubscribe;
        #endregion

        #region Properties
        public DataPush.DataPushClient Client { get; }
        public string Topic { get; }
        #endregion

        #region Delegages
        public event EventHandler<ByteString> DataRetrieved;
        #endregion

        #region Constructor
        public DataPullSubscriber(DataPush.DataPushClient client, string topic)
        {
            Client = client;
            Topic = topic;

            _pullObservable = Observable.Interval(TimeSpan.FromSeconds(1));

            _pullSubscribe = _pullObservable
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(async x => await pollDataAsync(x));
        }
        #endregion

        #region 
        private async Task<bool> pollDataAsync(long obj)
        {
            var request = new DataPullRequest
            {
                Topic = Path.GetFileName("Test"),
            };

            var pushReply = await Client.PullDataAsync(request);

            if (pushReply.Data.Length == 0)
            {
                return false;
            }
            else
            {
                DataRetrieved?.Invoke(this, pushReply.Data);

                return true;
            }
        }
        #endregion
    }
}