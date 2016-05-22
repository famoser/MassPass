using System;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IApiEncryptionService
    {
        Task<byte[]> Encrypt(byte[] data, Guid serverId);
        Task<byte[]> Decrypt(byte[] data, Guid serverId);
    }
}
