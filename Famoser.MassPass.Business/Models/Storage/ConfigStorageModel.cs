using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Models.Configuration;

namespace Famoser.MassPass.Business.Models.Storage
{
    public class ConfigStorageModel
    {
        public ApiConfiguration ApiConfiguration { get; set; }
        public UserConfiguration UserConfiguration { get; set; }
    }
}
