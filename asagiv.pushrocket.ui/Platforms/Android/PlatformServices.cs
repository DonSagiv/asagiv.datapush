using asagiv.pushrocket.common.Interfaces;

namespace asagiv.pushrocket.ui
{
    // All the code in this file is only included on Android.
    public class PlatformServices : IPlatformServices
    {
        private const string mainDownloadDir = "/storage/emulated/0/Download";

        public string GetDownloadDirectory()
        {
            // var docsDirectory = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
            // return docsDirectory.AbsolutePath;

            return mainDownloadDir;
        }
    }
}