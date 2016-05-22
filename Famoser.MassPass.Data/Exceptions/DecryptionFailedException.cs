using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Exceptions
{
    public class DecryptionFailedException : Exception
    {
        public DecryptionFailedException() : base("Decryption failed!")
        {
            
        }
    }
}
