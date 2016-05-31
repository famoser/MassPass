using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Tests.Data.Mocks
{
    public class PasswordVaultServiceMock : IPasswordVaultService
    {
        private Dictionary<Guid, byte[]> _passwords = new Dictionary<Guid, byte[]>();
        
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<byte[]> GetPasswordAsync(Guid serverId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (_passwords.ContainsKey(serverId))
                return _passwords[serverId];
            return new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
            };
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> RegisterPasswordAsync(Guid serverId, byte[] password)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _passwords.Add(serverId, password);
            return true;
        }
    }
}
