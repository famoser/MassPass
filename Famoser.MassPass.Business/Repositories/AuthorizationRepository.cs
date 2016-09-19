using System.Threading.Tasks;
using Famoser.MassPass.Business.Models.Storage;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Models.Configuration;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Repositories
{
    public class AuthorizationRepository : BaseRepository, IAuthorizationRepository
    {
        private readonly IApiConfigurationService _apiConfigurationService;

        public AuthorizationRepository(IApiConfigurationService apiConfigurationService)
        {
            _apiConfigurationService = apiConfigurationService;
        }

        public Task<ApiClient> CreateUserAsync(UserConfiguration configuration)
        {
            return ExecuteSafe(async () =>
            {
                var apiConfig = await _apiConfigurationService.GetApiConfigurationAsync();
                var client = new ApiClient(apiConfig.Uri, configuration.UserId, configuration.DeviceId);
                var res = await client.CreateUserAsync(new CreateUserRequest()
                {
                    DeviceName = configuration.DeviceName,
                    UserName = configuration.UserName
                });
                if (res.IsSuccessfull)
                {
                    configuration.AuthorizationMessage = res.Message;
                    return client;
                }
                return null;
            });
        }

        public Task<bool> CreateAuthorizationAsync(ApiClient client, string authCode, string content)
        {
            return ExecuteSafe(async () =>
            {
                var res = await client.CreateAuthorizationAsync(new CreateAuthorizationRequest()
                {
                    AuthorisationCode = authCode,
                    Content = content
                });
                return res.IsSuccessfull;
            });
        }

        public Task<ApiClient> AuthorizeAsync(UserConfiguration configuration, string authCode)
        {
            return ExecuteSafe(async () =>
            {
                var apiConfig = await _apiConfigurationService.GetApiConfigurationAsync();
                var client = new ApiClient(apiConfig.Uri, configuration.UserId, configuration.DeviceId);
                var resp = await client.AuthorizeAsync(new AuthorizationRequest()
                {
                    DeviceName = configuration.DeviceName,
                    AuthorisationCode = authCode
                });
                if (resp.IsSuccessfull)
                {
                    configuration.AuthorizationContent = JsonConvert.DeserializeObject<UserAuthorizationContent>(resp.Content);
                    configuration.AuthorizationMessage = resp.Message;
                    return client;
                }
                return null;
            });
        }

        private ApiClient _authorizedClient;
        public Task<ApiClient> GetAuthorizedApiClientAsync()
        {
            return ExecuteSafe(async () =>
            {
                if (_authorizedClient == null)
                {
                    var apiConfig = await _apiConfigurationService.GetApiConfigurationAsync();
                    var userConfig = await _apiConfigurationService.GetUserConfigurationAsync();
                    var client = new ApiClient(apiConfig.Uri, userConfig.UserId, userConfig.DeviceId);
                    var resp = await client.CheckIsAuthorizedAsync();
                    if (resp)
                        _authorizedClient = client;
                }
                return _authorizedClient;
            });
        }
    }
}
