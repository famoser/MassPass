using System;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class RawContentEntityRequest : ApiRequest
    {
        public Guid ServerId { get; set; }
        public string VersionId { get; set; }
    }
}
