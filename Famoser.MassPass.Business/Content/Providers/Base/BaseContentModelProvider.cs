using System;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Content.Providers.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Storage.Cache;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Content.Providers.Base
{
    public abstract class BaseContentModelProvider<T> : IContentModelProvider where T : BaseContentModel
    {
        public virtual void WriteValues(BaseContentModel target, BaseContentModel source)
        {
            target.Description = source.Description;
            target.Name = source.Name;
            target.ContentApiInformations = source.ContentApiInformations;
            target.ContentLoadingState = source.ContentLoadingState;
            target.LivecycleStatus = source.LivecycleStatus;
        }

        public BaseContentModel Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public bool CanDeserialize(string json)
        {
            return json.Contains(GetTypeGuid().ToString());
        }

        public virtual string Serialize(BaseContentModel model)
        {
            return JsonConvert.SerializeObject(model);
        }

        public abstract string GetListName();
        public virtual bool ShowAsList()
        {
            return true;
        }

        public abstract ContentType GetContentType();
        protected abstract BaseContentModel ConstructModel(Guid id);

        public BaseContentModel FromCache(ContentCacheModel entity)
        {
            var model = ConstructModel(entity.Id);
            model.ContentApiInformations = entity.ContentApiInformations;
            model.ContentLoadingState = LoadingState.NotLoaded;
            model.Description = entity.Description;
            model.Name = entity.Name;
            model.LivecycleStatus = entity.LivecycleStatus;
            return model;
        }

        protected abstract Guid GetTypeGuid();
    }
}
