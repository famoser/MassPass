using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IApiEncryptionService
    {
        Task<byte[]> Encrypt(byte[] data, Guid serverId);
        Task<byte[]> Decrypt(byte[] data, Guid serverId);
    }
}
