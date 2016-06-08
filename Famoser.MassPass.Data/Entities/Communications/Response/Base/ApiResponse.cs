using System;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
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
        public string DebugMessage { get; set; }
        public string RawResponse { get; set; }
        public bool IsSuccessfull => Successfull && !RequestFailed && ApiError == ApiError.None && Exception == null;
        public string GetApiErrorAsString => ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, ApiError>(ApiError).Description;
    }
}