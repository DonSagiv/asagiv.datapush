using asagiv.pushrocket.common.Interfaces;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace asagiv.pushrocket.winservice
{
    public class GrpcClientBackgroundWorker : BackgroundService
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly IGrpcDataDownloader _downloader;
        private readonly IGrpcClient _client;
        #endregion

        public GrpcClientBackgroundWorker(ILogger logger, IGrpcDataDownloader downloader, IGrpcClient client)
        {
            _logger = logger;
            _downloader = downloader;
            _client = client;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger?.Information("Initializing Data Push Windows Service.");

                await _client.CreatePullSubscriberAsync();
                await _client.RegisterNodeAsync(true);

                _client.DataRetrievedObservable
                    .SelectMany(x => _downloader.OnDataRetrievedAsync(x))
                    .Subscribe();

                _downloader.AcknowledgeDelivery += async (_, e) => await _client.AcknowledgeDeliveryAsync(e);

                while (!stoppingToken.IsCancellationRequested && !_client.IsDisposed)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }
    }
}
