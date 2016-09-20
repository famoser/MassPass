using System;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models.Base;

namespace Famoser.MassPass.Business.Content.Models
{
    public class CreditCardModel : BaseContentModel
    {
        public CreditCardModel(Guid id) : base(id, ContentType.CreditCard)
        {

        }

        //todo: credit card properties
    }
}
