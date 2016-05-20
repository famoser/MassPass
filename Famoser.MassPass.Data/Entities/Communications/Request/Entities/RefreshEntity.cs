using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Entities.Communications.Request.Entities
{
    public class RefreshEntity
    {
        public Guid ServerId { get; set; }
        public string VersionId { get; set; }
    }
}
