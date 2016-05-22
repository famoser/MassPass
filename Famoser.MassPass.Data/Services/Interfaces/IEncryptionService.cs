using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IEncryptionService
    {
        Task<byte[]> Encrypt(byte[] data, byte[] key);
        Task<byte[]> Decrypt(byte[] data, byte[] key);
        Task<byte[]> GeneratePasswort(string password);
    }
}
