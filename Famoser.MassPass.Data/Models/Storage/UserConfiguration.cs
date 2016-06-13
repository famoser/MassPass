using System;
using System.Collections.Generic;

namespace Famoser.MassPass.Data.Models.Storage
{
    public class UserConfiguration
    {
        public UserConfiguration()
        {
            RelationIds = new List<Guid>();
        }

        public int Version { get; set; }
        public Guid UserId { get; set; }
        public Guid DeviceId { get; set; }
        public List<Guid> RelationIds { get; set; }
    }
}
