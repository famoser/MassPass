using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
