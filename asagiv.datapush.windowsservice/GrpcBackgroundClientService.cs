using asagiv.datapush.common.Utilities;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace asagiv.datapush.windowsservice
{
    public class GrpcBackgroundClientService : BackgroundService
    {
        #region Fields
        private readonly GrpcClient _client;
        private readonly GrpcFileDownloader _downloader;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public GrpcBackgroundClientService(GrpcClient client, GrpcFileDownloader downloader, ILogger logger)
        {
            _logger = logger;
            _downloader = downloader;
            _client = client;

            _downloader.SaveDirectory = @"C:\Users\DonSa\Desktop\Rob Photos";
        }
        #endregion

        #region Methods
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger?.Information("Executing Service.");

            await _client.CreatePullSubscriberAsync();
            await _client.RegisterNodeAsync(true);

            _client.DataRetrieved += async(s,e) => await _downloader.OnClientDataRetrievedAsync(e);

            while (!_client.IsDisposed && !stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        #endregion
    }
}
