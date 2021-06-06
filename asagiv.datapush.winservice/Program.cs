using asagiv.datapush.common.Utilities;
using asagiv.datapush.winservice.Utilities;
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
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(GrpcClientFactory.CreateGprcClient);
                    services.AddSingleton<GrpcDataDownloader>();
                    services.AddSingleton(LoggerFactory.CreateLoggerWindows);
                    services.AddHostedService<GrpcClientBackgroundWorker>();
                });
    }
}
