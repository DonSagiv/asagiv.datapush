using asagiv.pushrocket.common.Interfaces;

namespace asagiv.pushrocket.ui.common
{
    // All the code in this file is only included on iOS.
    public class PlatformServices : IPlatformServices
    {
        public string GetDownloadDirectory()
        {
            throw new NotImplementedException();
        }
    }
}