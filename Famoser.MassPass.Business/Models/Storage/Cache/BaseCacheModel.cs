using System;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Business.Models.Storage.Cache
{
    public class BaseCacheModel
    {
        public Guid Id { get; set; }
        public ContentApiInformations ContentApiInformations { get; set; }
        public LivecycleStatus LivecycleStatus { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
