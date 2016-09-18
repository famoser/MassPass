using System;

namespace Famoser.MassPass.Data.Entities.Communications.Request.Entities
{
    public class RefreshEntity
    {
        public Guid ContentId { get; set; }
        public string VersionId { get; set; }
    }
}
