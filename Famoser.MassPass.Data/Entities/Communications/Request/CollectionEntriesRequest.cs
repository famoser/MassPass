using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class CollectionEntriesRequest : ApiRequest
    {
        public Guid RelationId { get; set; }
        public List<Guid> KnownServerIds { get; set; }
    }
}
