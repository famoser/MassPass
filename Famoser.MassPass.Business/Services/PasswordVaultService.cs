using System;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Helpers;
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

        public PasswordVaultService(IEncryptionService encryptionService, IFolderStorageService folderStorageService)
        {
            _encryptionService = encryptionService;
            _folderStorageService = folderStorageService;
        }

        private VaultStorageModel _storage;
        private DateTime _lastActionDateTime;
        private byte[] _activePasswordPhrase;
        public async Task<bool> CreateNewVaultAsync(string masterPassword)
        {
            _lastActionDateTime = DateTime.Now;
            _activePasswordPhrase = await _encryptionService.GeneratePasswortPhraseAsync(masterPassword);
            _storage = new VaultStorageModel();
            return await SaveVault();
        }

        public async Task<bool> TryUnlockVaultAsync(string masterPassword)
        {
            try
            {
                _activePasswordPhrase = await _encryptionService.GeneratePasswortPhraseAsync(masterPassword);
                var bytes = await _folderStorageService.GetCachedFileAsync(
                    ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(
                        FileKeys.EncryptedPasswords).Description);
                if (bytes?.Length > 0)
                {
                    var maybeJson = await _encryptionService.DecryptAsync(bytes, _activePasswordPhrase);
                    _storage = JsonConvert.DeserializeObject<VaultStorageModel>(StorageHelper.ByteToString(maybeJson));
                } else 
                    _storage = new VaultStorageModel();
                _lastActionDateTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
                return false;
            }
            return true;
        }

        private async Task<bool> SaveVault()
        {
            if (_storage != null)
            {
                var json = JsonConvert.SerializeObject(_storage);
                var bytes = StorageHelper.StringToBytes(json);
                var encryptedBytes = await _encryptionService.EncryptAsync(bytes, _activePasswordPhrase);
                return await _folderStorageService.SetCachedFileAsync(
                    ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(
                        FileKeys.EncryptedCache).Description, encryptedBytes);
            }
            return false;
        }

        public bool IsVaultUnLocked()
        {
            if (_lastActionDateTime - _lockTimeout < DateTime.Now)
            {
                _lastActionDateTime = DateTime.Now;
                return true;
            }
            return false;
        }

        private TimeSpan _lockTimeout = new TimeSpan(0, 2, 0); //two minutes
        public void SetLockTimeout(TimeSpan span)
        {
            _lockTimeout = span;
        }

        public async Task<bool> RegisterPasswordAsync(Guid collectionId, string password)
        {
            if (IsVaultUnLocked())
            {
                _storage.Vault[collectionId] = password;
                return await SaveVault();
            }
            return false;
        }

        public async Task<string> GetPasswordAsync(Guid collectionId)
        {
            if (IsVaultUnLocked())
            {
                _lastActionDateTime = DateTime.Now;
                return _storage.Vault[collectionId];
            }
            return null;
        }

        public bool LockVault()
        {
            _lastActionDateTime = DateTime.MinValue;
            _activePasswordPhrase = null;
            return true;
        }

        public bool ResetTimeout()
        {
            return IsVaultUnLocked();
        }

        public async Task<byte[]> EncryptWithMasterPasswordAsync(byte[] data)
        {
            if (IsVaultUnLocked())
                return await _encryptionService.EncryptAsync(data, _activePasswordPhrase);
            return null;
        }

        public async Task<byte[]> DecryptWithMasterPasswordAsync(byte[] data)
        {
            if (IsVaultUnLocked())
                return await _encryptionService.DecryptAsync(data, _activePasswordPhrase);
            return null;
        }
    }
}
