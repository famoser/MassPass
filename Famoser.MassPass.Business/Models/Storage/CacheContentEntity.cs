using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Business.Models.Storage
{
    public class CacheContentEntity
    {
        public Guid Id { get; set; }
        public Guid TypeId { get; set; }
        public Guid ParentId { get; set; }
        public ApiInformations ApiInformations { get; set; }
        public LocalStatus LocalStatus { get; set; }
        public LivecycleStatus LivecycleStatus { get; set; }
        public string Name { get; set; }
    }
}
