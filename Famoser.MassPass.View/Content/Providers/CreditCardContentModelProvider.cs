using System;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.View.Content.Providers.Base;
using Famoser.MassPass.View.Models;

namespace Famoser.MassPass.View.Content.Providers
{
    public class CreditCardContentModelProvider : BaseContentModelProvider<CreditCardModel, ViewCreditCardModel>
    {
        public override string GetListName()
        {
            return "credit cards";
        }

        public override ContentType GetContentType()
        {
            return ContentType.CreditCard;
        }

        protected override CreditCardModel ConstructModel(Guid id)
        {
            return new CreditCardModel(id);
        }

        protected override Guid GetTypeGuid()
        {
            return Guid.Parse("b7e6507f-3397-4565-9768-05267f4efced");
        }

        protected override ViewCreditCardModel ConstructViewModel(CreditCardModel contentModel)
        {
            return new ViewCreditCardModel();
        }
    }
}
