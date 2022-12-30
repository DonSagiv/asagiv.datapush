using asagiv.common.Logging;
using asagiv.pushrocket.common.Interfaces;
using asagiv.pushrocket.common.Models;
using asagiv.pushrocket.common.Utilities;
using asagiv.pushrocket.ui.Database;
using asagiv.pushrocket.ui.Interfaces;
using asagiv.pushrocket.ui.Utilities;
using asagiv.pushrocket.ui.ViewModels;
using Microsoft.Maui.LifecycleEvents;
using MudBlazor.Services;

namespace asagiv.pushrocket.ui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>()
                .ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

            var appDataDirectory = FileSystem.Current.AppDataDirectory;

            builder.Services.UseSerilog(appDataDirectory);
            builder.Services.AddSingleton<IClientSettingsModel, ClientSettingsModel>();
            builder.Services.AddSingleton<PushRocketDatabase>();
            builder.Services.AddSingleton<WaitIndicatorService>();
            builder.Services.AddSingleton<DarkModeService>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<ConnectionSettingsViewModel>();
            builder.Services.AddSingleton<IPlatformServices, PlatformServices>();
            builder.Services.AddSingleton<IGrpcDataDownloader, GrpcDataDownloader>();
            builder.Services.AddMudServices();
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
#if WINDOWS
            builder.Services.AddSingleton<ITrayService, TrayService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();
#endif

            builder.ConfigureLifecycleEvents(lc =>
            {
#if WINDOWS
                lc.AddWindows(w => w.OnWindowCreated((del) => 
                {
                    del.ExtendsContentIntoTitleBar = true;
                    // WindowsExtensions.SetIcon("Platforms/Windows/trayIcon.ico");
                }));
#endif
            });

            var app = builder.Build();

            // Allow app services to be accessed in static context.
            MauiAppServices.Instance.InjectServiceProvider(app.Services);

            return builder.Build();
        }
    }
}