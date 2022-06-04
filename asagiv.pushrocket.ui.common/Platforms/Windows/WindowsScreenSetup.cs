using asagiv.pushrocket.ui.common.Platforms.Windows;
using Microsoft.Maui.Handlers;
using System.Windows.Forms;
using Windows.Graphics;

namespace asagiv.pushrocket.ui.common
{
    // All the code in this file is only included on Windows.
    public static class WindowsExtensions
    {
        public static void SetUpWindow(WindowStartupLocation startupLocation = WindowStartupLocation.None, int width = 1000, int height = 800)
        {
            WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, _) =>
            {
                var mauiWindow = handler.VirtualView;
                var nativeWindow = handler.PlatformView;
                nativeWindow.Activate();
                IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                var currentScreen = startupLocation switch
                {
                    WindowStartupLocation.None => Screen.FromPoint(Cursor.Position),
                    WindowStartupLocation.Center => Screen.FromPoint(Cursor.Position),
                    WindowStartupLocation.CenterPrimary => Screen.PrimaryScreen,
                    _ => throw new NotImplementedException()
                };

                var bounds = currentScreen.Bounds;

                if(startupLocation == WindowStartupLocation.None)
                {
                    appWindow.Resize(new SizeInt32(width, height));
                }
                else
                {
                    var X0 = ((bounds.Width - width) / 2) + bounds.Left;
                    var Y0 = ((bounds.Height - height) / 2) + bounds.Top;

                    appWindow.MoveAndResize(new RectInt32(X0, Y0, width, height));
                }
            });
        }
    }
}