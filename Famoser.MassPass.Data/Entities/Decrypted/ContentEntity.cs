using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Entities.Decrypted
{
    public class ContentEntity : CollectionEntity
    {
        public string ContentJson { get; set; }
        public byte[] ContentFile { get; set; }
    }
}
