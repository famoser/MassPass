using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models.Base;

namespace Famoser.MassPass.Business.Models
{
    public class DeviceModel : BaseModel
    {
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; }
        public DateTime LastRequestDateTime { get; set; }
        public DateTime LastModificationDateTime { get; set; }
        public DateTime AuthorizationDateTime { get; set; }
    }
}
