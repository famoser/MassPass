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
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Services;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.Tests.Data.Mocks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;

namespace Famoser.MassPass.Tests.Data.Api
{
    public class ApiHelper : SingletonBase<ApiHelper> , IDisposable
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

        public string ErrorMessage(ApiResponse resp)
        {
            if (resp.IsSuccessfull)
                return "Request successfull";

            var res = "Request failed!";
            res += "\nRequest successfull: " + !resp.RequestFailed;
            res += "\nApi Successfull: " + resp.Successfull;
            res += "\nApi Error: " +  ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, ApiError>(resp.ApiError).Description;
            res += "\nException: " + resp.Exception.Message;
            return res;
        }

        public void Dispose()
        {
            CleanUpApi().Wait();
        }
    }
}
