using System.Collections.ObjectModel;
using Famoser.MassPass.Business.Models.Storage.Cache;
using Famoser.MassPass.Data.Models.Configuration;

namespace Famoser.MassPass.Business.Models.Storage
{
    public class CacheStorageModel
    {
        public ApiConfiguration ApiConfiguration { get; set; }
        public UserConfiguration UserConfiguration { get; set; }
        public ObservableCollection<CollectionCacheModel> CollectionCacheModels { get; set; }
    }
}
