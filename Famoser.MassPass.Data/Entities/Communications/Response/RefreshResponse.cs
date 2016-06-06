using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Entities.Communications.Response.Entitites;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class RefreshResponse : ApiResponse
    {
        public RefreshResponse()
        {
            RefreshEntities = new List<RefreshEntity>();
        }

        public List<RefreshEntity> RefreshEntities { get; set; }
    }
}
