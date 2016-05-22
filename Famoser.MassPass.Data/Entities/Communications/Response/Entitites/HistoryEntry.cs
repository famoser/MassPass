using System;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Entitites
{
    public class HistoryEntry
    {
        public Guid DeviceId { get; set; }
        public string VersionId { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
