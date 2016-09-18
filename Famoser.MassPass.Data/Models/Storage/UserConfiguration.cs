using System;
using System.Collections.Generic;

namespace Famoser.MassPass.Data.Models.Storage
{
    public class UserConfiguration
    {
        public UserConfiguration()
        {
            CollectionIds = new List<Guid>();
        }

        public int Version { get; set; }
        public Guid UserId { get; set; }
        public Guid DeviceId { get; set; }
        public string UserName { get; set; }
        public string DeviceName { get; set; }
        public List<Guid> CollectionIds { get; set; }
    }
}
