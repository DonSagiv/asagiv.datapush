using System.Runtime.InteropServices;

namespace asagiv.pushrocket.wininterop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NotifyIconData
    {
        public uint cbSize;
        public IntPtr WindowHandle;
        public uint TaskbarIconId;
        public IconDataMembers ValidMembers;
        public uint CallbackMessageId;
        public IntPtr IconHandle;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string ToolTipText;
        public IconState IconState;
        public IconState StateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string BalloonText;
        public uint VersionOrTimeout;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string BalloonTitle;
        public Guid TaskbarIconGuid;
        public IntPtr CustomBalloonIconHandle;

        public static NotifyIconData CreateDefault(IntPtr handle, string iconFile, string defaultTooltip)
        {
            var data = new NotifyIconData();

            if(Environment.OSVersion.Version.Major >= 6)
            {
                data.cbSize = (uint)Marshal.SizeOf(data);
            }
            else
            {
                data.cbSize = 952;
                data.VersionOrTimeout = 10;
            }

            data.WindowHandle = handle;
            data.TaskbarIconId = 0x0;
            data.CallbackMessageId = WindowMessageSink.CallbackMessageId;
            data.VersionOrTimeout = (uint)0x4;

            IntPtr hIcon = PInvoke.User32.LoadImage(IntPtr.Zero, iconFile,
                PInvoke.User32.ImageType.IMAGE_ICON, 16, 16, PInvoke.User32.LoadImageFlags.LR_LOADFROMFILE);

            data.IconHandle = hIcon;

            data.IconState = IconState.Hidden;
            data.StateMask = IconState.Hidden;

            data.ValidMembers = IconDataMembers.Message
                                | IconDataMembers.Icon
                                | IconDataMembers.Tip;

            data.ToolTipText = data.BalloonText = data.BalloonTitle = defaultTooltip ?? string.Empty;

            return data;
        }
    }
}
