using System;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Models.RestService;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Base
{
    public class ApiResponse
    {
        public ApiResponse()
        {
            ApiError = ApiError.None;
        }

        public ApiError ApiError { get; set; }
        public string ApiMessage { get; set; }
        public HttpResponseModel RawResponse { get; set; }
        public bool IsSuccessfull => RawResponse != null && RawResponse.IsRequestSuccessfull && ApiError == ApiError.None;
        public string GetApiErrorAsString => ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, ApiError>(ApiError).Description;
    }
}