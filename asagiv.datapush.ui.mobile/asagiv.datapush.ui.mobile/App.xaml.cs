using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace asagiv.datapush.ui.mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Do nothing for now.
        }

        protected override void OnSleep()
        {
            // Do nothing for now.
        }

        protected override void OnResume()
        {
            // Do nothing for now.
        }
    }
}
