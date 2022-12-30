using System.ComponentModel;
using System.Diagnostics;

namespace asagiv.pushrocket.wininterop
{
    public class WindowMessageSink : IDisposable
    {
        public const int CallbackMessageId = 0x400;
        private uint taskbarRestartMessageId;
        private bool isDoubleClick;
        private WindowProcedureHandler messageHandler;
        internal string WindowId { get; private set; }
        internal IntPtr MessageWindowHandle { get; private set; }
        public byte Version => 0x4;

        public event Action<bool> ChangeToolTipStateRequest;
        public event Action<MouseEvent> MouseEventReceived;
        public event Action<bool> BalloonToolTipChanged;
        public event Action TaskbarCreated;

        public WindowMessageSink()
        {
            CreateMessageWindow();
        }

        internal static WindowMessageSink CreateEmpty()
        {
            return new WindowMessageSink
            {
                MessageWindowHandle = IntPtr.Zero,
            };
        }

        private void CreateMessageWindow()
        {
            WindowId = "WPFTaskbarIcon_" + Guid.NewGuid();

            messageHandler = OnWindowMessageReceived;

            WindowClass wc;

            wc.style = 0;
            wc.lpfnWndProc = messageHandler;
            wc.cbClsExtra = 0;
            wc.cbWndExtra = 0;
            wc.hInstance = IntPtr.Zero;
            wc.hIcon = IntPtr.Zero;
            wc.hCursor = IntPtr.Zero;
            wc.hbrBackground = IntPtr.Zero;
            wc.lpszMenuName = string.Empty;
            wc.lpszClassName = WindowId;

            WinApi.RegisterClass(ref wc);

            taskbarRestartMessageId = WinApi.RegisterWindowMessage("TaskbarCreated");

            MessageWindowHandle = WinApi.CreateWindowEx(0, WindowId, "", 0, 0, 0, 1, 1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            if(MessageWindowHandle == IntPtr.Zero)
            {
                throw new Win32Exception("Message window handle was not a valid pointer");
            }
        }

        private IntPtr OnWindowMessageReceived(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if(messageId == taskbarRestartMessageId)
            {
                var listener = TaskbarCreated;
                listener?.Invoke();
            }

            ProcessWindowMessage(messageId, wParam, lParam);

            return WinApi.DefWindowProc(hWnd, messageId, wParam, lParam);
        }

        private void ProcessWindowMessage(uint msg, IntPtr wParam, IntPtr lParam)
        {
            if(msg != CallbackMessageId)
            {
                switch ((WindowMessages)msg)
                {
                    case WindowMessages.WM_DPICHANGED:
                        Debug.WriteLine("DPI Change");
                        break;
                }

                return;
            }

            var message = (WindowMessages)lParam.ToInt32();
            Debug.WriteLine("Got Message " + message);

            switch (message)
            {
                case WindowMessages.WM_CONTEXTMENU:
                    Debug.WriteLine("Unhandled WM_CONTEXTMENU");
                    break;
                case WindowMessages.WM_MOUSEMOVE:
                    MouseEventReceived?.Invoke(MouseEvent.MouseMove);
                    break;
                case WindowMessages.WM_LBUTTONDOWN:
                    MouseEventReceived?.Invoke(MouseEvent.IconLeftMouseDown);
                    break;
                case WindowMessages.WM_LBUTTONUP:
                    if(!isDoubleClick)
                    {
                        MouseEventReceived?.Invoke(MouseEvent.IconLeftMouseUp);
                    }
                    isDoubleClick = false;
                    break;
                case WindowMessages.WM_LBUTTONDBLCLK:
                    isDoubleClick = true;
                    MouseEventReceived?.Invoke(MouseEvent.IconDoubleClick);
                    break;
                case WindowMessages.WM_RBUTTONDOWN:
                    MouseEventReceived?.Invoke(MouseEvent.IconRightMouseDown);
                    break;
                case WindowMessages.WM_RBUTTONUP:
                    MouseEventReceived?.Invoke(MouseEvent.IconRightMouseUp);
                    break;
                case WindowMessages.WM_RBUTTONDBLCLK:
                    // Do not trigger anything.
                    break;
                case WindowMessages.WM_MBUTTONDOWN:
                    MouseEventReceived?.Invoke(MouseEvent.IconMiddleMouseDown);
                    break;
                case WindowMessages.WM_MBUTTONUP:
                    MouseEventReceived?.Invoke(MouseEvent.IconMiddleMouseUp);
                    break;
                case WindowMessages.WM_MBUTTONDBLCLK:
                    // Do not trigger anything.
                    break;
                case WindowMessages.NIN_BALLOONSHOW:
                    ChangeToolTipStateRequest?.Invoke(true);
                    break;
                case WindowMessages.NIN_BALLOONHIDE:
                case WindowMessages.NIN_BALLOONTIMEOUT:
                    BalloonToolTipChanged?.Invoke(false);
                    break;
                case WindowMessages.NIN_BALLOONUSERCLICK:
                    MouseEventReceived?.Invoke(MouseEvent.BaloonToolTipClicked);
                    break;
                case WindowMessages.NIN_POPUPOPEN:
                    ChangeToolTipStateRequest?.Invoke(true);
                    break;
                case WindowMessages.NIN_POPUPCLOSE:
                    ChangeToolTipStateRequest?.Invoke(false);
                    break;
                case WindowMessages.NIN_SELECT:
                    Debug.WriteLine("Unhandled NIN_SELECT");
                    break;
                case WindowMessages.NIN_KEYSELECT:
                    Debug.WriteLine("Unhandled NIN_KEYSELECT");
                    break;
                default:
                    Debug.WriteLine("Unhandled NotifyIcon message ID: " + lParam);
                    break;
            }
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WindowMessageSink()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            WinApi.DestroyWindow(MessageWindowHandle);

            messageHandler = null;
        }
    }
}
