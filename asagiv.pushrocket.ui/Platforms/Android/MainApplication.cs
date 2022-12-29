using Android.App;
using Android.Runtime;
using Serilog;

namespace asagiv.pushrocket.ui
{
    [Application]
    [assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage)]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp();
        }
    }
}