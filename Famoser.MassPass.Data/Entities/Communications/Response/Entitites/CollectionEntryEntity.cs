using System;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Entitites
{
   public class CollectionEntryEntity
    {
        public Guid ContentId { get; set; }
        public Guid CollectionId { get; set; }
        public string VersionId { get; set; }
    }
}
