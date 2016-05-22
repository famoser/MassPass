using System;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Encrypted
{
    public class EncryptedResponse : ApiResponse
    {
        public Uri DownloadUri { get; set; }
    }
}
