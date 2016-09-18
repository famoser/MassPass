using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Tests.Data.Mocks
{
    #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public class PasswordVaultServiceMock : IPasswordVaultService
    {
        private readonly Dictionary<Guid, byte[]> _passwords = new Dictionary<Guid, byte[]>();
        
        public async Task<bool> TryUnlockVaultAsync(string masterPassword)
        {
            return true;
        }

        public byte[] GetPassword(Guid relationId)
        {
            if (_passwords.ContainsKey(relationId))
                return _passwords[relationId];
            return new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
            };
        }

        public bool IsVaultUnLocked()
        {
            return true;
        }

        public bool LockVault()
        {
            return true;
        }

        public bool ResetTimeout()
        {
            return true;
        }

        public async Task<byte[]> EncryptWithMasterPasswordAsync(byte[] data)
        {
            return null;
        }

        public async Task<byte[]> DecryptWithMasterPasswordAsync(byte[] data)
        {
            return null;
        }

        public async Task<bool> RegisterPasswordAsync(Guid relationId, byte[] password)
        {
            _passwords.Add(relationId, password);
            return true;
        }
    }
}
