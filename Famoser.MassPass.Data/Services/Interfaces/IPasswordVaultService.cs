using System;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    /// <summary>
    /// the vault which can be unlocked with a master pass and provides all passwords 
    /// </summary>
    public interface IPasswordVaultService
    {
        Task<bool> CreateNewVault(string password);
        Task<bool> TryUnlockVaultAsync(string password);
        Task<bool> RegisterPasswordAsync(Guid relationId, byte[] password);
        byte[] GetPassword(Guid relationId);
        bool IsVaultUnLocked();
        bool LockVault();
        bool ResetTimeout();
        
        Task<byte[]> EncryptAsync(byte[] data);
        Task<byte[]> DecryptAsync(byte[] data);
    }
}
