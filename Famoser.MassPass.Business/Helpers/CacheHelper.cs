using System.Linq;
using Famoser.MassPass.Business.Content.Helpers;
using Famoser.MassPass.Business.Content.Models;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Storage.Cache;

namespace Famoser.MassPass.Business.Helpers
{
    public class CacheHelper
    {
        public static CollectionCacheModel CreateCache(CollectionModel collection)
        {
            var res = new CollectionCacheModel()
            {
                CacheContentModels = collection.ContentModels.Select(Convert).ToList()
            };
            return res;
        }

        public static CollectionModel ReadCache(CollectionCacheModel cache)
        {
            var model = new CollectionModel(cache.Id)
            {
                ContentApiInformations = cache.ContentApiInformations,
                Description = cache.Description,
                LivecycleStatus = cache.LivecycleStatus,
                Name = cache.Name
            };
            foreach (var cacheContentEntity in cache.CacheContentModels)
            {
                var converted = Convert(cacheContentEntity);
                if (converted != null)
                    model.ContentModels.Add(converted);
            }
            return model;
        }


        private static BaseContentModel Convert(ContentCacheModel entity)
        {
            var provider = ContentHelper.GetProvider(entity.ContentType);
            return provider?.FromCache(entity);
        }

        private static ContentCacheModel Convert(BaseContentModel entity)
        {
            return new ContentCacheModel()
            {
                Id = entity.Id,
                LivecycleStatus = entity.LivecycleStatus,
                ContentApiInformations = entity.ContentApiInformations,
                Name = entity.Name,
                ContentType = entity.ContentType,
                Description = entity.Description
            };
        }
    }
}
