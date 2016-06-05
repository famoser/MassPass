using System.Collections.Generic;
using System.Collections.ObjectModel;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Models.Storage;

namespace Famoser.MassPass.Business.Helpers
{
    public class CacheHelper
    {
        public static CollectionCacheModel CreateCache(ObservableCollection<ContentModel> contents)
        {
            var res = new CollectionCacheModel()
            {
                ContentModels = new List<ContentModel>(contents),
                SaveVersion = 1
            };
            return res;
        }

        public static IList<ContentModel> ReadCache(CollectionCacheModel cache)
        {
            return cache.ContentModels;
        }  
    }
}
