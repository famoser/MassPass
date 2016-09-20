using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Storage.Cache;

namespace Famoser.MassPass.Business.Content.Providers.Interfaces
{
    public interface IContentModelProvider
    {
        void WriteValues(BaseContentModel target, BaseContentModel source);
        BaseContentModel Deserialize(string json);
        bool CanDeserialize(string json);
        string Serialize(BaseContentModel model);
        string GetListName();
        bool ShowAsList();
        ContentType GetContentType();
        BaseContentModel FromCache(ContentCacheModel entity);
    }
}
