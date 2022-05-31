using asagiv.pushrocket.ui.common;
using asagiv.pushrocket.ui.common.Platforms.Windows;

namespace asagiv.pushrocket.ui.WinUI
{
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            this.InitializeComponent();

            WindowsExtensions.SetUpWindow(WindowStartupLocation.Center, 600, 800);
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}