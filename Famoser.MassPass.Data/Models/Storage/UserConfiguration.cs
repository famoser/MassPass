using System;
using System.Collections.Generic;

namespace Famoser.MassPass.Data.Models.Storage
{
    public class UserConfiguration
    {
        public UserConfiguration()
        {
            ReleationIds = new List<Guid>();
        }
        public Guid UserId { get; set; }
        public Guid DeviceId { get; set; }
        public List<Guid> ReleationIds { get; set; }
    }
}
