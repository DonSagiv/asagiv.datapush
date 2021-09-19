using asagiv.datapush.common.Utilities;
using asagiv.datapush.ui.common.Interfaces;
using asagiv.datapush.ui.mobile.Models;
using asagiv.datapush.ui.mobile.Utilities;
using asagiv.datapush.ui.mobile.ViewModels;
using asagiv.datapush.ui.mobile.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace asagiv.datapush.ui.mobile
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider;

        public App()
        {
            ServiceProvider = new ServiceCollection()
                .AddSingleton(x => LoggerFactory.CreateLogger(FileSystem.AppDataDirectory))
                .AddSingleton<MainPageView>()
                .AddSingleton<MainPageViewModel>()
                .AddSingleton<IClientSettingsViewModel, ClientSettingsViewModel>()
                .AddSingleton<IClientSettingsModel, ClientSettingsModel>()
                .AddSingleton<IConnectionSettingsViewModel, ConnectionSettingsViewModel>()
                .AddDbContext<XFormsDataPushDbContext>()
                .BuildServiceProvider();

            InitializeComponent();

            MainPage = ServiceProvider.GetService(typeof(MainPageView)) as MainPageView;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public void Test()
        {

        }
    }
}
