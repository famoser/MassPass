using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Presentation.UniversalWindows.Services
{
    #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    class ErrorApiReportingService : IErrorApiReportingService
    {
        public void ReportUnhandledApiError(ApiResponse response, ContentModel model = null)
        {
            MessageDialog diag = new MessageDialog(response.GetApiErrorAsString, "Api error occured");
            diag.ShowAsync();
        }
    }
}
