using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Content.Providers.Base;
using Famoser.MassPass.Business.Content.Providers.Interfaces;
using Famoser.MassPass.Business.Models.Content;
using Famoser.MassPass.Business.Models.Content.Base;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Content.Providers
{
    public class CollectionContentModelProvider : BaseContentModelProvider<CollectionModel>
    {
        public override string GetListName()
        {
            return "collections";
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
