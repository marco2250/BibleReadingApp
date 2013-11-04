using System.IO;

namespace BibleReading.Common45.Root
{
    public static class ByteExtension
    {
        public static MemoryStream ToMemoryStream(this byte[] bytes)
        {
            var m = new MemoryStream();
            m.Write(bytes, 0, bytes.Length);
            m.Position = 0;

            return m;
        }
    }
}
