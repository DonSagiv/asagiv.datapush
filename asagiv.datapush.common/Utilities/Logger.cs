using System;

namespace asagiv.datapush.common.Utilities
{
    public class Logger
    {
        #region Statics
        private static readonly Lazy<Logger> _lazyInstance = new Lazy<Logger>(() => new Logger());
        public static Logger Instance => _lazyInstance.Value;
        #endregion

        #region Delegates
        public event EventHandler<string> LogEntryAdd;
        #endregion

        #region Methods
        public void Append(string message)
        {
            var logEntry = $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}: {message}";
            LogEntryAdd?.Invoke(this, message);
        }
        #endregion

    }
}
