using System;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    /// <summary>
    /// the vault, it contains the decryption passwords for the collections & the user api infos 
    /// </summary>
    public interface IPasswordVaultService
    {
        Task<bool> CreateNewVault(string masterPassword);
        Task<bool> TryUnlockVaultAsync(string masterPassword);
        bool IsVaultUnLocked();
        bool LockVault();
        bool ResetTimeout();
        
        Task<bool> RegisterPasswordAsync(Guid collectionId, string password);
        Task<string> GetPasswordAsync(Guid relationId);
        
        Task<byte[]> EncryptWithMasterPasswordAsync(byte[] data);
        Task<byte[]> DecryptWithMasterPasswordAsync(byte[] data);
    }
}
