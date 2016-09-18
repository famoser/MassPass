using System;
using System.Reflection;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ApiUriAttribute : Attribute
    {
        public string RelativeUrl { get; }

        public ApiUriAttribute(string relativeUrl, ApiNode dependsOn = ApiNode.Index, ApiType apiType = ApiType.Version1)
        {
            if (dependsOn != ApiNode.Index)
            {
                var type = typeof(ApiNode);
                var attribute = type.GetRuntimeField(dependsOn.ToString()).GetCustomAttribute<ApiUriAttribute>();
                RelativeUrl = "/" + attribute.RelativeUrl;
            }
            else
            {
                if (apiType == ApiType.Version1)
                    RelativeUrl = "1.0";
                else
                    throw new Exception("Unknown API version");
            }

            RelativeUrl += "/" + relativeUrl;
        }
    }
}
