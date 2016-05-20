using System;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class UpdateResponse : ApiResponse
    {
        public Guid ServerId { get; set; }
        public Guid ServerRelationId { get; set; }
        public string VersionId { get; set; }
    }
}
