using System;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Models;
using Famoser.MassPass.Data.Models.Storage;

namespace Famoser.MassPass.Data.Entities
{
    public class TransferEntity
    {
        public ContentApiInformations ContentApiInformations { get; set; }
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string ContentJson { get; set; }
        public byte[] ContentFile { get; set; }
    }
}
