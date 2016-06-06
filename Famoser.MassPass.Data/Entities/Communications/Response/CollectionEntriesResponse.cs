using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Entities.Communications.Response.Entitites;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class CollectionEntriesResponse : ApiResponse
    {
        public CollectionEntriesResponse()
        {
            CollectionEntryEntities = new List<CollectionEntryEntity>();
        }

        public List<CollectionEntryEntity> CollectionEntryEntities { get; set; }
    }
}
