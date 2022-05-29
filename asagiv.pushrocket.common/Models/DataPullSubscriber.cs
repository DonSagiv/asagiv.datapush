using asagiv.pushrocket.common.Interfaces;
using Grpc.Core;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Models
{
    public sealed class DataPullSubscriber : IDataPullSubscriber
    {
        #region Fields
        private readonly IDisposable _pullSubscribe;
        #endregion

        #region Delegages
        public event EventHandler<IResponseStreamContext<DataPullResponse>> DataRetrieved;
        #endregion

        #region Properties
        public DataPush.DataPushClient Client { get; }
        public string DestinationNode { get; }
        public bool IsDisposed { get; private set; }
        #endregion

        #region Constructor
        public DataPullSubscriber(DataPush.DataPushClient client, string node)
        {
            Client = client;

            DestinationNode = node;

            var pullObservable = Observable.Interval(TimeSpan.FromSeconds(1));

            _pullSubscribe = pullObservable
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(async _ => await PollDataAsync());
        }
        #endregion

        #region Methods
        private async Task<bool> PollDataAsync()
        {
            var request = new DataPullRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                DestinationNode = DestinationNode
            };

            var pullResponse = Client.PullData(request);

            if (await pullResponse.ResponseStream.MoveNext() && !string.IsNullOrEmpty(pullResponse.ResponseStream.Current.Name))
            {
                var responseStreamContext = new ResponseStreamContext<DataPullResponse>(pullResponse.ResponseStream.Current, pullResponse.ResponseStream);

                DataRetrieved?.Invoke(this, responseStreamContext);

                return true;
            }

            return false;
        }

        public void Dispose()
        {
            _pullSubscribe.Dispose();

            GC.SuppressFinalize(this);

            IsDisposed = true;
        }
        #endregion
    }
}