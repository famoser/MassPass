using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Singleton;
using Famoser.MassPass.Data.Services;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.Tests.Data.Mocks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Famoser.MassPass.Tests.Data.Api
{
    public class ApiHelper : SingletonBase<ApiHelper>
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
            
        }
    }
}
