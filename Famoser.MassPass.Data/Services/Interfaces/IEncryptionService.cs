using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IEncryptionService
    {
        Task<byte[]> Encrypt(byte[] data, string password);

        Task<byte[]> Decrypt(byte[] data, string password);
    }
}
