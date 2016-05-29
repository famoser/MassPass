using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
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
                    UserId = userGuid,
                    AuthorisationCode = "empty"
                };
                var authStatusRequest = new AuthorizationStatusRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                };

                //act
                var res = await dataService.Authorize(authRequest);
                var res2 = await dataService.AuthorizationStatus(authStatusRequest);

                //assert
                AssertionHelper.CheckForSuccessfull(res, "res");
                AssertionHelper.CheckForSuccessfull(res2, "res2");
                Assert.IsTrue(res2.IsAuthorized, ErrorHelper.ErrorMessageForRequest(res2));
                Assert.IsNull(res2.UnauthorizedReason, ErrorHelper.ErrorMessageForRequest(res2));
            }
        }

        [TestMethod]
        public async Task TestAllRoutesNeedAuthorization()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var dataService = helper.GetDataService();
                var deviceGuid = Guid.NewGuid();
                var userGuid = Guid.NewGuid();

                var responses = new List<ApiResponse>();

                //0
                responses.Add(await dataService.AuthorizationStatus(new AuthorizationStatusRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //1
                responses.Add(await dataService.AuthorizedDevices(new AuthorizedDevicesRequest
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //2
                responses.Add(await dataService.CreateAuthorization(new CreateAuthorizationRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //3
                responses.Add(await dataService.GetHistory(new ContentEntityHistoryRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //4
                responses.Add(await dataService.Refresh(new RefreshRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //5
                responses.Add(await dataService.UnAuthorize(new UnAuthorizationRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //6
                responses.Add(await dataService.Update(new UpdateRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //7
                responses.Add(await dataService.Read(new CollectionEntriesRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //8
                responses.Add(await dataService.Read(new ContentEntityRequest
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                for (int index = 0; index < responses.Count; index++)
                {
                    var apiResponse = responses[index];
                    AssertionHelper.CheckForUnSuccessfull(apiResponse, "request nr " + index, ApiError.NotAuthorized);
                }
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
                var authStatusRequest = new AuthorizationStatusRequest()
                {
                    UserId = userGuid,
                    DeviceId = newDeviceGuid
                };

                //act
                var valid1 = await dataService.CreateAuthorization(validCreateAuthRequest);
                var valid2 = await dataService.Authorize(validAuthRequest);
                var valid3 = await dataService.AuthorizationStatus(authStatusRequest);

                //assert
                AssertionHelper.CheckForSuccessfull(valid1, "valid1");
                AssertionHelper.CheckForSuccessfull(valid2, "valid2");
                AssertionHelper.CheckForSuccessfull(valid3, "valid3");
                Assert.IsTrue(valid3.IsAuthorized, "valid3 not authorized");
            }
        }

        [TestMethod]
        public async Task TestUnAuthorization()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var dataService = helper.GetDataService();
                var guids = await helper.CreateValidatedDevice();
                var newDeviceGuid = await helper.AddValidatedDevice(guids.Item1, guids.Item2);
                var unauthReason = "cause fuck u";

                var validUnAuthRequest = new UnAuthorizationRequest()
                {
                    UserId = guids.Item1,
                    DeviceId = guids.Item2,
                    DeviceToBlockId = newDeviceGuid,
                    Reason = unauthReason
                };
                var authStatusRequest = new AuthorizationStatusRequest()
                {
                    UserId = guids.Item1,
                    DeviceId = newDeviceGuid
                };

                //act
                var valid1 = await dataService.UnAuthorize(validUnAuthRequest);
                var valid2 = await dataService.AuthorizationStatus(authStatusRequest);

                //assert
                AssertionHelper.CheckForSuccessfull(valid1, "valid1");
                AssertionHelper.CheckForSuccessfull(valid2, "valid2");
                Assert.IsFalse(valid2.IsAuthorized, ErrorHelper.ErrorMessageForRequest(valid2, "valid2"));
                Assert.IsTrue(valid2.UnauthorizedReason == unauthReason, "unauth reason not correct");
            }
        }

        [TestMethod]
        public async Task TestAuthroizedDevices()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var dataService = helper.GetDataService();
                string userName = "user name";
                string deviceName1 = "device name 1";
                string deviceName2 = "device name 2";
                string deviceName3 = "device name 3";
                var guids = await helper.CreateValidatedDevice(userName, deviceName1);
                var guids2 = await helper.AddValidatedDevice(guids.Item1, guids.Item2, deviceName2, userName);
                var guids3 = await helper.AddValidatedDevice(guids.Item1, guids.Item2, deviceName3, userName);

                var validUnAuthRequest = new AuthorizedDevicesRequest()
                {
                    UserId = guids.Item1,
                    DeviceId = guids.Item2
                };

                //act
                var valid1 = await dataService.AuthorizedDevices(validUnAuthRequest);

                //assert
                AssertionHelper.CheckForSuccessfull(valid1, "valid1");
                Assert.IsTrue(valid1.AuthorizedDeviceEntities.Count == 3, "not valid authorized devies number");
                foreach (var authorizedDeviceEntity in valid1.AuthorizedDeviceEntities)
                {
                    if (authorizedDeviceEntity.DeviceId == guids.Item2)
                        Assert.IsTrue(authorizedDeviceEntity.DeviceName == deviceName1);
                    else if (authorizedDeviceEntity.DeviceId == guids2)
                        Assert.IsTrue(authorizedDeviceEntity.DeviceName == deviceName2);
                    else if (authorizedDeviceEntity.DeviceId == guids3)
                        Assert.IsTrue(authorizedDeviceEntity.DeviceName == deviceName3);

                    AssertionHelper.CheckDateTimeNowValidity(authorizedDeviceEntity.AuthorizationDateTime, "AuthorizationDateTime");
                    AssertionHelper.CheckDateTimeNowValidity(authorizedDeviceEntity.LastModificationDateTime, "LastModificationDateTime");
                }
            }
        }
    }
}
