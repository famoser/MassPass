using System;
using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Sync;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Entities.Decrypted
{
    public class CollectionEntity
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid TypeId { get; set; }
        public ContentStatus ContentStatus { get; set; }
        public string Name { get; set; }
    }
}
