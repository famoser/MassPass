using System;

namespace Famoser.MassPass.Business.Helpers
{
    public static class StorageHelper
    {
        public static string ByteToString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static byte[] StringToBytes(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            byte[] bytes = new byte[content.Length * sizeof(char)];
            Buffer.BlockCopy(content.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
