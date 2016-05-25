using System;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Services;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.Tests.Data.Mocks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.MassPass.Tests.Data.Api
{
    [TestClass]
    public class AuthorizationTest
    {
        [TestMethod]
        public async Task TestRequestIsSuccessfull()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var dataService = helper.GetDataService();
                var userGuid = Guid.NewGuid();
                var deviceGuid = Guid.NewGuid();
                var authRequest = new AuthorizationRequest()
                {
                    DeviceId = deviceGuid,
                    UserName = "my user",
                    DeviceName = "my device",
                    UserId = userGuid
                };

                //act
                var res = await dataService.Authorize(authRequest);

                //assert
                Assert.IsTrue(res.IsSuccessfull, ErrorHelper.ErrorMessageForRequest(res));
                Assert.IsTrue(!res.RequestFailed, ErrorHelper.ErrorMessageForRequest(res));
                Assert.IsTrue(res.ApiError == ApiError.None, ErrorHelper.ErrorMessageForRequest(res));
                Assert.IsNull(res.Exception, ErrorHelper.ErrorMessageForRequest(res));
            }
        }

        [TestMethod]
        public async Task TestInitialAuthorization()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var dataService = helper.GetDataService();
                var userGuid = Guid.NewGuid();
                var deviceGuid = Guid.NewGuid();
                var authRequest = new AuthorizationRequest()
                {
                    DeviceId = deviceGuid,
                    UserName = "my user",
                    DeviceName = "my device",
                    UserId = userGuid
                };

                //act
                var res = await dataService.Authorize(authRequest);

                //assert
                AssertionHelper.CheckForSuccessfull(res, "res");
            }
        }

        [TestMethod]
        public async Task TestValidAuthorization()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var dataService = helper.GetDataService();
                var userGuid = Guid.NewGuid();
                var deviceGuid = Guid.NewGuid();
                var authCode = Guid.NewGuid().ToString();
                var authRequest = new AuthorizationRequest()
                {
                    DeviceId = deviceGuid,
                    UserName = "my user",
                    DeviceName = "my device",
                    UserId = userGuid
                };
                var validCreateAuthRequest = new CreateAuthorizationRequest()
                {
                    UserId = userGuid,
                    DeviceId = deviceGuid,
                    AuthorisationCode = authCode
                };
                var inValidUserCreateAuthRequest = new CreateAuthorizationRequest()
                {
                    UserId = Guid.NewGuid(),
                    DeviceId = deviceGuid,
                    AuthorisationCode = authCode
                };
                var inValidDeviceCreateAuthRequest = new CreateAuthorizationRequest()
                {
                    UserId = userGuid,
                    DeviceId = Guid.NewGuid(),
                    AuthorisationCode = authCode
                };

                //act
                var res = await dataService.Authorize(authRequest);
                var valid1 = await dataService.CreateAuthorization(validCreateAuthRequest);
                var invalidUser = await dataService.CreateAuthorization(inValidUserCreateAuthRequest);
                var invalidDevice = await dataService.CreateAuthorization(inValidDeviceCreateAuthRequest);

                //assert
                AssertionHelper.CheckForSuccessfull(res, "res");
                AssertionHelper.CheckForSuccessfull(valid1, "valid1");

                AssertionHelper.CheckForUnSuccessfull(invalidUser, "invalidUser", ApiError.NotAuthorized);
                AssertionHelper.CheckForUnSuccessfull(invalidDevice, "invalidDevice", ApiError.NotAuthorized);
            }
        }

        [TestMethod]
        public async Task TestNewAuthorization()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var dataService = helper.GetDataService();
                var guids = await helper.CreateValidatedDevice();
                var userGuid = guids.Item1;
                var deviceGuid = guids.Item2;

                var newDeviceGuid = Guid.NewGuid();
                var authCode = Guid.NewGuid().ToString();
                var validCreateAuthRequest = new CreateAuthorizationRequest()
                {
                    UserId = userGuid,
                    DeviceId = deviceGuid,
                    AuthorisationCode = authCode
                };
                var validAuthRequest = new AuthorizationRequest()
                {
                    UserId = userGuid,
                    DeviceId = newDeviceGuid,
                    AuthorisationCode = authCode
                };
                var validCreateAuthRequest2 = new CreateAuthorizationRequest()
                {
                    UserId = userGuid,
                    DeviceId = newDeviceGuid,
                    AuthorisationCode = authCode
                };

                //act
                var valid1 = await dataService.CreateAuthorization(validCreateAuthRequest);
                var valid2 = await dataService.Authorize(validAuthRequest);
                var valid3 = await dataService.CreateAuthorization(validCreateAuthRequest2);

                //assert
                AssertionHelper.CheckForSuccessfull(valid1, "valid1");
                AssertionHelper.CheckForSuccessfull(valid2, "valid2");
                AssertionHelper.CheckForSuccessfull(valid3, "valid3");
            }
        }
    }
}
