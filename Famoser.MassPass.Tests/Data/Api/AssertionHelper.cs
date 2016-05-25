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
    }
}
