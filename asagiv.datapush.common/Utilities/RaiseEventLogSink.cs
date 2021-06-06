using System;
using Serilog.Core;
using Serilog.Events;

namespace asagiv.datapush.common.Utilities
{
    public class RaiseEventLogSink : ILogEventSink
    {
        #region Fields
        private readonly IFormatProvider _formatProvider;
        #endregion

        #region Delegates
        public event EventHandler<string> LogEventRaised;
        #endregion

        #region Constructor
        public RaiseEventLogSink(IFormatProvider formatProvider = null)
        {
            _formatProvider = formatProvider;
        }
        #endregion

        public void Emit(LogEvent logEvent)
        {
            var logEntry = logEvent.RenderMessage(_formatProvider);
            LogEventRaised?.Invoke(this, logEntry);
        }
    }
}
