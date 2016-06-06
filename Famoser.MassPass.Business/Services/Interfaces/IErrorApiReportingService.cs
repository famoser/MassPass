using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Business.Services.Interfaces
{
    public interface IErrorApiReportingService
    {
        void ReportUnhandledApiError(ApiResponse response, ContentModel model = null);
    }
}
