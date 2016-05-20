using System;

namespace Famoser.MassPass.Data.Entities.Communications.Request.Base
{
    public class ApiRequest
    {
        public Guid DeviceId { get; set; }
        public Guid UserId { get; set; }
    }
}
