using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IPasswordService
    {
        Task<byte[]> GetPasswordFor(Guid serverId);
    }
}
