using System.Collections.Generic;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Entities.Communications.Response.Entitites;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Authorization
{
    public class AuthorizedDevicesResponse : ApiResponse
    {
        public List<AuthorizedDeviceEntity> AuthorizedDeviceEntities { get; set; }
    }
}
