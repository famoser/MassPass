using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                CacheContentModels = contents.Select(Convert).ToList(),
                SaveVersion = 1
            };
            return res;
        }

        public static IList<ContentModel> ReadCache(CollectionCacheModel cache)
        {
            return cache.CacheContentModels.Select(Convert).ToList();
        }


        private static ContentModel Convert(CacheContentEntity entity)
        {
            return new ContentModel()
            {
                Id = entity.Id,
                ParentId = entity.ParentId,
                TypeId = entity.TypeId,
                LivecycleStatus = entity.LivecycleStatus,
                LocalStatus = entity.LocalStatus,
                ApiInformations = entity.ContentApiInformations,
                Name = entity.Name
            };
        }

        private static CacheContentEntity Convert(ContentModel entity)
        {
            return new CacheContentEntity()
            {
                Id = entity.Id,
                ParentId = entity.ParentId,
                TypeId = entity.TypeId,
                LivecycleStatus = entity.LivecycleStatus,
                LocalStatus = entity.LocalStatus,
                ContentApiInformations = entity.ApiInformations,
                Name = entity.Name
            };
        }

        public static void WriteAllValues(ContentModel target, ContentModel source)
        {
            target.ContentFile = source.ContentFile;
            target.ContentJson = source.ContentJson;
            target.Id = source.Id;
            target.ParentId = source.ParentId;
            target.TypeId = source.TypeId;
            target.LivecycleStatus = source.LivecycleStatus;
            target.LocalStatus = source.LocalStatus;
            target.ApiInformations = source.ApiInformations;
            target.Name = source.Name;
        }
    }
}
