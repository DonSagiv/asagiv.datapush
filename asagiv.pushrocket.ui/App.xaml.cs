using System.Diagnostics;

namespace asagiv.pushrocket.ui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
    }
}