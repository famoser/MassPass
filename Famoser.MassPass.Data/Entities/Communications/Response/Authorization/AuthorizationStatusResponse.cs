using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Authorization
{
    public class AuthorizationStatusResponse : ApiResponse
    {
        public bool IsAuthorized { get; set; }
        public string UnauthorizedReason { get; set; }
    }
}
