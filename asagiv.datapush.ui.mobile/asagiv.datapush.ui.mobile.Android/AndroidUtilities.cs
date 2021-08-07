using Android.Content;
using Android.Net;
using Android.Webkit;
using asagiv.datapush.ui.mobile.Utilities;
using asagiv.datapush.ui.mobile.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace asagiv.datapush.ui.mobile.Droid
{
    public static class AndroidUtilities
    {
        public async static Task SendDataAsync(Intent intent, Context context)
        {
            var viewModel = GetViewModel();

            if (viewModel == null)
            {
                return;
            }

            if (!viewModel.IsConnected)
            {
                LoggerInstance.Instance.Log.Information("Connecting to Server.");

                await viewModel.ConnectToServerAsync();
            }

            var shareStreamContexts = GetClipData(intent, context).ToList();

            await viewModel.PushShareStreamContexts(shareStreamContexts);
        }

        private static IEnumerable<ShareStreamContext> GetClipData(Intent intent, Context context)
        {
            // Get Clip Data URIs.
            var clipData = Enumerable.Range(0, intent.ClipData.ItemCount)
                .Select(x => intent.ClipData.GetItemAt(x).Uri)
                .ToList();

            LoggerInstance.Instance.Log.Information($"Found {clipData} clips.");

            // Create Streams
            var shareStreamContextEnumerable = clipData.Select(x => GetShareStreamContext(context, x));

            return shareStreamContextEnumerable;
        }

        private static ShareStreamContext GetShareStreamContext(Context context, Uri x)
        {
            string filePath, fileName = null;

            filePath = FilesHelper.GetActualPathForFile(x, context);

            if (filePath != null)
            {
                LoggerInstance.Instance.Log.Information($"File Name Found: {filePath}.");

                fileName = Path.GetFileNameWithoutExtension(filePath);
            }

                        // Get Data Type.
            var mimeType = context.ContentResolver.GetType(x);

            // Get Extension from Mime Type.
            var extension = MimeTypeMap.Singleton.GetExtensionFromMimeType(mimeType);

            LoggerInstance.Instance.Log.Information($"File Found of Type: {extension}.");

            return new ShareStreamContext(context.ContentResolver.OpenInputStream(x), extension, fileName);
        }

        private static DataPushViewModel GetViewModel()
        {
            var service = App.ServiceProvider.GetService(typeof(DataPushViewModel));

            if (!(service is DataPushViewModel viewModel))
            {
                return null;
            }

            return viewModel;
        }
    }
}