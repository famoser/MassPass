using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Authorization
{
    public class UnAuthorizationResponse : ApiResponse
    {
        public string Message { get; set; }
    }
}
