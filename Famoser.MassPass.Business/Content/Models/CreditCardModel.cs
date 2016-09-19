using System;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Content.Base;

namespace Famoser.MassPass.Business.Models.Content
{
    public class CreditCardModel : BaseContentModel
    {
        public CreditCardModel(Guid id) : base(id, ContentType.CreditCard)
        {

        }

        //todo: credit card properties
    }
}
