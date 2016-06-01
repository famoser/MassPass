using System;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Entities
{
    public class CollectionEntity
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid TypeId { get; set; }
        public LivecycleStatus LivecycleStatus { get; set; }
        public string Name { get; set; }
    }
}
