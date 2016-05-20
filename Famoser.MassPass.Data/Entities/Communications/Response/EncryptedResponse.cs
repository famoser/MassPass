using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class EncryptedResponse
    {
        public string VersionId { get; set; }
        public Uri DownloadUri { get; set; }
    }
}
