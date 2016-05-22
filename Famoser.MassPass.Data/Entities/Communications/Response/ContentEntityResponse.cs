using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Entities.Decrypted;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class ContentEntityResponse : ApiResponse
    {
        public ContentEntity ContentEntity { get; set; }
    }
}
