using System;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.View.Content.Providers.Base;
using Famoser.MassPass.View.Models;

namespace Famoser.MassPass.View.Content.Providers
{
    class LoginContentModelProvider : BaseContentModelProvider<LoginModel, ViewLoginModel>
    {
        public override string GetListName()
        {
            return "logins";
        }

        public override ContentType GetContentType()
        {
            return ContentType.Login;
        }

        protected override LoginModel ConstructModel(Guid id)
        {
            return new LoginModel(id);
        }

        protected override Guid GetTypeGuid()
        {
            return Guid.Parse("20044381-b364-432e-98b8-70b38ca45282");
        }

        protected override ViewLoginModel ConstructViewModel(LoginModel contentModel)
        {
            return new ViewLoginModel();
        }
    }
}
