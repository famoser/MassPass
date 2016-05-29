using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.MassPass.Tests.Data.Api
{
    public class AssertionHelper
    {
        public static void CheckForUnSuccessfull(ApiResponse resp, string requestIdentifier, ApiError error)
        {
            Assert.IsFalse(resp.IsSuccessfull, ErrorHelper.ErrorMessageForRequest(resp, requestIdentifier));
            Assert.IsFalse(resp.RequestFailed, ErrorHelper.ErrorMessageForRequest(resp, requestIdentifier));
            Assert.IsFalse(resp.Successfull, ErrorHelper.ErrorMessageForRequest(resp, requestIdentifier));
            Assert.IsTrue(resp.ApiError == ApiError.NotAuthorized, ErrorHelper.ErrorMessageForRequest(resp, requestIdentifier));
            Assert.IsNull(resp.Exception, ErrorHelper.ErrorMessageForRequest(resp, requestIdentifier));
        }

        public static void CheckForSuccessfull(ApiResponse resp, string requestIdentifier)
        {
            Assert.IsTrue(resp.IsSuccessfull, ErrorHelper.ErrorMessageForRequest(resp, requestIdentifier));
        }

        public static void CheckForEquality<T>(T one, T two)
        {
            // Just grabbing this to get hold of the type name:
            var type = one.GetType();

            // Get the PropertyInfo object:
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                Assert.IsTrue(property.GetValue(one) == property.GetValue(two), "value named " + property.Name + " is not equal");
            }
        }

        public static void CheckDateTimeNowValidity(DateTime date, string propertyName)
        {
            Assert.IsTrue(date < DateTime.Now + TimeSpan.FromSeconds(10) && date > DateTime.Now - TimeSpan.FromSeconds(20), propertyName + " wrong");
        }
    }
}
