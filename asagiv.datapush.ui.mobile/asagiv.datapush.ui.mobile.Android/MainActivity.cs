using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using System.Threading.Tasks;
using asagiv.datapush.ui.mobile.Utilities;

namespace asagiv.datapush.ui.mobile.Droid
{
    [Activity(Label = "asagiv.datapush.ui.mobile", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"image/*")]
    [IntentFilter(new[] { Intent.ActionSendMultiple }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"image/*")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            if (Intent == null)
            {
                return;
            }

            if (Intent.Action == Intent.ActionSend || Intent.Action == Intent.ActionSendMultiple)
            {
                LoggerInstance.Instance.Log.Information("Intents Found.");

                Task.Run(async () => await AndroidUtilities.SendDataAsync(Intent, ApplicationContext));
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (intent.Action == Intent.ActionSend || intent.Action == Intent.ActionSendMultiple)
            {
                LoggerInstance.Instance.Log.Information("Intents Found.");

                Task.Run(async () => await AndroidUtilities.SendDataAsync(intent, ApplicationContext));
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}