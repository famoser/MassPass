using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class UnAuthorizationRequest : ApiRequest
    {
        public Guid DeviceToBlockId { get; set; }
        public string Reason { get; set; }
    }
}
