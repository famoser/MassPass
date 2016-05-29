using System;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class DownloadContentEntityResponse : ApiResponse
    {
        public Uri DownloadUri { get; set; }
    }
}
