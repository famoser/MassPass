using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Entities.Communications.Response.Entitites;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class ContentEntityHistoryResponse : ApiResponse
    {
        public List<HistoryEntry> HistoryEntries { get; set; }
    }
}
