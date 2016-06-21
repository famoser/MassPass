using System;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Presentation.UniversalWindows.Services.Mock
{
    public class MockErrorApiReportingService : IErrorApiReportingService
    {
        public void ReportUnhandledApiError(ApiResponse response, ContentModel model = null)
        {
            throw new NotImplementedException();
        }
    }
}
