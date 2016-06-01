using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Data.Models;
using Famoser.MassPass.Data.Models.Storage;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Services
{
    public class ApiConfigurationService : IApiConfigurationService
    {
        private IStorageService _storageService;

        public ApiConfigurationService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        private ApiConfiguration _config;
        public async Task<ApiConfiguration> GetApiConfigurationAsync()
        {
            try
            {
                if (_config == null)
                {
                    var json = await _storageService.GetCachedTextFileAsync(
                        ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.ApiConfiguration)
                            .Description);
                    if (json != null)
                    {
                        _config = JsonConvert.DeserializeObject<ApiConfiguration>(json);
                    }
                }
                return _config;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return null;
        }

        public async Task<bool> SetApiConfigurationAsync(ApiConfiguration config)
        {
            try
            {
                if (config.Version == 1)
                {
                    _config = config;
                    var json = JsonConvert.SerializeObject(_config);
                    return await _storageService.SetCachedTextFileAsync(
                        ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.ApiConfiguration)
                            .Description, json);
                }
                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        public async Task<bool> SetApiConfigurationAsync(string config)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<ApiConfiguration>(config);
                return await SetApiConfigurationAsync(obj);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }
    }
}
