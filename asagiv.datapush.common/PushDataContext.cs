using System;

namespace asagiv.datapush.common
{
    public class PushDataContext
    {
        public string topic { get; set; }
        public byte[] data { get; set; }
    }
}
