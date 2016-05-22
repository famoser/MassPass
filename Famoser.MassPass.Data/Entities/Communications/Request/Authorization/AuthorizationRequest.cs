using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class AuthorizationRequest : ApiRequest
    {
        public string AuthorisationCode { get; set; }

        public string UserName { get; set; }
        public string DeviceName { get; set; }
    }
}
