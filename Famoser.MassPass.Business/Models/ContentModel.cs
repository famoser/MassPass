using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Business.Models
{
    public class ContentModel : CollectionModel
    {
        public string ContentJson { get; set; }
        public byte[] ContentFile { get; set; }
    }
}
