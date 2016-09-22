using System;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Business.Services
{
    public class ApiEncryptionService : IApiEncryptionService
    {
        private readonly IPasswordVaultService _passwordVaultService;
        private readonly IEncryptionService _encryptionService;

        public ApiEncryptionService(IPasswordVaultService passwordVaultService, IEncryptionService encryptionService)
        {
            _passwordVaultService = passwordVaultService;
            _encryptionService = encryptionService;
        }

        public async Task<byte[]> EncryptAsync(byte[] data, Guid collectionId)
        {
            if (!_passwordVaultService.IsVaultUnLocked())
            {
                var passwordPhrase = await _passwordVaultService.GetPasswordAsync(collectionId);
                var bytes = Encoding.UTF8.GetBytes(passwordPhrase);
                return await _encryptionService.EncryptAsync(data, bytes);
            }
            return null;
        }

        public Task<byte[]> EncryptFromStringAsync(string data, Guid collectionId)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            return EncryptAsync(bytes, collectionId);
        }

        public async Task<byte[]> DecryptAsync(byte[] data, Guid collectionId)
        {
            if (!_passwordVaultService.IsVaultUnLocked())
            {
                var passwordPhrase = await _passwordVaultService.GetPasswordAsync(collectionId);
                var bytes = Encoding.UTF8.GetBytes(passwordPhrase);
                return await _encryptionService.DecryptAsync(data, bytes);
            }
            return null;
        }

        public async Task<string> DecryptToStringAsync(byte[] data, Guid collectionId)
        {
            var decrypted = await DecryptAsync(data, collectionId);
            return Encoding.UTF8.GetString(decrypted, 0, decrypted.Length);
        }
    }
}
