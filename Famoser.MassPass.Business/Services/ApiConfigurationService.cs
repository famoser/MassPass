using System;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Helpers;
using Famoser.MassPass.Business.Models.Storage;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Models.Configuration;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace Famoser.MassPass.Business.Services
{
    public class ApiConfigurationService : IApiConfigurationService
    {
        private readonly IFolderStorageService _folderStorageService;
        private readonly IPasswordVaultService _passwordVaultService;

        public ApiConfigurationService(IFolderStorageService folderStorageService, IPasswordVaultService passwordVaultService)
        {
            _folderStorageService = folderStorageService;
            _passwordVaultService = passwordVaultService;
        }

        private ConfigStorageModel _config;
        private readonly AsyncLock _asyncLock = new AsyncLock();
        private bool _isInitialized = false;

        private async Task Initialize()
        {
            using (await _asyncLock.LockAsync())
            {
                if (_isInitialized)
                    return;

                _isInitialized = true;

                var encryptedBytes = await _folderStorageService.GetCachedFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.EncryptedConfiguration).Description);
                var bytes = await _passwordVaultService.DecryptWithMasterPasswordAsync(encryptedBytes);
                var json = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                _config = JsonConvert.DeserializeObject<ConfigStorageModel>(json);
            }
        }

        public async Task<ApiConfiguration> GetApiConfigurationAsync()
        {
            await Initialize();

            return _config.ApiConfiguration;
        }

        public Task<bool> SetApiConfigurationAsync(ApiConfiguration config)
        {
            throw new NotImplementedException();
        }

        public async Task<UserConfiguration> GetUserConfigurationAsync()
        {
            await Initialize();

            return _config.UserConfiguration;
        }

        public Task<bool> SetUserConfigurationAsync(UserConfiguration config)
        {
            throw new NotImplementedException();
        }
    }
}
