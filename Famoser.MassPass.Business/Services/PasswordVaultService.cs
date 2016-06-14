using System;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Helpers;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Models.Storage;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Services
{
    public class PasswordVaultService : IPasswordVaultService
    {
        private readonly IEncryptionService _encryptionService;
        private readonly IFolderStorageService _folderStorageService;
        private readonly IConfigurationService _configurationService;

        public PasswordVaultService(IEncryptionService encryptionService, IFolderStorageService folderStorageService, IConfigurationService configurationService)
        {
            _encryptionService = encryptionService;
            _folderStorageService = folderStorageService;
            _configurationService = configurationService;
        }

        private PasswordVaultStorageModel _storage;
        private DateTime _unlockDateTime;
        private DateTime _lastActionDateTime;
        private byte[] _activePasswordPhrase;
        public async Task<bool> TryUnlockVaultAsync(string password)
        {
            try
            {
                _activePasswordPhrase = await _encryptionService.GeneratePasswortPhraseAsync(password);
                var maybeJson = await _encryptionService.DecryptAsync(await GetCachedLockContent(), _activePasswordPhrase);
                _storage = JsonConvert.DeserializeObject<PasswordVaultStorageModel>(StorageHelper.ByteToString(maybeJson));
                _unlockDateTime = DateTime.Now;
                _lastActionDateTime = DateTime.Now;

                if (_timeoutConfig == null)
                    _timeoutConfig = await _configurationService.GetConfiguration(SettingKeys.LockTimout);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        private byte[] _cachedLockContent;
        private async Task<byte[]> GetCachedLockContent()
        {
            if (_cachedLockContent == null)
            {
                _cachedLockContent = await _folderStorageService.GetCachedFileAsync(
                            ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(
                                FileKeys.PasswordVault).Description);
            }
            return _cachedLockContent;
        }

        private async Task<bool> SaveCachedLockContent()
        {
            if (_storage != null)
            {
                var json = JsonConvert.SerializeObject(_storage);
                var bytes = StorageHelper.StringToBytes(json);
                _cachedLockContent = await _encryptionService.EncryptAsync(bytes, _activePasswordPhrase);
                return await _folderStorageService.SetCachedFileAsync(
                    ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(
                        FileKeys.PasswordVault).Description, _cachedLockContent);
            }
            return false;
        }

        private ConfigurationModel _timeoutConfig;
        public bool IsVaultUnLocked()
        {
            if (_lastActionDateTime - TimeSpan.FromSeconds(_timeoutConfig.IntValue) < DateTime.Now)
                return true;
            return false;
        }

        public byte[] GetPassword(Guid relationId)
        {
            if (IsVaultUnLocked())
            {
                _lastActionDateTime = DateTime.Now;
                if (_storage.Vault.ContainsKey(relationId))
                    return _storage.Vault[relationId];
            }
            return null;
        }

        public async Task<bool> RegisterPasswordAsync(Guid relationId, byte[] password)
        {
            if (IsVaultUnLocked())
            {
                _lastActionDateTime = DateTime.Now;
                _storage.Vault[relationId] = password;
                return await SaveCachedLockContent();
            }
            return false;
        }

        public bool LockVault()
        {
            _lastActionDateTime = DateTime.MinValue;
            _unlockDateTime = DateTime.MinValue;
            _activePasswordPhrase = null;
            return true;
        }

        public bool ResetTimeout()
        {
            if (IsVaultUnLocked())
            {
                _lastActionDateTime = DateTime.Now;
                return true;
            }
            return false;
        }

        public async Task<byte[]> EncryptAsync(byte[] data)
        {
            if (IsVaultUnLocked())
                await _encryptionService.EncryptAsync(data, _activePasswordPhrase);
            return null;
        }

        public async Task<byte[]> DecryptAsync(byte[] data)
        {
            if (IsVaultUnLocked())
                await _encryptionService.DecryptAsync(data, _activePasswordPhrase);
            return null;
        }
    }
}
