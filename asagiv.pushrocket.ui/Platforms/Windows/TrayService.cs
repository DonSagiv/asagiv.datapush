using asagiv.pushrocket.ui.Interfaces;
using asagiv.pushrocket.wininterop;

namespace asagiv.pushrocket.ui
{
    public class TrayService : ITrayService
    {
        private WindowsTrayIcon _tray;

        public Action ClickHandler { get; set; }

        public void Initialize()
        {
            _tray = new WindowsTrayIcon("Platforms/Windows/trayicon.ico", "PushRocket")
            {
                LeftClick = () =>
                {
                    WindowsExtensions.BringToFront();
                    ClickHandler?.Invoke();
                }
            };
        }
    }
}
