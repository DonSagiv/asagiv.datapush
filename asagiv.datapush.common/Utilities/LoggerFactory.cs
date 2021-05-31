using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;

namespace asagiv.datapush.common.Utilities
{
    public static class LoggerFactory
    {
        #region Statics
        private static readonly LoggerConfiguration _defaultConfig = new LoggerConfiguration();
        private const string outputTemplate = "{Level:u} {Timestamp:yyyy-MM-dd hh:mm:ss.fff tt} [{ThreadId}] {Message}{NewLine}{Exception}";
        public static LoggerConfiguration DefaultConfig => _defaultConfig;
        #endregion

        public static ILogger CreateLogger(IServiceProvider serviceProvider)
        {
            string logPath = null;

            if (serviceProvider.GetService(typeof(IConfiguration)) is IConfiguration configuration)
            {
                logPath = configuration.GetSection("LogFileSinkPath").Value;
            }

            var config = _defaultConfig
                .MinimumLevel.Information()
                .Enrich.WithThreadId()
                .WriteTo.Console(outputTemplate:outputTemplate);

            if (!string.IsNullOrWhiteSpace(logPath))
            {
                var logDirectory = Path.GetDirectoryName(logPath);

                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                config = config.WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate:outputTemplate);
            };

            var logger = config.CreateLogger();

            logger.Information($"Logger Initialized. ({DateTime.Now.ToLongDateString()})");

            return logger;
        }
    }
}
