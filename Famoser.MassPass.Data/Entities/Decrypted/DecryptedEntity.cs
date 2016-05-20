using System;
using Famoser.MassPass.Data.Entities.Sync;

namespace Famoser.MassPass.Data.Entities.Decrypted
{
    public class DecryptedEntity : SyncEntity
    {
        public Guid ParentId { get; set; }
        public string Name { get; set; }
    }
}
