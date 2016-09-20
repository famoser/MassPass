using System;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Content.Providers.Base;
using Famoser.MassPass.Business.Enums;

namespace Famoser.MassPass.Business.Content.Providers
{
    class LoginContentModelProvider : BaseContentModelProvider<LoginModel>
    {
        public override string GetListName()
        {
            return "logins";
        }

        public override ContentType GetContentType()
        {
            return ContentType.Login;
        }

        protected override BaseContentModel ConstructModel(Guid id)
        {
            return new CollectionModel(id);
        }

        protected override Guid GetTypeGuid()
        {
            return Guid.Parse("20044381-b364-432e-98b8-70b38ca45282");
        }
    }
}
