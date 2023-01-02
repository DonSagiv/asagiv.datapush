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
        public MainPage(ITrayService trayService)
        {
            InitializeComponent();

            if (!isSetup)
            {
                trayService?.Initialize();

                isSetup = true;
            }
        }
        #endregion
    }
}