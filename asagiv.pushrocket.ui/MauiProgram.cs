﻿using asagiv.common.Logging;
using asagiv.pushrocket.ui.common.Database;
using asagiv.pushrocket.ui.common.Utilities;
using asagiv.pushrocket.ui.common.ViewModels;
using MudBlazor.Services;

namespace asagiv.pushrocket.ui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

            var appDataDirectory = FileSystem.Current.AppDataDirectory;

            builder.Services.AddSingleton<PushRocketDatabase>();
            builder.Services.AddSingleton<WaitIndicatorService>();
            builder.Services.AddSingleton<DarkModeService>();
            builder.Services.UseSerilog(appDataDirectory);
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<ConnectionSettingsViewModel>();

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            builder.Services.AddMudServices();

            return builder.Build();
        }
    }
}