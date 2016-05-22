using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Base
{
    public class ApiResponse
    {
        public bool Successfull { get; set; }
        public ApiError ApiError { get; set; }

        public bool ResponseReceived { get; set; }
        public bool IsSuccessfull => Successfull && ResponseReceived && ApiError == ApiError.None;
    }
}