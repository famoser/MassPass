using System;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Entitites
{
    public class AuthorizedDeviceEntity
    {
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; }
        public DateTime LastRequestDateTime { get; set; }
        public DateTime LastModificationDateTime { get; set; }
        public DateTime AuthorizationDateTime { get; set; }
    }
}
