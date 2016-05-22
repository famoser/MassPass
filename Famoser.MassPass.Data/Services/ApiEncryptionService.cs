using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Data.Services
{
    public class ApiEncryptionService : IApiEncryptionService
    {
        private readonly IPasswordService _passwordService;
        private readonly IEncryptionService _encryptionService;

        public ApiEncryptionService(IPasswordService passwordService, IEncryptionService encryptionService)
        {
            _passwordService = passwordService;
            _encryptionService = encryptionService;
        }

        public async Task<byte[]> Encrypt(byte[] data, Guid serverId)
        {
            var password = await _passwordService.GetPasswordFor(serverId);
            return await _encryptionService.Encrypt(data, password);
        }

        public async Task<byte[]> Decrypt(byte[] data, Guid serverId)
        {
            var password = await _passwordService.GetPasswordFor(serverId);
            return await _encryptionService.Decrypt(data, password);
        }
    }
}
