using System;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Models
{
    public class EntityServerInformations
    {
        public Guid ServerId { get; set; }
        public Guid ServerRelationId { get; set; }
        public string VersionId { get; set; }
        public RemoteStatus RemoteStatus { get; set; }
    }
}
