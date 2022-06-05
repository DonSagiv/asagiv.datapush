﻿using asagiv.common.Logging;
using asagiv.pushrocket.ui.common.Utilities;
using asagiv.pushrocket.ui.common.ViewModels;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;

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

            builder.Services.AddSingleton<WaitIndicatorService>();
            builder.Services.UseSerilog();
            builder.Services.AddSingleton<MainViewModel>();

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            builder.Services
                .AddBlazorise(o => o.Immediate = false)
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            return builder.Build();
        }
    }
}