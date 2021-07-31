using System.IO;

namespace asagiv.datapush.ui.mobile.Utilities
{
    public class ShareStreamContext
    {
        #region Properties
        public string ShareFileName { get; }
        public string Extension { get; }
        public Stream InputStream { get; }
        #endregion

        #region Constructor
        public ShareStreamContext(Stream inputStream, string extension, string shareFileName = null)
        {
            InputStream = inputStream;
            Extension = extension;
            ShareFileName = shareFileName;
        }
        #endregion
    }
}
