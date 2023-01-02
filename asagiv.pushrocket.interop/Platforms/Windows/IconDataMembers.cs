namespace asagiv.pushrocket.interop.Platforms.Windows
{
    [Flags]
    public enum IconDataMembers
    {
        Message = 0x01,
        Icon = 0x02,
        Tip = 0x04,
        Info = 0x10,
        Realtime = 0x40,
        UseLegacyTooltips = 0x80,
    }
}
