using System;
using Famoser.MassPass.Data.Enum;
using PInvoke;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Entitites
{
    public class RefreshEntity
    {
        public Guid ServerId { get; set; }
        public string VersionId { get; set; }
        public RemoteStatus RemoteStatus { get; set; }
    }
}
