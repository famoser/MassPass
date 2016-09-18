using System;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Data.Services
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
            var password = await _passwordVaultService.GetPasswordAsync(collectionId);
            return await _encryptionService.EncryptAsync(data, Encoding.UTF8.GetBytes(password));
        }

        public async Task<byte[]> DecryptAsync(byte[] data, Guid collectionId)
        {
            var password = await _passwordVaultService.GetPasswordAsync(collectionId);
            return await _encryptionService.DecryptAsync(data, Encoding.UTF8.GetBytes(password));
        }
    }
}
