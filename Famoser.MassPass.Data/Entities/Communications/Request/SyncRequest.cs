using System;
using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;
using Famoser.MassPass.Data.Entities.Communications.Request.Entities;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class SyncRequest : ApiRequest
    {
        public SyncRequest()
        {
            RefreshEntities = new List<RefreshEntity>();
            CollectionIds = new List<Guid>();
        }

        public List<RefreshEntity> RefreshEntities { get; set; }
        public List<Guid> CollectionIds { get; set; }
    }
}
