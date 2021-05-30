using asagiv.common.Utilities;
using asagiv.datapush.common.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace asagiv.datapush.windowsservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(GrpcClientFactory.CreateGprcClient);
                    services.AddSingleton<GrpcFileDownloader>();
                    services.AddHostedService<GrpcBackgroundClientService>();
                });


    }
}
