using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Business.Helpers
{
    public static class StorageHelper
    {
        public static string ByteToString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            return Convert.ToBase64String(bytes);
        }

        public static byte[] StringToBytes(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            return Convert.FromBase64String(content);
        }
    }
}
