using System;

namespace Famoser.MassPass.Data.Entities.Communications.Request.Base
{
    public class ApiRequest
    {
        internal Guid DeviceId { get; set; }
        internal Guid UserId { get; set; }
    }
}
