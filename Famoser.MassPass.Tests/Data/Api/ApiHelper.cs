using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Singleton;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Services;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.Tests.Data.Mocks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Famoser.MassPass.Tests.Data.Api
{
    public class ApiHelper : SingletonBase<ApiHelper>, IDisposable
    {
        private static IApiConfigurationService _apiConfigurationService;
        private static readonly Tuple<Guid, Guid> UserDevice1Tuple = new Tuple<Guid, Guid>(Guid.Parse("d0e47801-2571-4790-a7a3-5c9f6ceba9e3"), Guid.Parse("99311fab-cb31-4f95-83f3-1f4fb965928b"));
        private static readonly Tuple<Guid, Guid> UserDevice2Tuple = new Tuple<Guid, Guid>(Guid.Parse("d0e47801-2571-4790-a7a3-5c9f6ceba9e3"), Guid.Parse("993111ab-cb31-4395-83f3-1f4fb965928b"));

        public ApiHelper()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IEncryptionService, EncryptionService>();
            SimpleIoc.Default.Register<IApiEncryptionService, ApiEncryptionService>();
            SimpleIoc.Default.Register<IPasswordVaultService, PasswordVaultServiceMock>();
            SimpleIoc.Default.Register<IApiConfigurationService, ApiConfigurationServiceMock>();
            _apiConfigurationService = SimpleIoc.Default.GetInstance<IApiConfigurationService>();
        }

        public async Task<ApiClient> CreateUnAuthorizedClient()
        {
            return new ApiClient((await _apiConfigurationService.GetApiConfigurationAsync()).Uri, Guid.NewGuid(), Guid.NewGuid());
        }

        /// <summary>
        /// validate a new user to the API
        /// </summary>
        /// <returns>Tuple with Item1 = userGuid and Item2 = deviceGuid</returns>
        public async Task<ApiClient> CreateAuthorizedClient1Async(string userName = "my user", string deviceName = "my device")
        {
            var client = new ApiClient((await _apiConfigurationService.GetApiConfigurationAsync()).Uri, UserDevice1Tuple.Item1, UserDevice1Tuple.Item2);

            var status = await client.GetAuthorizationStatusAsync(new AuthorizationStatusRequest());
            AssertionHelper.CheckForSuccessfull(status);
            if (!status.IsAuthorized)
            {
                //authorize
                var respo = await client.CreateUserAsync(new CreateUserRequest()
                {
                    UserName = userName,
                    DeviceName = deviceName
                });
                AssertionHelper.CheckForSuccessfull(respo);
            }

            return client;
        }

        /// <summary>
        /// validate a new user to the API
        /// </summary>
        /// <returns>Tuple with Item1 = userGuid and Item2 = deviceGuid</returns>
        public async Task<ApiClient> AddValidatedDevice2Async(ApiClient client, string deviceName = "new device")
        {
            var authCode = Guid.NewGuid().ToString();
            var resp1 = await client.CreateAuthorizationAsync(new CreateAuthorizationRequest()
            {
                AuthorisationCode = authCode,
                Content = "testcontent"
            });
            AssertionHelper.CheckForSuccessfull(resp1);
            var newCLient = new ApiClient(client.BaseUri, UserDevice2Tuple.Item1, UserDevice2Tuple.Item2);
            var resp2 = await newCLient.AuthorizeAsync(new AuthorizationRequest
            {
                DeviceName = deviceName,
                AuthorisationCode = authCode
            });
            AssertionHelper.CheckForSuccessfull(resp2);
            return newCLient;
        }

        private async Task CleanUpApiAsync()
        {
            var client = new ApiClient((await _apiConfigurationService.GetApiConfigurationAsync()).Uri, UserDevice1Tuple.Item1, UserDevice1Tuple.Item2);
            await client.WipeUserAsync(new WipeUserRequest());
        }

        public void Dispose()
        {
            CleanUpApiAsync().Wait();
        }
    }
}
