using System;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Models
{
    public class ServerInformations
    {
        public Guid ServerId { get; set; }
        public Guid ServerRelationId { get; set; }
        public Guid PasswordId { get; set; }
        public string VersionId { get; set; }
        public LocalStatus LocalStatus { get; set; }
    }
}
