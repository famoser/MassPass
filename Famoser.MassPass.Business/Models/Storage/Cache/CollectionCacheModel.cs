using System.Collections.Generic;

namespace Famoser.MassPass.Business.Models.Storage.Cache
{
    public class CollectionCacheModel : BaseCacheModel
    {
        public List<ContentCacheModel> CacheContentModels { get; set; }
    }
}
