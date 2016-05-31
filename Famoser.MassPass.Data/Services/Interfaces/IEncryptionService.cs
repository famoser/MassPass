using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    /// <summary>
    /// The implementation of the encryption
    /// </summary>
    public interface IEncryptionService
    {
        Task<byte[]> EncryptAsync(byte[] data, byte[] key);
        Task<byte[]> DecryptAsync(byte[] data, byte[] key);
        Task<byte[]> GeneratePasswortPhraseAsync(string password);
    }
}
