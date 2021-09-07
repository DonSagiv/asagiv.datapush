using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using Serilog;

namespace asagiv.datapush.ui.mobile.Droid
{
    [Activity(Label = "PushRocket", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"image/*")]
    [IntentFilter(new[] { Intent.ActionSendMultiple }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"image/*")]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"video/*")]
    [IntentFilter(new[] { Intent.ActionSendMultiple }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = @"video/*")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        #region Fields
        private ILogger _logger;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            _logger = App.ServiceProvider.GetService(typeof(ILogger)) as ILogger;

            // Do nothing if no intents found.
            if (Intent == null)
            {
                return;
            }

            if (Intent.Action == Intent.ActionSend || Intent.Action == Intent.ActionSendMultiple)
            {
                _logger?.Information("Intents Found.");

                // Send data from intents (if app launched from "Share" menu).
                AndroidUtilities.PrepareDataToSend(Intent, ApplicationContext, _logger);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (intent.Action == Intent.ActionSend || intent.Action == Intent.ActionSendMultiple)
            {
                _logger.Information("Intents Found.");

                // Send data from intents (if app launched from "Share" menu).
                AndroidUtilities.PrepareDataToSend(Intent, ApplicationContext, _logger);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}