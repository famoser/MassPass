using System;
using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class CollectionEntriesRequest : ApiRequest
    {
        public CollectionEntriesRequest()
        {
            KnownContentIds = new List<Guid>();
        }
        public Guid CollectionId { get; set; }
        public List<Guid> KnownContentIds { get; set; }
    }
}
