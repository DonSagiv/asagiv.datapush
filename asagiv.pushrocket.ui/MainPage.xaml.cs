using asagiv.pushrocket.ui.Interfaces;
using asagiv.pushrocket.ui.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace asagiv.pushrocket.ui
{
    public partial class MainPage : ContentPage
    {
        #region Statics
        private static bool isSetup = false;
        #endregion

        #region Constructor
        public MainPage()
        {
            InitializeComponent();

            if (!isSetup)
            {
                isSetup = true;

                SetupTrayIcon();
            }
        }
        #endregion

        #region Methods
        private void SetupTrayIcon()
        {
            var trayService = MauiAppServices.Instance.GetService<ITrayService>();
            var notificationService = MauiAppServices.Instance.GetService<INotificationService>();

            trayService.ClickHandler = () => notificationService.ShowNotification("I am running!", "This is a test!");

            trayService?.Initialize();
        }
        #endregion
    }
}