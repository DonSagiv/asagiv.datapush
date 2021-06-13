using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace asagiv.datapush.common.Utilities
{
    public static class ByteArrayUtils
    {
        public static byte[] ToByteArray(this object inputObject)
        {
            var bf = new BinaryFormatter();

            using(var ms = new MemoryStream())
            {
                bf.Serialize(ms, inputObject);
                return ms.ToArray();
            }
        }
    }
}
