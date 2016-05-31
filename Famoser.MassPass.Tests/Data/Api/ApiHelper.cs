using System;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Singleton;
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
        private static IDataService _dataService;
        private static IApiConfigurationService _apiConfigurationService;

        public ApiHelper()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IEncryptionService, EncryptionService>();
            SimpleIoc.Default.Register<IDataService, DataService>();
            SimpleIoc.Default.Register<IApiEncryptionService, ApiEncryptionService>();
            SimpleIoc.Default.Register<IPasswordVaultService, PasswordVaultServiceMock>();
            SimpleIoc.Default.Register<IApiConfigurationService, ApiConfigurationServiceMock>();
            _dataService = SimpleIoc.Default.GetInstance<IDataService>();
            _apiConfigurationService = SimpleIoc.Default.GetInstance<IApiConfigurationService>();
        }

        public IDataService GetDataService()
        {
            return _dataService;
        }

        public static async Task CleanUpApi()
        {
            var config = await _apiConfigurationService.GetApiConfigurationAsync();
            var newUri = new Uri(config.Uri.AbsoluteUri + "/1.0/cleanup");
            var service = new HttpService();
            service.FireAndForget(newUri);
        }

        /// <summary>
        /// validate a new user to the API
        /// </summary>
        /// <returns>Tuple with Item1 = userGuid and Item2 = deviceGuid</returns>
        public async Task<Tuple<Guid, Guid>> CreateValidatedDevice(string userName = "my user", string deviceName = "my device")
        {
            var userGuid = Guid.NewGuid();
            var deviceGuid = Guid.NewGuid();
            var authRequest = new AuthorizationRequest
            {
                DeviceId = deviceGuid,
                UserName = userName,
                DeviceName = deviceName,
                UserId = userGuid
            };
            var res = await GetDataService().AuthorizeAsync(authRequest);
            AssertionHelper.CheckForSuccessfull(res, "auth request in CreateValidatesDevice");
            return new Tuple<Guid, Guid>(userGuid, deviceGuid);
        }

        /// <summary>
        /// validate a new user to the API
        /// </summary>
        /// <returns>Tuple with Item1 = userGuid and Item2 = deviceGuid</returns>
        public async Task<Guid> AddValidatedDevice(Guid userId, Guid deviceId, string deviceName = "new device", string userName = "my user")
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
                UserName = userName,
                DeviceName = deviceName,
                UserId = userGuid,
                AuthorisationCode = authCode
            };
            var res1 = await GetDataService().CreateAuthorizationAsync(createAuthRequest);
            var res2 = await GetDataService().AuthorizeAsync(authRequest);
            AssertionHelper.CheckForSuccessfull(res1, "create auth request in AddCreateValidatedDevice");
            AssertionHelper.CheckForSuccessfull(res2, "auth request in AddCreateValidatedDevice");
            return newDeviceId;
        }

        /// <summary>
        /// validate a new user to the API
        /// </summary>
        /// <returns>Tuple with Item1 = userGuid and Item2 = deviceGuid</returns>
        public async Task<Tuple<Guid, Guid, string>> AddEntity(Guid userId, Guid deviceId, Guid? relationId = null, Guid? serverId = null)
        {
            serverId = serverId ?? Guid.NewGuid();
            relationId = relationId ?? Guid.NewGuid();
            var newEntity = new UpdateRequest()
            {
                UserId = userId,
                DeviceId = deviceId,
                ServerId = serverId.Value,
                RelationId = relationId.Value,
                ContentEntity = EntityMockHelper.GetContentEntity()
            };

            var res1 = await GetDataService().UpdateAsync(newEntity);
            AssertionHelper.CheckForSuccessfull(res1, "AddEntity request in AddEntity");
            return new Tuple<Guid, Guid, string>(relationId.Value, serverId.Value, res1.VersionId);
        }

        public void Dispose()
        {
            CleanUpApi().Wait();
        }
    }
}
