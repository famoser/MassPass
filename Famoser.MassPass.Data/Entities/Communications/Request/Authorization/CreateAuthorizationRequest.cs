using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request.Authorization
{
    public class CreateAuthorizationRequest : ApiRequest
    {
        public string AuthorisationCode { get; set; }
        public string Content { get; set; }
    }
}
