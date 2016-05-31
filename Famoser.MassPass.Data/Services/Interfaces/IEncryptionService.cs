using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IEncryptionService
    {
        Task<byte[]> EncryptAsync(byte[] data, byte[] key);
        Task<byte[]> DecryptAsync(byte[] data, byte[] key);
        Task<byte[]> GeneratePasswortAsync(string password);
    }
}
