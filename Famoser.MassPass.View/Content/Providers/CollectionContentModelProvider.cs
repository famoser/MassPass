using System;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.View.Content.Providers.Base;
using Famoser.MassPass.View.Enums;
using Famoser.MassPass.View.Models;

namespace Famoser.MassPass.View.Content.Providers
{
    public class CollectionContentModelProvider : BaseContentModelProvider<CollectionModel, ViewCollectionModel>
    {
        public override string GetListName()
        {
            return "collections";
        }

        public override ContentType GetContentType()
        {
            return ContentType.Collection;
        }

        protected override CollectionModel ConstructModel(Guid id)
        {
            return new CollectionModel(id);
        }

        protected override Guid GetTypeGuid()
        {
            return Guid.Parse("2576024e-1d60-4127-bf0f-f1a83a8af93c");
        }

        protected override ViewCollectionModel ConstructViewModel(BaseContentModel contentModel)
        {
            return new ViewCollectionModel();
        }

        public override PageKeys GetPageKey()
        {
            return PageKeys.ListContentPage;
        }

        public override bool ShowAsList()
        {
            return false;
        }
    }
}
