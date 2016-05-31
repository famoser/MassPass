using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<byte[]> EncryptAsync(byte[] data, Guid relationId)
        {
            if (!_passwordVaultService.IsVaultUnLocked())
            {
                var passwordPhrase = _passwordVaultService.GetPassword(relationId);
                return await _encryptionService.EncryptAsync(data, passwordPhrase);
            }
            return null;
        }

        public async Task<byte[]> DecryptAsync(byte[] data, Guid relationId)
        {
            if (!_passwordVaultService.IsVaultUnLocked())
            {
                var passwordPhrase = _passwordVaultService.GetPassword(relationId);
                return await _encryptionService.DecryptAsync(data, passwordPhrase);
            }
            return null;
        }
    }
}
