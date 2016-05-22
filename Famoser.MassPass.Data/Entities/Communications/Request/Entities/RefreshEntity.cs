using System;

namespace Famoser.MassPass.Data.Entities.Communications.Request.Entities
{
    public class RefreshEntity
    {
        public Guid ServerId { get; set; }
        public string VersionId { get; set; }
    }
}
