using asagiv.datapush.common.Interfaces;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace asagiv.datapush.winservice
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

                _client.DataRetrieved += async (s, e) => await _downloader.OnDataRetrievedAsync(e);
                _downloader.AcknowledgeDelivery += async (s, e) => await _client.AcknowledgeDeliveryAsync(e);

                while (!stoppingToken.IsCancellationRequested && !_client.IsDisposed)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }
    }
}
