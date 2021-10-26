using Android.Content;
using Android.Net;
using Android.Webkit;
using asagiv.datapush.ui.common.Interfaces;
using asagiv.datapush.ui.mobile.Utilities;
using asagiv.datapush.ui.mobile.ViewModels;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace asagiv.datapush.ui.mobile.Droid
{
    public static class AndroidUtilities
    {
        public static void PrepareDataToSend(Intent intent, Context context, ILogger logger = null)
        {
            var viewModel = GetViewModel();

            var shareStreamContexts = GetClipData(intent, context, logger).ToList();

            viewModel.PrepareShareStreamContexts(shareStreamContexts);
        }

        private static IEnumerable<ShareStreamContext> GetClipData(Intent intent, Context context, ILogger logger = null)
        {
            // Get Clip Data URIs.
            var clipData = Enumerable.Range(0, intent.ClipData.ItemCount)
                .Select(x => intent.ClipData.GetItemAt(x).Uri)
                .ToList();

            logger?.Information($"Found {clipData} clips.");

            // Create Streams
            var shareStreamContextEnumerable = clipData.Select(x => GetShareStreamContext(context, x, logger));

            return shareStreamContextEnumerable;
        }

        private static ShareStreamContext GetShareStreamContext(Context context, Uri x, ILogger logger = null)
        {
            string filePath, fileName = null;

            filePath = FilesHelper.GetActualPathForFile(x, context);

            if (filePath != null)
            {
                logger.Information($"File Name Found: {filePath}.");

                fileName = Path.GetFileNameWithoutExtension(filePath);
            }

            // Get Data Type.
            var mimeType = context.ContentResolver.GetType(x);

            // Get Extension from Mime Type.
            var extension = MimeTypeMap.Singleton.GetExtensionFromMimeType(mimeType);

            logger.Information($"File Found of Type: {extension}.");

            return new ShareStreamContext(context.ContentResolver.OpenInputStream(x), extension, fileName);
        }

        public static ClientSettingsViewModel GetViewModel()
        {
            var service = App.ServiceProvider.GetService(typeof(IClientSettingsViewModel));

            return service is ClientSettingsViewModel viewModel ? viewModel : null;
        }
    }
}