using System;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Business.Repositories
{
    public class AuthorizationRepository : BaseRepository, IAuthorizationRepository
    {
        private readonly IErrorApiReportingService _errorApiReportingService;
        private readonly IApiConfigurationService _apiConfigurationService;

        public AuthorizationRepository(IErrorApiReportingService errorApiReportingService, IApiConfigurationService apiConfigurationService)
        {
            _errorApiReportingService = errorApiReportingService;
            _apiConfigurationService = apiConfigurationService;
        }

        public async Task<bool> AuthorizeNew(Guid userId, Guid deviceId, string userName, string deviceName)
        {
            try
            {
                var client = new ApiClient((await _apiConfigurationService.GetApiConfigurationAsync()).Uri, userId, deviceId);
                var resp = await client.AuthorizeAsync(new AuthorizationRequest()
                {
                    AuthorisationCode = null,
                    DeviceName = deviceName,
                    UserName = userName
                });
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
                var client = new ApiClient((await _apiConfigurationService.GetApiConfigurationAsync()).Uri, userId, deviceId);
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
