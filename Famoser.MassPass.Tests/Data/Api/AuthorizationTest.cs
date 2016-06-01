using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities;
using Famoser.MassPass.Data.Entities.Communications;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.MassPass.Tests.Data.Api
{
    [TestClass]
    public class AuthorizationTest
    {
        [TestMethod]
        public async Task RequestIsSuccessfull()
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
                var res = await dataService.AuthorizeAsync(authRequest);

                //assert
                Assert.IsTrue(res.IsSuccessfull, ErrorHelper.ErrorMessageForRequest(res));
                Assert.IsTrue(!res.RequestFailed, ErrorHelper.ErrorMessageForRequest(res));
                Assert.IsTrue(res.ApiError == ApiError.None, ErrorHelper.ErrorMessageForRequest(res));
                Assert.IsNull(res.Exception, ErrorHelper.ErrorMessageForRequest(res));
            }
        }

        [TestMethod]
        public async Task InitialAuthorization()
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
                var res = await dataService.AuthorizeAsync(authRequest);
                var res2 = await dataService.GetAuthorizationStatusAsync(authStatusRequest);

                //assert
                AssertionHelper.CheckForSuccessfull(res, "res");
                AssertionHelper.CheckForSuccessfull(res2, "res2");
                Assert.IsTrue(res2.IsAuthorized, ErrorHelper.ErrorMessageForRequest(res2));
                Assert.IsNull(res2.UnauthorizedReason, ErrorHelper.ErrorMessageForRequest(res2));
            }
        }

        [TestMethod]
        public async Task AllRoutesNeedAuthorization()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var dataService = helper.GetDataService();
                var deviceGuid = Guid.NewGuid();
                var userGuid = Guid.NewGuid();

                var responses = new List<ApiResponse>();

                //0
                responses.Add(await dataService.GetAuthorizationStatusAsync(new AuthorizationStatusRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //1
                responses.Add(await dataService.GetAuthorizedDevicesAsync(new AuthorizedDevicesRequest
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //2
                responses.Add(await dataService.CreateAuthorizationAsync(new CreateAuthorizationRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //3
                responses.Add(await dataService.GetHistoryAsync(new ContentEntityHistoryRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //4
                responses.Add(await dataService.RefreshAsync(new RefreshRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //5
                responses.Add(await dataService.UnAuthorizeAsync(new UnAuthorizationRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //6
                responses.Add(await dataService.UpdateAsync(new UpdateRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid,
                    ContentEntity = new ContentEntity()
                }));

                //7
                responses.Add(await dataService.ReadAsync(new CollectionEntriesRequest()
                {
                    DeviceId = deviceGuid,
                    UserId = userGuid
                }));

                //8
                responses.Add(await dataService.ReadAsync(new ContentEntityRequest
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
        public async Task NewAuthorization()
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
                    AuthorisationCode = authCode,
                    DeviceName = "my device",
                    UserName = "this user"
                };
                var authStatusRequest = new AuthorizationStatusRequest()
                {
                    UserId = userGuid,
                    DeviceId = newDeviceGuid
                };

                //act
                var valid1 = await dataService.CreateAuthorizationAsync(validCreateAuthRequest);
                var valid2 = await dataService.AuthorizeAsync(validAuthRequest);
                var valid3 = await dataService.GetAuthorizationStatusAsync(authStatusRequest);

                //assert
                AssertionHelper.CheckForSuccessfull(valid1, "valid1");
                AssertionHelper.CheckForSuccessfull(valid2, "valid2");
                AssertionHelper.CheckForSuccessfull(valid3, "valid3");
                Assert.IsTrue(valid3.IsAuthorized, "valid3 not authorized");
            }
        }

        [TestMethod]
        public async Task UnAuthorization()
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
                var valid1 = await dataService.UnAuthorizeAsync(validUnAuthRequest);
                var valid2 = await dataService.GetAuthorizationStatusAsync(authStatusRequest);

                //assert
                AssertionHelper.CheckForSuccessfull(valid1, "valid1");
                AssertionHelper.CheckForSuccessfull(valid2, "valid2");
                Assert.IsFalse(valid2.IsAuthorized, ErrorHelper.ErrorMessageForRequest(valid2, "valid2"));
                Assert.IsTrue(valid2.UnauthorizedReason == unauthReason, "unauth reason not correct");
            }
        }

        [TestMethod]
        public async Task AuthroizedDevices()
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
                var valid1 = await dataService.GetAuthorizedDevicesAsync(validUnAuthRequest);

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
