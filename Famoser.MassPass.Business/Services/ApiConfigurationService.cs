using System;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Storage;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Models.Storage;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace Famoser.MassPass.Business.Services
{
    public class ApiConfigurationService : IApiConfigurationService
    {
        private readonly IFolderStorageService _folderStorageService;

        public ApiConfigurationService(IFolderStorageService folderStorageService)
        {
            _folderStorageService = folderStorageService;
        }

        private ApiStorageModel _config;
        private readonly AsyncLock _asyncLock = new AsyncLock();
        private async Task<ApiStorageModel> GetStorageModelAsync()
        {
            using (await _asyncLock.LockAsync())
            {
                if (_config == null)
                {
                    string json = null;
                    try
                    {
                        json = await _folderStorageService.GetCachedTextFileAsync(
                            ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.ApiConfiguration)
                                .Description);
                    }
                    catch (Exception)
                    {
                        //most likely file not found exception, so ignore it
                    }
                    if (json != null)
                    {
                        _config = JsonConvert.DeserializeObject<ApiStorageModel>(json);
                    }
                }
                return _config;
            }
        }

        private async Task<bool> SaveStorageModelAsync(ApiStorageModel config)
        {
            _config = config;
            var json = JsonConvert.SerializeObject(_config);
            return await _folderStorageService.SetCachedTextFileAsync(
                ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.ApiConfiguration)
                    .Description, json);
        }

        public async Task<bool> IsConfigurationReady()
        {
            var storage = await GetStorageModelAsync();
            return storage != null;
        }

        public async Task<ApiConfiguration> GetApiConfigurationAsync()
        {
            var storage = await GetStorageModelAsync();
            return storage?.ApiConfiguration;
        }

        private bool IsValidApiConfiguration(ApiConfiguration config)
        {
            if (config.Version == 1)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> SetApiConfigurationAsync(ApiConfiguration config)
        {
            try
            {
                if (IsValidApiConfiguration(config))
                {
                    var storageModel = await GetStorageModelAsync() ?? new ApiStorageModel();
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

        public bool CanSetApiConfigurationAsync(string config)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<ApiConfiguration>(config);
                return IsValidApiConfiguration(obj);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        public async Task<bool> TrySetApiConfigurationAsync(string config)
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

        private bool IsValidUserConfiguration(UserConfiguration config)
        {
            if (config.Version == 1)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> SetUserConfigurationAsync(UserConfiguration config, Guid deviceGuid)
        {
            config.DeviceId = deviceGuid;
            var storageModel = await GetStorageModelAsync() ?? new ApiStorageModel();
            storageModel.UserConfiguration = config;
            return await SaveStorageModelAsync(storageModel);
        }

        public bool CanSetUserConfigurationAsync(string config)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<UserConfiguration>(config);
                return IsValidUserConfiguration(obj);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        public async Task<bool> TrySetUserConfigurationAsync(string config, Guid deviceGuid)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<UserConfiguration>(config);
                return await SetUserConfigurationAsync(obj, deviceGuid);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }
    }
}
