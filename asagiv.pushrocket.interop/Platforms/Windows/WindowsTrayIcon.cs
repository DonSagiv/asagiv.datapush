using System.Diagnostics;

namespace asagiv.pushrocket.interop.Platforms.Windows
{
    public class WindowsTrayIcon
    {
        public static readonly object SyncRoot = new();

        private readonly object lockObject = new();
        private NotifyIconData iconData;
        private readonly WindowMessageSink messageSink;

        public Action LeftClick { get; set; }
        public Action RightClick { get; set; }
        public bool IsTaskBarIconCreated { get; private set; }

        public WindowsTrayIcon(string iconFile, string defaultTooltip)
        {
            messageSink = new WindowMessageSink();

            iconData = NotifyIconData.CreateDefault(messageSink.MessageWindowHandle, iconFile, defaultTooltip);

            CreateTaskbarIcon();

            messageSink.MouseEventReceived += MessageSink_MouseEventReceived;
            messageSink.TaskbarCreated += MessageSink_TaskbarCreated;
        }

        private void MessageSink_MouseEventReceived(MouseEvent obj)
        {
            if (obj == MouseEvent.IconLeftMouseUp)
            {
                LeftClick?.Invoke();
            }
            else if (obj == MouseEvent.IconRightMouseUp)
            {
                RightClick?.Invoke();
            }
        }

        private void MessageSink_TaskbarCreated()
        {
            RemoveTaskbarIcon();
            CreateTaskbarIcon();
        }

        private void CreateTaskbarIcon()
        {
            lock (lockObject)
            {
                if (IsTaskBarIconCreated)
                {
                    return;
                }

                const IconDataMembers members = IconDataMembers.Message
                                | IconDataMembers.Icon
                                | IconDataMembers.Tip;

                var status = WriteIconData(ref iconData, NotifyCommand.Add, members);

                if (!status)
                {
                    return;
                }

                SetVersion();

                IsTaskBarIconCreated = true;
            }
        }

        private void RemoveTaskbarIcon()
        {
            lock (lockObject)
            {
                if (!IsTaskBarIconCreated)
                {
                    return;
                }

                WriteIconData(ref iconData, NotifyCommand.Delete, IconDataMembers.Message);

                IsTaskBarIconCreated = false;
            }
        }

        private void SetVersion()
        {
            iconData.VersionOrTimeout = 0x4;

            var status = WinApi.Shell_NotifyIcon(NotifyCommand.SetVersion, ref iconData);

            if (!status)
            {
                Debug.Fail("Could not set version");
            }
        }

        public static bool WriteIconData(ref NotifyIconData data, NotifyCommand command)
        {
            return WriteIconData(ref data, command, data.ValidMembers);
        }

        public static bool WriteIconData(ref NotifyIconData data, NotifyCommand command, IconDataMembers flags)
        {
            data.ValidMembers = flags;

            lock (SyncRoot)
            {
                return WinApi.Shell_NotifyIcon(command, ref data);
            }
        }
    }
}
