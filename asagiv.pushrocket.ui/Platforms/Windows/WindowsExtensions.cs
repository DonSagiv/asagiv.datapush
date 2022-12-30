using asagiv.pushrocket.ui.Platforms.Windows;
using Microsoft.Maui.Handlers;
using System.Windows.Forms;
using Windows.Graphics;

namespace asagiv.pushrocket.ui
{
    // All the code in this file is only included on Windows.
    public static class WindowsExtensions
    {
        public static IntPtr Hwnd { get; set; }

        public static void SetUpWindow(WindowStartupLocation startupLocation = WindowStartupLocation.None, int width = 1000, int height = 800)
        {
            WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, _) =>
            {
                var nativeWindow = handler.PlatformView;
                nativeWindow.Activate();
                Hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(Hwnd);
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

        public static void SetIcon(string iconFileName)
        {
            if(Hwnd == IntPtr.Zero)
            {
                return;
            }

            var hIcon = PInvoke.User32.LoadImage(IntPtr.Zero,
                iconFileName,
                PInvoke.User32.ImageType.IMAGE_ICON,
                16,
                16,
                PInvoke.User32.LoadImageFlags.LR_LOADFROMFILE);

            PInvoke.User32.SendMessage(Hwnd, PInvoke.User32.WindowMessage.WM_SETICON, (IntPtr)0, hIcon);
        }
        
        public static void BringToFront()
        {
            PInvoke.User32.ShowWindow(Hwnd, PInvoke.User32.WindowShowStyle.SW_SHOW);
            PInvoke.User32.ShowWindow(Hwnd, PInvoke.User32.WindowShowStyle.SW_RESTORE);
        }

        public static void MinimizeToTray()
        {
            PInvoke.User32.ShowWindow(Hwnd, PInvoke.User32.WindowShowStyle.SW_MINIMIZE);
            PInvoke.User32.ShowWindow(Hwnd, PInvoke.User32.WindowShowStyle.SW_HIDE);
        }
    }
}