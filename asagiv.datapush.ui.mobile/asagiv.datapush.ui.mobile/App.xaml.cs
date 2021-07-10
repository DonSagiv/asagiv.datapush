using asagiv.datapush.ui.mobile.Views;
using Xamarin.Forms;

namespace asagiv.datapush.ui.mobile
{
    public partial class App : Application
    {
        public App()
        {
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
    }
}
