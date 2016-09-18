using System;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class UpdateResponse : ApiResponse
    {
        public string VersionId { get; set; }
    }
}
