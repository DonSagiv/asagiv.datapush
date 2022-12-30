using System.Runtime.InteropServices;

namespace asagiv.pushrocket.wininterop
{
    internal static class WinApi
    {
        private const string User32 = "user32.dll";

        [DllImport("shell32.Dll", CharSet = CharSet.Unicode)]
        public static extern bool Shell_NotifyIcon(NotifyCommand cmd, [In] ref NotifyIconData data);

        [DllImport(User32, EntryPoint = "CreateWindowExW", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(int dwExStyle,
            [MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
            int dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport(User32)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport(User32, EntryPoint = "RegisterClassW", SetLastError = true)]
        public static extern short RegisterClass(ref WindowClass lpWindowClass);

        [DllImport(User32, EntryPoint = "RegisterWindowMessageW")]
        public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPWStr)] string lpString);

        [DllImport(User32, SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport(User32)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(User32, CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool GetPhysicalCursorPos(ref Point lpPoint);

        [DllImport(User32, SetLastError = true)]
        public static extern bool GetCursorPos(ref Point lpPoint);
    }
}
