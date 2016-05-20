using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Entitites
{
    public class RefreshEntity
    {
        public Guid ServerId { get; set; }
        public RemoteStatus RemoteStatus { get; set; }
    }
}
