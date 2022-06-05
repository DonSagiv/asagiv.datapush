using asagiv.common.Logging;
using asagiv.pushrocket.ui.common.Interfaces;
using Serilog;

namespace asagiv.pushrocket.ui.common
{
    public class PlatformSpecificData : IPlatformSpecificData
    {
        #region Properties
        public ILogger logger { get; }
        #endregion

        #region Constructor
        public PlatformSpecificData()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var logFileDirectory = Path.Combine(appDataFolder, "PushRocket", "Logs");

            logger = LoggerFactory.CreateLogger(logFileDirectory);
        }
        #endregion
    }
}
