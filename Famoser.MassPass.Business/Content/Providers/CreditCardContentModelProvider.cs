using System;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Content.Providers.Base;
using Famoser.MassPass.Business.Enums;

namespace Famoser.MassPass.Business.Content.Providers
{
    public class CreditCardContentModelProvider : BaseContentModelProvider<CreditCardModel>
    {
        public override string GetListName()
        {
            return "credit cards";
        }

        public override ContentType GetContentType()
        {
            return ContentType.CreditCard;
        }

        protected override BaseContentModel ConstructModel(Guid id)
        {
            return new CreditCardModel(id);
        }

        protected override Guid GetTypeGuid()
        {
            return Guid.Parse("b7e6507f-3397-4565-9768-05267f4efced");
        }
    }
}
