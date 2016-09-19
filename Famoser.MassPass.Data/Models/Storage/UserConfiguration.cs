using System;
using System.Collections.Generic;

namespace Famoser.MassPass.Data.Models.Storage
{
    public class UserConfiguration
    {
        public Guid UserId { get; set; }
        public Guid DeviceId { get; set; }
        public string UserName { get; set; }
        public string DeviceName { get; set; }
        public string AuthorizationContent { get; set; }
        public string AuthorizationMessage { get; set; }
    }
}
