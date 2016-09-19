using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Content.Providers.Base;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Content;

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

        protected override Guid GetTypeGuid()
        {
            return Guid.Parse("20044381-b364-432e-98b8-70b38ca45282");
        }
    }
}
