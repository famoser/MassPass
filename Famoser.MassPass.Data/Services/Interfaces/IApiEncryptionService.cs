using System;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IApiEncryptionService
    {
        Task<byte[]> EncryptAsync(byte[] data, Guid serverId);
        Task<byte[]> DecryptAsync(byte[] data, Guid serverId);
    }
}
