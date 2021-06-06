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

        public static ILogger CreateLogger(string logFilePath)
        {
            var config = InitializeConfig();

            config = GetLogPathDirectory(logFilePath, config);

            var logger = config.CreateLogger();

            logger.Information($"Logger Initialized. ({DateTime.Now.ToLongDateString()})");

            return logger;
        }

        public static ILogger CreateLoggerXamarin(IServiceProvider serviceProvider)
        {
            var logEventSink = serviceProvider.GetService(typeof(RaiseEventLogSink)) as RaiseEventLogSink;

            var config = InitializeConfig()
                .WriteTo.Sink(logEventSink);

            var logger = config.CreateLogger();

            logger.Information($"Logger Initialized. ({DateTime.Now.ToLongDateString()})");

            return logger;
        }

        public static ILogger CreateLoggerWindows(IServiceProvider serviceProvider)
        {
            string logPath = null;

            if (serviceProvider.GetService(typeof(IConfiguration)) is IConfiguration configuration)
            {
                var appDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var appDataFolder = Path.Combine(appDataRoot, "asagiv_datapush");

                var logPathRaw = configuration.GetSection("LogFileSinkPath").Value;

                logPath = logPathRaw.Replace("{appdata}", appDataFolder);
            }

            var config = InitializeConfig();

#if DEBUG
            config = config.WriteTo.Console(outputTemplate: outputTemplate);
#endif

            config = GetLogPathDirectory(logPath, config);

            var logger = config.CreateLogger();

            logger.Information($"Logger Initialized. ({DateTime.Now.ToLongDateString()})");

            return logger;
        }

        private static LoggerConfiguration InitializeConfig()
        {
            return _defaultConfig
                .MinimumLevel.Information()
                .Enrich.WithThreadId();
        }

        private static LoggerConfiguration GetLogPathDirectory(string logPath, LoggerConfiguration config)
        {
            if (!string.IsNullOrWhiteSpace(logPath))
            {
                var logDirectory = Path.GetDirectoryName(logPath);

                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                config = config.WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: outputTemplate);
            }

            return config;
        }
    }
}