using System;

namespace Famoser.MassPass.Data.Models.Configuration
{
    public class UserConfiguration
    {
        public Guid UserId { get; set; }
        public Guid DeviceId { get; set; }
        public string UserName { get; set; }
        public string DeviceName { get; set; }
        public UserAuthorizationContent AuthorizationContent { get; set; }
        public string AuthorizationMessage { get; set; }
    }
}
