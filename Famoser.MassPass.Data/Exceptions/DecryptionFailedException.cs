using System;

namespace Famoser.MassPass.Data.Exceptions
{
    public class DecryptionFailedException : Exception
    {
        public DecryptionFailedException() : base("Decryption failed!")
        {
            
        }
    }
}
