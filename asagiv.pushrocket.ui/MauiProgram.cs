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

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            builder.Services
                .AddBlazorise(o => o.Immediate = true)
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            return builder.Build();
        }
    }
}