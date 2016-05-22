using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;
using Famoser.MassPass.Data.Entities.Decrypted;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class UpdateRequest : ApiRequest
    {
        public ContentEntity ContentEntity { get; set; }
    }
}
