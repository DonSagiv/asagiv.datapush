using asagiv.pushrocket.wininterop;
using System.Diagnostics;

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