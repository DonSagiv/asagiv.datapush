using asagiv.common.Logging;
using asagiv.pushrocket.ui.common.Interfaces;
using Serilog;

namespace asagiv.pushrocket.ui.common
{
    // All the code in this file is only included on Android.
    public class PlatformSpecificData : IPlatformSpecificData
    {
        #region Properties
        public ILogger logger { get; }
        public string LogFileDirectory { get; }
        #endregion

        #region Constructor
        public PlatformSpecificData()
        {
            logger = LoggerFactory.CreateLogger();
        }
        #endregion
    }
}