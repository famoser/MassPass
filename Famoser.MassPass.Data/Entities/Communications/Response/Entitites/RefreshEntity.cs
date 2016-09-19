using System;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Entitites
{
    public class RefreshEntity
    {
        public Guid ContentId { get; set; }
        public Guid CollectionId { get; set; }
        public string VersionId { get; set; }
        public ServerVersion ServerVersion { get; set; }
    }
}
