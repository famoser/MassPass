using Famoser.FrameworkEssentials.Helpers;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Enum;
using Famoser.FrameworkEssentials.Attributes;

namespace Famoser.MassPass.Tests.Data
{
    public class ErrorHelper
    {
        public static string ErrorMessageForRequest(ApiResponse resp, string requestName = null)
        {
            string inBetween = requestName != null ? " " + requestName + " " : " ";

            if (resp.IsSuccessfull)
                return "Request" + inBetween + "successfull";

            var res = "Request" + inBetween + "failed!";
            res += "\nRequest successfull: " + !resp.RequestFailed;
            res += "\nApi Successfull: " + resp.Successfull;
            res += "\nApi Error: " + ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, ApiError>(resp.ApiError).Description;
            res += "\nDebug Message: " + resp.ApiMessage;
            res += "\nException: " + resp.Exception?.Message;
            return res;
        }
    }
}
