namespace asagiv.pushrocket.interop.Platforms.Windows
{
    public enum BalloonFlags
    {
        None = 0x00,
        Info = 0x01,
        Warning = 0x02,
        Error = 0x03,
        User = 0x04,
        NoSound = 0x10,
        LargeIcon = 0x20,
        RespectQuietTime = 0x80
    }
}