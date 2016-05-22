using System;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IPasswordService
    {
        Task<byte[]> GetPasswordFor(Guid serverId);
    }
}
