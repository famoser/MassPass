using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Authorization
{
    public class CreateUserResponse : ApiResponse
    {
        public string Message { get; set; }
    }
}
