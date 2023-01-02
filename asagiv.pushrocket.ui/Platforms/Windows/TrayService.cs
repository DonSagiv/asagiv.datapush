using asagiv.pushrocket.interop.Platforms.Windows;
using asagiv.pushrocket.ui.Interfaces;

namespace asagiv.pushrocket.ui
{
    public class TrayService : ITrayService
    {
        #region Delegates
        public Action ClickHandler { get; set; }
        #endregion

        #region Methods
        public void Initialize()
        {
            var tray = new WindowsTrayIcon("Platforms/Windows/trayicon.ico", "PushRocket")
            {
                LeftClick = () =>
                {
                    WindowsExtensions.BringToFront();
                    ClickHandler?.Invoke();
                }
            };
        }
        #endregion
    }
}
