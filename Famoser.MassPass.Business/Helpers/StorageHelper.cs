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
            return Convert.ToBase64String(bytes);
        }

        public static byte[] StringToBytes(string content)
        {
            return Convert.FromBase64String(content);
        }
    }
}
