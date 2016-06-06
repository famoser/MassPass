using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Entities.Communications.Response.Entitites;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Authorization
{
    public class AuthorizedDevicesResponse : ApiResponse
    {
        public AuthorizedDevicesResponse()
        {
            AuthorizedDeviceEntities = new List<AuthorizedDeviceEntity>();
        }

        public List<AuthorizedDeviceEntity> AuthorizedDeviceEntities { get; set; }
    }
}
