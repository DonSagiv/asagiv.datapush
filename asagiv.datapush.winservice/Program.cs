using asagiv.common.Logging;
using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace asagiv.datapush.winservice
{
    public class Program
    {
        protected Program() { }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton(GrpcClientFactory.CreateGprcClient);
                    services.AddSingleton<IGrpcDataDownloader, GrpcDataDownloader>();
                    services.AddSingleton(LoggerFactory.CreateLoggerWindows);
                    services.AddHostedService<GrpcClientBackgroundWorker>();
                });
    }
}
