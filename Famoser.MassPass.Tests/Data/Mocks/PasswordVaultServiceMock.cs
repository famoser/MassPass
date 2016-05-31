using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Tests.Data.Mocks
{
    #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public class PasswordVaultServiceMock : IPasswordVaultService
    {
        private Dictionary<Guid, byte[]> _passwords = new Dictionary<Guid, byte[]>();
        
        public async Task<bool> UnlockVaultAsync(string password)
        {
            return true;
        }

        public async Task<byte[]> GetPasswordAsync(Guid relationId)
        {
            if (_passwords.ContainsKey(relationId))
                return _passwords[relationId];
            return new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
            };
        }
        
        public async Task<bool> RegisterPasswordAsync(Guid relationId, byte[] password)
        {
            _passwords.Add(relationId, password);
            return true;
        }

        public async Task<bool> LockVaultAsync()
        {
            return true;
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
