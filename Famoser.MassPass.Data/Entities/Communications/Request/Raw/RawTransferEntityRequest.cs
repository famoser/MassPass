using System;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request.Raw
{
    public class RawTransferEntityRequest : ApiRequest
    {
        public Guid ContentId { get; set; }
        public string VersionId { get; set; }
    }
}
