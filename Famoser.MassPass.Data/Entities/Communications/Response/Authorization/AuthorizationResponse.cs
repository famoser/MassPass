using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class AuthorizationResponse : ApiResponse
    {
        public string Message { get; set; }
    }
}
