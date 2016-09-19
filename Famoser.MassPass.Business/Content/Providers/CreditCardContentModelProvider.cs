using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Content.Providers.Base;
using Famoser.MassPass.Business.Models.Content;
using Famoser.MassPass.Business.Models.Content.Base;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Content.Providers
{
    public class CreditCardContentModelProvider : BaseContentModelProvider<CreditCardModel>
    {
        public override string GetListName()
        {
            return "credit cards";
        }

        protected override Guid GetTypeGuid()
        {
            return Guid.Parse("b7e6507f-3397-4565-9768-05267f4efced");
        }
    }
}
