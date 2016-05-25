using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Services.Base;
using Famoser.FrameworkEssentials.Singleton;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Services;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.Tests.Data.Mocks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Famoser.MassPass.Tests.Data.Api
{
    public class ApiHelper : SingletonBase<ApiHelper>, IDisposable
    {
        private static IDataService _dataService;
        private static IConfigurationService _configurationService;

        public ApiHelper()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IEncryptionService, EncryptionService>();
            SimpleIoc.Default.Register<IDataService, DataService>();
            SimpleIoc.Default.Register<IApiEncryptionService, ApiEncryptionService>();
            SimpleIoc.Default.Register<IPasswordService, PasswordServiceMock>();
            SimpleIoc.Default.Register<IConfigurationService, ConfigurationServiceMock>();
            _dataService = SimpleIoc.Default.GetInstance<IDataService>();
            _configurationService = SimpleIoc.Default.GetInstance<IConfigurationService>();
        }

        public IDataService GetDataService()
        {
            return _dataService;
        }

        public static async Task CleanUpApi()
        {
            var config = await _configurationService.GetApiConfiguration();
            var newUri = new Uri(config.Uri.AbsolutePath + "/cleanup");
            var service = new HttpService();
            service.FireAndForget(newUri);
        }

        /// <summary>
        /// validate a new user to the API
        /// </summary>
        /// <returns>Tuple with Item1 = userGuid and Item2 = deviceGuid</returns>
        public async Task<Tuple<Guid, Guid>> CreateValidatedDevice()
        {
            var userGuid = Guid.NewGuid();
            var deviceGuid = Guid.NewGuid();
            var authRequest = new AuthorizationRequest
            {
                DeviceId = deviceGuid,
                UserName = "my user",
                DeviceName = "my device",
                UserId = userGuid
            };
            var res = await GetDataService().Authorize(authRequest);
            AssertionHelper.CheckForSuccessfull(res, "auth request in CreateValidatesDevice");
            return new Tuple<Guid, Guid>(userGuid, deviceGuid);
        }

        /// <summary>
        /// validate a new user to the API
        /// </summary>
        /// <returns>Tuple with Item1 = userGuid and Item2 = deviceGuid</returns>
        public async Task<Guid> AddValidatedDevice(Guid userId, Guid deviceId, string deviceName = "new device")
        {
            var userGuid = userId;
            var newDeviceId = Guid.NewGuid();
            var authCode = Guid.NewGuid().ToString();
            var createAuthRequest = new CreateAuthorizationRequest()
            {
                DeviceId = deviceId,
                AuthorisationCode = authCode,
                UserId = userGuid
            };
            var authRequest = new AuthorizationRequest
            {
                DeviceId = newDeviceId,
                UserName = "my user",
                DeviceName = "my device",
                UserId = userGuid,
                AuthorisationCode = authCode
            };
            var res1 = await GetDataService().CreateAuthorization(createAuthRequest);
            var res2 = await GetDataService().Authorize(authRequest);
            AssertionHelper.CheckForSuccessfull(res1, "create auth request in AddCreateValidatedDevice");
            AssertionHelper.CheckForSuccessfull(res2, "auth request in AddCreateValidatedDevice");
            return newDeviceId;
        }

        public void Dispose()
        {
            CleanUpApi().Wait();
        }
    }
}
