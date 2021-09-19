using asagiv.datapush.common.Interfaces;
using asagiv.datapush.common.Utilities;
using asagiv.datapush.ui.common.Interfaces;
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
                .AddSingleton<IClientSettingsViewModel, WindowsClientSettingsViewModel>()
                .AddSingleton<IClientSettingsModel, WindowsClientSettingsModel>()
                .AddSingleton<IConnectionSettingsViewModel, ConnectionSettingsViewModel>()
                .AddSingleton<IPullNodeSettingsViewModel, WindowsServiceViewModel>()
                .AddSingleton<IPullNodeSettingsModel, WindowsServiceSettingsModel>()
                .AddDbContext<WinUiDataPushDbContext>()
                .BuildServiceProvider();

            base.OnStartup(e);

            _mainWindow = ServiceProvider.GetService(typeof(MainWindow)) as MainWindow;

            _mainWindow.Show();
        }
    }
}
