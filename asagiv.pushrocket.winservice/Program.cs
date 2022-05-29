using asagiv.common.Logging;
using asagiv.pushrocket.common.Interfaces;
using asagiv.pushrocket.common.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace asagiv.pushrocket.winservice
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
                    services.UseSerilog();
                    services.AddHostedService<GrpcClientBackgroundWorker>();
                });
    }
}
