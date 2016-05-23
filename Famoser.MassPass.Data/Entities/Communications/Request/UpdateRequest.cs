using System;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class UpdateRequest : ApiRequest
    {
        public ContentEntity ContentEntity { get; set; }
        public Guid ServerId { get; set; }
        public Guid RelationId { get; set; }
    }
}
