using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Exceptions
{
    public class UploadFailedException : Exception
    {
        public UploadFailedException() : base("Upload failed")
        {
            
        }
    }
}
