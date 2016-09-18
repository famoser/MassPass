using System;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    /// <summary>
    /// this service can encrypt & decrypt an entity
    /// </summary>
    public interface IApiEncryptionService
    {
        Task<byte[]> EncryptAsync(byte[] data, Guid collectionId);
        Task<byte[]> DecryptAsync(byte[] data, Guid collectionId);
    }
}
