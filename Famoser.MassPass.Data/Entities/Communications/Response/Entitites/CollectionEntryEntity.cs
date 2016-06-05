using System;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Entitites
{
   public class CollectionEntryEntity
    {
        public Guid ServerId { get; set; }
        public Guid RelationId { get; set; }
        public string VersionId { get; set; }
    }
}
