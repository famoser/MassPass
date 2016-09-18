using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.MassPass.Data;
using Famoser.MassPass.Data.Entities;
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
        public async Task InitialAuthorization()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var client = await helper.CreateAuthorizedClient1Async();

                //act
                var res = await client.CheckIsAuthorizedAsync();

                //assert
                Assert.IsTrue(res, "Authentication failed!");
            }
        }

        [TestMethod]
        public async Task AllRoutesNeedAuthorization()
        {
            using (var helper = new ApiHelper())
            {
                //arrange

                var dataService = await helper.CreateUnAuthorizedClient();

                //test all routes
                var responses = new List<ApiResponse>
                {
                    await dataService.AuthorizeAsync(new AuthorizationRequest()),
                    await dataService.CreateAuthorizationAsync(new CreateAuthorizationRequest()),
                    await dataService.CreateUserAsync(new CreateUserRequest()),
                    await dataService.GetAuthorizationStatusAsync(new AuthorizationStatusRequest()),
                    await dataService.GetAuthorizedDevicesAsync(new AuthorizedDevicesRequest()),
                    await dataService.GetHistoryAsync(new ContentEntityHistoryRequest()),
                    await dataService.ReadAsync(new ContentEntityRequest()),
                    await dataService.SyncAsync(new SyncRequest()),
                    await dataService.UnAuthorizeAsync(new UnAuthorizationRequest()),
                    
                };

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
                //arrange && act
                var client1 = await helper.CreateAuthorizedClient1Async();
                var client2 = await helper.AddValidatedDevice2Async(client1);
                
                //act
                var valid1 = await client1.CheckIsAuthorizedAsync();
                var valid2 = await client2.CheckIsAuthorizedAsync();

                //assert
                Assert.IsTrue(valid1, "valid1 failed");
                Assert.IsTrue(valid2, "valid2 failed");
            }
        }

        [TestMethod]
        public async Task UnAuthorization()
        {
            using (var helper = new ApiHelper())
            {
                //arrange && act
                var client1 = await helper.CreateAuthorizedClient1Async();
                var client2 = await helper.AddValidatedDevice2Async(client1);
                var unAuthReason = "Cause fuck you, thats why!";

                //act
                var req1 = await client1.UnAuthorizeAsync(new UnAuthorizationRequest()
                {
                    DeviceToBlockId = client2.DeviceId,
                    Reason = unAuthReason
                });
                var invalid1 = await client2.GetAuthorizationStatusAsync(new AuthorizationStatusRequest());


                //assert
                AssertionHelper.CheckForSuccessfull(req1, "req1");
                AssertionHelper.CheckForSuccessfull(invalid1, "invalid1");
                Assert.IsFalse(invalid1.IsAuthorized, ErrorHelper.ErrorMessageForRequest(invalid1, "invalid1"));
                Assert.IsTrue(invalid1.UnauthorizedReason == unAuthReason, "unAuthReason not correct");
            }
        }

        [TestMethod]
        public async Task AuthroizedDevices()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var client1 = await helper.CreateAuthorizedClient1Async();
                var client2 = await helper.AddValidatedDevice2Async(client1);

                //act
                var valid1 = await client2.GetAuthorizedDevicesAsync(new AuthorizedDevicesRequest());

                //assert
                AssertionHelper.CheckForSuccessfull(valid1, "valid1");
                Assert.IsTrue(valid1.AuthorizedDeviceEntities.Count == 2, "not valid authorized devies number");
                foreach (var authorizedDeviceEntity in valid1.AuthorizedDeviceEntities)
                {
                    AssertionHelper.CheckDateTimeNowValidity(authorizedDeviceEntity.AuthorizationDateTime, "AuthorizationDateTime");
                    AssertionHelper.CheckDateTimeNowValidity(authorizedDeviceEntity.LastModificationDateTime, "LastModificationDateTime");
                }
            }
        }
    }
}
