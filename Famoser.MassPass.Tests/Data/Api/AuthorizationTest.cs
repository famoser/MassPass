using System;
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
        public void TestAuthroization()
        {
            InitializeIoc();
            var dataService = SimpleIoc.Default.GetInstance<IDataService>();
            var userGuid = Guid.NewGuid();
            var deviceGuid = Guid.NewGuid();
            var authRequest = new AuthorizationRequest()
            {
                DeviceId = deviceGuid,
                UserName = "my user",
                DeviceName = "my device",
                UserId = userGuid
            };

            dataService.Authorize()
        }
    }
}
