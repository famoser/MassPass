using System;

namespace Famoser.MassPass.Data.Entities
{
    public class NoteEntity : SyncEntity
    {
        public Guid TypeId { get; set; }
        public string ContentJson { get; set; }
        public byte[] ContentFile { get; set; }
    }
}
