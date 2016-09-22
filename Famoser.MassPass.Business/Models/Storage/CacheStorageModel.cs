using System.Collections.ObjectModel;
using Famoser.MassPass.Business.Models.Storage.Cache;

namespace Famoser.MassPass.Business.Models.Storage
{
    public class CacheStorageModel
    {
        public ObservableCollection<CollectionCacheModel> CollectionCacheModels { get; set; }
    }
}
