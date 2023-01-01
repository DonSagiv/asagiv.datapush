using Android.Content;
using asagiv.pushrocket.common.Interfaces;
using asagiv.pushrocket.ui.Utilities;

namespace asagiv.pushrocket.ui.Platforms.Android
{
    public static class AndroidUtilities
    {
        public static void ExtractClipData(Intent intent, Context context, ISourceStreamPubSub pushDataPubSub)
        {
            var files = Enumerable.Range(0, intent.ClipData.ItemCount)
                .Select(x => intent.ClipData.GetItemAt(x).Uri)
                .Select(x => 
                {
                    var file = Path.GetFileName(FilesHelper.GetActualPathForFile(x, context));
                    var streamTask = Task.FromResult(context.ContentResolver.OpenInputStream(x));

                    return new SourceStreamInfo(file, streamTask);
                });

            pushDataPubSub.PublishSourceStreams(files);
        }
    }
}
