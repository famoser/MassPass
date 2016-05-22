using System;

namespace Famoser.MassPass.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ApiUriAttribute : Attribute
    {
        public string RelativeUrl { get; }

        public ApiUriAttribute(string relativeUrl)
        {
            RelativeUrl = relativeUrl;
        }
    }
}
