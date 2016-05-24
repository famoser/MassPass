using System;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
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
        public async Task TestAuthroization()
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
                Assert.IsTrue(res.IsSuccessfull, helper.ErrorMessage(res));
            }
        }
    }
}
