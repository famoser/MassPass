using System;
using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Sync;

namespace Famoser.MassPass.Data.Entities.Decrypted
{
    public class CollectionEntity : DecryptedEntity
    {
        public Guid RelationId { get; set; }
        public Guid TypeId { get; set; }
    }
}
