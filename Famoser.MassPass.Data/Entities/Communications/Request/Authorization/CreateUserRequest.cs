using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request.Authorization
{
    public class CreateUserRequest : ApiRequest
    {
        public string UserName { get; set; }
        public string DeviceName { get; set; }
    }
}
