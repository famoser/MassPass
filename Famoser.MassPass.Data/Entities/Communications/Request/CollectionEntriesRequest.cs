using System;
using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class CollectionEntriesRequest : ApiRequest
    {
        public Guid RelationId { get; set; }
        public List<Guid> KnownServerIds { get; set; }
    }
}
