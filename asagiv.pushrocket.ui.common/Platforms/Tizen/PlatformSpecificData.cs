using asagiv.pushrocket.ui.common.Interfaces

namespace asagiv.pushrocket.ui.common
{
    // All the code in this file is only included on Tizen.
    public class PlatformSpecificData : IPlatformSpecificData
    {
        public string LogFileDirectory { get; }
    }
}