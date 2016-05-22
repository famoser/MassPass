using System;
using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class CollectionEntriesResponse : ApiResponse
    {
        public List<Guid> ServerIds { get; set; }
    }
}
