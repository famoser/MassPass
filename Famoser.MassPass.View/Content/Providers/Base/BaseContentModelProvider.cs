using System;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Content.Providers.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Storage.Cache;
using Famoser.MassPass.View.Content.Interfaces;
using Famoser.MassPass.View.Enums;
using Famoser.MassPass.View.Models;
using Famoser.MassPass.View.Models.Base;
using Newtonsoft.Json;

namespace Famoser.MassPass.View.Content.Providers.Base
{
    public abstract class BaseContentModelProvider<T1, T2> : IViewContentModelProvider
        where T1 : BaseContentModel
        where T2 : ViewContentModel
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
            return JsonConvert.DeserializeObject<T1>(json);
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
        protected abstract T1 ConstructModel(Guid id);

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
        protected abstract T2 ConstructViewModel(BaseContentModel contentModel);

        public void SaveValues(BaseContentModel target, ViewContentModel source)
        {
            target.Name = source.Name;
            target.Description = source.Description;
        }

        public ViewContentModel GetViewContentModel(BaseContentModel model)
        {
            var vm = ConstructViewModel(model);
            vm.Name = model.Name;
            vm.Description = model.Description;
            return vm;
        }

        public abstract PageKeys GetPageKey();
    }
}
