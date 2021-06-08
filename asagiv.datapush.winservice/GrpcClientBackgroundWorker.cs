using asagiv.datapush.common.Utilities;
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
        private readonly GrpcDataDownloader _downloader;
        private readonly GrpcClient _client;
        #endregion

        public GrpcClientBackgroundWorker(ILogger logger, GrpcDataDownloader downloader, GrpcClient client)
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
