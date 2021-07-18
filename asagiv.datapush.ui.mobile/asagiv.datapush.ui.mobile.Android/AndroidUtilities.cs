using Android.Content;
using Android.Net;
using Android.Webkit;
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
                await viewModel.ConnectToServerAsync();
            }

            var uriList = GetClipData(intent, context).ToList();

            // await viewModel.PushFilesAsync(uriList);
        }

        private static IEnumerable<Uri> GetClipData(Intent intent, Context context)
        {
            var a = ContentResolver.QueryArgGroupColumns.ToList();

            var clipData = Enumerable.Range(0, intent.ClipData.ItemCount)
                .Select(x => intent.ClipData.GetItemAt(x).Uri)
                .ToList();

            var firstClip = clipData.FirstOrDefault();

            // Get Data Type.
            var mimeType = context.ContentResolver.GetType(firstClip);
            var extension = MimeTypeMap.Singleton.GetExtensionFromMimeType(mimeType);

            using var stream = context.ContentResolver.OpenInputStream(firstClip);

            using var ms = new MemoryStream();

            stream.CopyTo(ms);

            var data = ms.ToArray();

            return clipData;
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