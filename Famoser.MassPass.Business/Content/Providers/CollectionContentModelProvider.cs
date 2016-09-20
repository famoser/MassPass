using System;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Content.Providers.Base;
using Famoser.MassPass.Business.Enums;

namespace Famoser.MassPass.Business.Content.Providers
{
    public class CollectionContentModelProvider : BaseContentModelProvider<CollectionModel>
    {
        public override string GetListName()
        {
            return "collections";
        }

        public override ContentType GetContentType()
        {
            return ContentType.Collection;
        }

        protected override BaseContentModel ConstructModel(Guid id)
        {
            return new CollectionModel(id);
        }

        protected override Guid GetTypeGuid()
        {
            return Guid.Parse("2576024e-1d60-4127-bf0f-f1a83a8af93c");
        }

        public override bool ShowAsList()
        {
            return false;
        }
    }
}
