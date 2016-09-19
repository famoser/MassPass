using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Authorization
{
    public class AuthorizationResponse : ApiResponse
    {
        public string Message { get; set; }
        public string Content { get; set; }
    }
}
