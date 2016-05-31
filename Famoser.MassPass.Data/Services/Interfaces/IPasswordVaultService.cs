using System;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IPasswordVaultService
    {
        Task<byte[]> GetPasswordAsync(Guid serverId);
        Task<bool> RegisterPasswordAsync(Guid serverId, byte[] password);
    }
}
