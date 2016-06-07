using System;
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
