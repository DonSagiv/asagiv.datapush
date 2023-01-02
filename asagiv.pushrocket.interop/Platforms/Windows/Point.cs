using System.Runtime.InteropServices;

namespace asagiv.pushrocket.interop.Platforms.Windows
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }
}
