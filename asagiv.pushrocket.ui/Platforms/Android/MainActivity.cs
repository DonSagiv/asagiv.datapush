using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using asagiv.pushrocket.ui.Utilities;
using Serilog;

namespace asagiv.pushrocket.ui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "image/*")]
    [IntentFilter(new[] { Intent.ActionSendMultiple }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "image/*")]
    [IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "video/*")]
    [IntentFilter(new[] { Intent.ActionSendMultiple }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "video/*")]
    public class MainActivity : MauiAppCompatActivity
    {
        #region Fields
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        public MainActivity()
        {
            _logger = MauiAppServices.Instance.GetService<ILogger>();

            _logger?.Information("Android MainActivity Instantiated.");
        }
        #endregion

        #region Methods
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _logger?.Information("Android App OnCreate");

            // Do nothing if no intents found.
            if (Intent == null)
            {
                return;
            }

            if (Intent.Action == Intent.ActionSend || Intent.Action == Intent.ActionSendMultiple)
            {
                _logger?.Information($"Found Intent Action: {Intent.Action}");
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            _logger?.Information("Android App On New Intent");

            if (intent.Action == Intent.ActionSend || intent.Action == Intent.ActionSendMultiple)
            {
                _logger?.Information($"Found Intent Action: {Intent.Action}");
            }
        }
        #endregion
    }
}