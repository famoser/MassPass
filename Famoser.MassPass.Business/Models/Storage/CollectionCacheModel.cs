using System.Collections.Generic;

namespace Famoser.MassPass.Business.Models.Storage
{
    public class CollectionCacheModel
    {
        public List<CacheContentEntity> CacheContentModels { get; set; }
        public int SaveVersion { get; set; }
    }
}
