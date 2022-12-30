using asagiv.pushrocket.common.Interfaces;

namespace asagiv.pushrocket.ui
{
    public class PlatformServices : IPlatformServices
    {
        public string GetDownloadDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}
