namespace asagiv.pushrocket.interop.Platforms.Windows
{
    public enum WindowMessages : uint
    {
        WM_CONTEXTMENU = 0x007b,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_DPICHANGED = 0x02e0,
        WM_USER = 0x0400,
        NIN_SELECT = WM_USER,
        NIN_KEYSELECT = WM_USER + 1,
        NIN_BALLOONSHOW = WM_USER + 2,
        NIN_BALLOONHIDE = WM_USER + 3,
        NIN_BALLOONTIMEOUT = WM_USER + 4,
        NIN_BALLOONUSERCLICK = WM_USER + 5,
        NIN_POPUPOPEN = WM_USER + 6,
        NIN_POPUPCLOSE = WM_USER + 7,
    }
}
