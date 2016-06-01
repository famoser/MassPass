using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Services;
using Famoser.MassPass.Data.Models.Storage;

namespace Famoser.MassPass.Business.Models.Storage
{
    public class ApiStorageModel
    {
        public ApiConfiguration ApiConfiguration { get; set; }
        public UserConfiguration UserConfiguration { get; set; }
    }
}
