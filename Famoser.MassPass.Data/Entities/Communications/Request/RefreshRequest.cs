using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;
using Famoser.MassPass.Data.Entities.Communications.Request.Entities;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class RefreshRequest : ApiRequest
    {
        public RefreshRequest()
        {
            RefreshEntities = new List<RefreshEntity>();
        }

        public List<RefreshEntity> RefreshEntities { get; set; }
    }
}
