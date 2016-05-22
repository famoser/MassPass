using System;

namespace Famoser.MassPass.Data.Exceptions
{
    public class UploadFailedException : Exception
    {
        public UploadFailedException() : base("Upload failed")
        {
            
        }
    }
}
