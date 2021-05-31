using Serilog;

namespace asagiv.datapush.server.common
{
    public static class LoggerFactory
    {
        public static ILogger CreateLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
