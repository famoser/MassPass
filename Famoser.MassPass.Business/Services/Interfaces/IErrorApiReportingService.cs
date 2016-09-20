using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;

namespace Famoser.MassPass.Business.Services.Interfaces
{
    public interface IErrorApiReportingService
    {
        void ReportUnhandledApiError(ApiResponse response, BaseContentModel model = null);
    }
}
