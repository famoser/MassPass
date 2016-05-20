using System;
using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Sync;

namespace Famoser.MassPass.Data.Entities.Decrypted
{
    public class CollectionEntity : DecryptedEntity
    {
        public Guid RelationId { get; set; }
    }
}
