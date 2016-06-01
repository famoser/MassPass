using System;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Storage;
using Famoser.MassPass.Data.Models.Storage;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Services
{
    public class ApiConfigurationService : IApiConfigurationService
    {
        private readonly IStorageService _storageService;

        public ApiConfigurationService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        private async Task<ApiStorageModel> GetStorageModelAsync()
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
                        _config = JsonConvert.DeserializeObject<ApiStorageModel>(json);
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

        private async Task<bool> SaveStorageModelAsync(ApiStorageModel config)
        {
            try
            {
                _config = config;
                if (_config != null)
                {
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

        private ApiStorageModel _config;
        public async Task<ApiConfiguration> GetApiConfigurationAsync()
        {
            var storage = await GetStorageModelAsync();
            return storage?.ApiConfiguration;
        }

        public async Task<bool> SetApiConfigurationAsync(ApiConfiguration config)
        {
            try
            {
                if (config.Version == 1)
                {
                    var storageModel = await GetStorageModelAsync();
                    storageModel.ApiConfiguration = config;
                    return await SaveStorageModelAsync(storageModel);
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

        public async Task<UserConfiguration> GetUserConfigurationAsync()
        {
            var storage = await GetStorageModelAsync();
            return storage?.UserConfiguration;
        }

        public async Task<bool> SetUserConfigurationAsync(UserConfiguration config)
        {
            try
            {
                var storageModel = await GetStorageModelAsync();
                storageModel.UserConfiguration = config;
                return await SaveStorageModelAsync(storageModel);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        public async Task<bool> SetUserConfigurationAsync(string config)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<UserConfiguration>(config);
                return await SetUserConfigurationAsync(obj);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }
    }
}
