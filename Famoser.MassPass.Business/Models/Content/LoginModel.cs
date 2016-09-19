using System;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Content.Base;

namespace Famoser.MassPass.Business.Models.Content
{
    public class LoginModel : BaseContentModel
    {
        public LoginModel(Guid id) : base(id, ContentType.Login)
        {
        }

        //login properties
    }
}
