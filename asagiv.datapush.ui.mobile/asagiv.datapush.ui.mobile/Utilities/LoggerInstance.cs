using asagiv.datapush.common.Utilities;
using Serilog;
using System;
using Xamarin.Essentials;

namespace asagiv.datapush.ui.mobile.Utilities
{
    public class LoggerInstance
    {
        #region Statics
        private readonly static Lazy<LoggerInstance> _lazyInstance = new Lazy<LoggerInstance>(() => new LoggerInstance());
        public static LoggerInstance Instance => _lazyInstance.Value;
        #endregion

        #region Properties
        public ILogger Log { get; }
        #endregion

        #region Constructor
        public LoggerInstance()
        {
            var appDataDirectory = FileSystem.AppDataDirectory;

            Log = LoggerFactory.CreateLogger(appDataDirectory);

            Log.Information("Logger Initialized.");
        }
        #endregion

    }
}
