using System;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models.Base;

namespace Famoser.MassPass.Business.Content.Models
{
    public class LoginModel : BaseContentModel
    {
        public LoginModel(Guid id) : base(id, ContentType.Login)
        {
        }

        //login properties
    }
}
