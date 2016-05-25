using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Authorization
{
    public class AuthorizationStatusResponse : ApiResponse
    {
        public bool IsAuthorized { get; set; }
        public string UnauthorizedReason { get; set; }
    }
}
