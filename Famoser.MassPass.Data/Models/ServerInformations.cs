using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Models
{
    public class ServerInformations
    {
        public Guid ServerId { get; set; }
        public Guid ServerRelationId { get; set; }
        public DateTime ServerTimeStamp { get; set; }
    }
}
