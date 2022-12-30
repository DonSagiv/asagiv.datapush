using asagiv.pushrocket.common.Interfaces;

namespace asagiv.pushrocket.ui
{
    // All the code in this file is only included on Android.
    public class PlatformServices : IPlatformServices
    {
        public string GetDownloadDirectory()
        {
            return FileSystem.Current.AppDataDirectory;
        }
    }
}