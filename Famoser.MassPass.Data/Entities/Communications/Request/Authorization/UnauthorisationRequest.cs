using System;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request.Authorization
{
    public class UnAuthorizationRequest : ApiRequest
    {
        public Guid DeviceToBlockId { get; set; }
        public string Reason { get; set; }
    }
}
