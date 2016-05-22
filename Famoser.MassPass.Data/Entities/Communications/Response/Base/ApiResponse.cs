using System;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Base
{
    public class ApiResponse
    {
        public ApiResponse()
        {
            ApiError = ApiError.None;
        }

        public bool Successfull { get; set; }
        public ApiError ApiError { get; set; }

        public bool RequestFailed { get; set; }
        public Exception Exception { get; set; }
        public bool IsSuccessfull => Successfull && !RequestFailed && ApiError == ApiError.None;
    }
}