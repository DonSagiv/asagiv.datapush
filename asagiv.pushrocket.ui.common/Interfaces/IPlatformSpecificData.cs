using Serilog;

namespace asagiv.pushrocket.ui.common.Interfaces
{
    public interface IPlatformSpecificData
    {
        public ILogger logger { get; }
    }
}
