using System;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    /// <summary>
    /// the vault which can be unlocked with a master pass and provides all passwords 
    /// </summary>
    public interface IPasswordVaultService
    {
        Task<bool> UnlockVaultAsync(byte[] password);
        Task<byte[]> GetPasswordAsync(Guid relationId);
        Task<bool> RegisterPasswordAsync(Guid relationId, byte[] password);
        Task<bool> LockVaultAsync();
    }
}
