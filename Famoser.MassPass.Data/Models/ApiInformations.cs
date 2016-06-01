using System;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Models
{
    public class ApiInformations
    {
        public Guid ServerId { get; set; }
        public Guid ServerRelationId { get; set; }
        public string VersionId { get; set; }
        public ApiStatus ApiStatus { get; set; }
    }
}
