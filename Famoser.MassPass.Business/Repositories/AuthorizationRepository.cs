using System;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Business.Repositories
{
    public class AuthorizationRepository : BaseRepository, IAuthorizationRepository
    {
        private readonly IDataService _dataService;
        private readonly IErrorApiReportingService _errorApiReportingService;
        private readonly IApiConfigurationService _apiConfigurationService;

        public AuthorizationRepository(IDataService dataService, IErrorApiReportingService errorApiReportingService, IApiConfigurationService apiConfigurationService) : base(apiConfigurationService)
        {
            _dataService = dataService;
            _errorApiReportingService = errorApiReportingService;
            _apiConfigurationService = apiConfigurationService;
        }

        public async Task<bool> AuthorizeNew(Guid userId, Guid deviceId, string userName, string deviceName)
        {
            try
            {
                var requHelper = await GetRequestHelper();
                var resp = await _dataService.AuthorizeAsync(requHelper.AuthorizationRequest(userName, deviceName, null));
                if (!resp.IsSuccessfull)
                {
                    _errorApiReportingService.ReportUnhandledApiError(resp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
                return false;
            }
            return true;
        }

        public async Task<bool> AuthorizeAdditional(Guid userId, Guid deviceId, string authCode, string userName, string deviceName)
        {
            try
            {
                var requHelper = await GetRequestHelper();
                var resp = await _dataService.AuthorizeAsync(requHelper.AuthorizationRequest(userName, deviceName, authCode));
                if (!resp.IsSuccessfull)
                {
                    _errorApiReportingService.ReportUnhandledApiError(resp);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
                return false;
            }
            return true;
        }
    }
}
