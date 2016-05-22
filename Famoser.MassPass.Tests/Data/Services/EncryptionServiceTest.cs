using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Services;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.Tests.Data.Mocks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.MassPass.Tests.Data.Services
{
    [TestClass]
    public class EncryptionServiceTest
    {
        private void RegisterMocks()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IConfigurationService, ConfigurationServiceMock>();
        }

        [TestMethod]
        public async Task AlwaysSameKeyGenerated()
        {
            //arrange
            RegisterMocks();
            SimpleIoc.Default.Register<IEncryptionService, EncryptionService>();
            var encService = SimpleIoc.Default.GetInstance<IEncryptionService>();
            var passwort = Guid.NewGuid().ToString();
            var testingCases = 100;
            var results = new List<byte[]>();

            //act
            for (int i = 0; i < testingCases; i++)
            {
                results.Add(await encService.GeneratePasswort(passwort));
            }

            //assert
            for (int i = 1; i < results.Count; i++)
            {
                for (int j = 0; j < results[i].Length; j++)
                {
                    Assert.IsTrue(results[i][j] == results[i - 1][j]);
                }
            }
        }

        [TestMethod]
        public async Task SymetricEncryption()
        {
            //arrange
            RegisterMocks();
            SimpleIoc.Default.Register<IEncryptionService, EncryptionService>();
            var encService = SimpleIoc.Default.GetInstance<IEncryptionService>();
            var key = await encService.GeneratePasswort(Guid.NewGuid().ToString());
            var data = new byte[] { 234, 234, 13, 41, 24, 143, 12, 32, 12, 34, 123, 12, 41, 41, 23, 12, 3, 41, 41, 41, 2, 41, 231, 24, 12, 4, 31, 41, 24, 1, 42, 12 };

            //act
            var encrypted = await encService.Encrypt(data, key);
            var decrypted = await encService.Decrypt(encrypted, key);

            //assert
            Assert.IsTrue(decrypted.Length == data.Length, "not same length!");
            for (int i = 0; i < decrypted.Length; i++)
            {
                Assert.IsTrue(data[i] == decrypted[i], "not same entry at " + i);
            }
        }

        [TestMethod]
        public async Task TestAvailableKeySizes()
        {
            //arrange
            RegisterMocks();
            SimpleIoc.Default.Register<IEncryptionService, EncryptionService>();
            var encService = SimpleIoc.Default.GetInstance<IEncryptionService>();
            var availableKeySizes = new List<int>();
            var data = new byte[] { 234, 234, 13, 41, 24, 143, 12, 32, 12, 34, 123, 12, 41, 41, 23, 12, 3, 41, 41, 41, 2, 41, 231, 24, 12, 4, 31, 41, 24, 1, 42, 12 };

            //act
            for (int i = 0; i < 129; i++)
            {
                var key = new byte[i];
                try
                {
                    var encrypted = await encService.Encrypt(data, key);
                    availableKeySizes.Add(i);
                }
                catch
                {
                    //argument exception throws if invalid key size
                }
            }

            //assert
            Assert.IsTrue(availableKeySizes.Contains(8));
            Assert.IsTrue(availableKeySizes.Contains(16));
            Assert.IsTrue(availableKeySizes.Contains(32));
            Assert.IsTrue(availableKeySizes.Count() == 3);
        }

        [TestMethod]
        public async Task TestInitialisationVectorSize()
        {
            //arrange
            RegisterMocks();
            SimpleIoc.Default.Register<IEncryptionService, EncryptionService>();
            var encService = SimpleIoc.Default.GetInstance<IEncryptionService>();
            var configService = SimpleIoc.Default.GetInstance<IConfigurationService>();
            var availableIvSizes = new List<int>();
            var data = new byte[] { 234, 234, 13, 41, 24, 143, 12, 32, 12, 34, 123, 12, 41, 41, 23, 12, 3, 41, 41, 41, 2, 41, 231, 24, 12, 4, 31, 41, 24, 1, 42, 12 };
            var key = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

            //act
            for (int i = 0; i < 65; i++)
            {
                var config = await configService.GetApiConfiguration();
                config.InitialisationVector = new byte[i];
                await configService.SetApiConfiguration(config);
                try
                {
                    var encrypted = await encService.Encrypt(data, key);
                    availableIvSizes.Add(i);
                }
                catch
                {
                    //argument exception throws if invalid key size
                }
            }

            //assert
            Assert.IsTrue(availableIvSizes.Contains(16));
            Assert.IsTrue(availableIvSizes.Count == 1);
        }
    }
}
