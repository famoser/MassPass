using Windows.UI.Popups;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Presentation.UniversalWindows.Services
{
    #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    class ErrorApiReportingService : IErrorApiReportingService
    {
        public void ReportUnhandledApiError(ApiResponse response, BaseContentModel model = null)
        {
            MessageDialog diag = new MessageDialog(response.GetApiErrorAsString, "Api error occured");
            diag.ShowAsync();
        }
    }
}
