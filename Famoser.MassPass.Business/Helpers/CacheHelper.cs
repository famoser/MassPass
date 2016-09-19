using System.Linq;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Content;
using Famoser.MassPass.Business.Models.Content.Base;
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
            BaseContentModel model;
            switch (entity.ContentType)
            {
                case ContentType.Note:
                    model = new NoteModel(entity.Id);
                    break;

                case ContentType.CreditCard:
                    model = new CreditCardModel(entity.Id);
                    break;

                case ContentType.Login:
                    model = new LoginModel(entity.Id);
                    break;
                default:
                    return null;
            }
            model.ContentApiInformations = entity.ContentApiInformations;
            model.ContentLoadingState = LoadingState.NotLoaded;
            model.Description = entity.Description;
            model.Name = entity.Name;
            model.LivecycleStatus = entity.LivecycleStatus;
            model.LocalStatus = entity.LocalStatus;
            return model;
        }

        private static ContentCacheModel Convert(BaseContentModel entity)
        {
            return new ContentCacheModel()
            {
                Id = entity.Id,
                LivecycleStatus = entity.LivecycleStatus,
                LocalStatus = entity.LocalStatus,
                ContentApiInformations = entity.ContentApiInformations,
                Name = entity.Name,
                ContentType = entity.ContentType,
                Description = entity.Description
            };
        }
    }
}
