using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Entities.Communications.Response.Entitites;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class SyncResponse : ApiResponse
    {
        public SyncResponse()
        {
            RefreshEntities = new List<RefreshEntity>();
        }

        public List<RefreshEntity> RefreshEntities { get; set; }
    }
}
