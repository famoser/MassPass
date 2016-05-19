using System;

namespace Famoser.MassPass.Data.Entities
{
    public class NoteEntity
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public Guid TypeId { get; set; }
        public string Name { get; set; }
        public string ContentJson { get; set; }
    }
}
