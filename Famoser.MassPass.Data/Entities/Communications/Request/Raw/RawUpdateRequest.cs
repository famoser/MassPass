using System;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class RawUpdateRequest : ApiRequest
    {
        public Guid ServerId { get; set; }
        public Guid RelationId { get; set; }
    }
}
