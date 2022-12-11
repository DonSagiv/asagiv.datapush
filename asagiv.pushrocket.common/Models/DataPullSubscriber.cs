using asagiv.pushrocket.common.Interfaces;
using Grpc.Core;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace asagiv.pushrocket.common.Models
{
    public sealed class DataPullSubscriber : IDataPullSubscriber
    {
        #region Fields
        private readonly IDisposable _pullSubscribe;
        private readonly Subject<IResponseStreamContext<DataPullResponse>> _dataRetrievedSubject;
        #endregion

        #region Properties
        public DataPush.DataPushClient Client { get; }
        public string DestinationNode { get; }
        public bool IsDisposed { get; private set; }
        public IObservable<IResponseStreamContext<DataPullResponse>> DataRetrievedObservable => _dataRetrievedSubject.AsObservable();
        #endregion

        #region Constructor
        public DataPullSubscriber(DataPush.DataPushClient client, string node)
        {
            Client = client;

            DestinationNode = node;

            // Check every second if there is data to be pulled.
            var pullObservable = Observable.Interval(TimeSpan.FromSeconds(1));

            _pullSubscribe = pullObservable
                .SelectMany(x => PollDataAsync())
                .Subscribe();
        }
        #endregion

        #region Methods
        private async Task<bool> PollDataAsync()
        {
            // Generate the data pull request, send via gRPC to the server.
            var request = new DataPullRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                DestinationNode = DestinationNode
            };

            var pullResponse = Client.PullData(request);

            // Check if any data is to be pulled.
            if (await pullResponse.ResponseStream.MoveNext() && !string.IsNullOrEmpty(pullResponse.ResponseStream.Current.Name))
            {
                // Create a context for pulling the data from the server.
                var responseStreamContext = new ResponseStreamContext<DataPullResponse>(pullResponse.ResponseStream.Current, pullResponse.ResponseStream);

                _dataRetrievedSubject.OnNext(responseStreamContext);

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