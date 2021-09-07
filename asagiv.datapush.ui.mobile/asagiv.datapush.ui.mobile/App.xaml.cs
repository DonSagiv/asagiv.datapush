﻿using asagiv.datapush.common.Utilities;
using asagiv.datapush.ui.mobile.Models;
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
                .AddSingleton<ClientSettingsViewModel>()
                .AddSingleton<ClientSettingsModel>()
                .AddSingleton<ConnectionSettingsViewModel>()
                .BuildServiceProvider();

            InitializeComponent();

            MainPage = new MainPageView();
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
