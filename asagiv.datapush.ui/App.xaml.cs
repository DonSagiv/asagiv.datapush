using asagiv.datapush.common.Utilities;
using asagiv.datapush.ui.Models;
using asagiv.datapush.ui.Utilities;
using asagiv.datapush.ui.ViewModels;
using asagiv.datapush.ui.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace asagiv.datapush.ui
{
    public partial class App : Application
    {
        #region Statics
        public static IServiceProvider ServiceProvider;
        #endregion

        #region Fields
        private MainWindow _mainWindow;
        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceProvider = new ServiceCollection()
                .AddSingleton(LoggerFactory.CreateLoggerWindows)
                .AddSingleton<MainWindow>()
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<WindowsClientViewModel>()
                .AddSingleton<WindowsClientSettingsModel>()
                .AddSingleton<ConnectionSettingsViewModel>()
                .AddDbContext<WinUiDataPushDbContext>()
                .AddSingleton<WindowsServiceViewModel>()
                .AddSingleton<WindowsServiceSettingsModel>()
                .BuildServiceProvider();

            base.OnStartup(e);

            _mainWindow = ServiceProvider.GetService(typeof(MainWindow)) as MainWindow;

            _mainWindow.Show();
        }
    }
}
