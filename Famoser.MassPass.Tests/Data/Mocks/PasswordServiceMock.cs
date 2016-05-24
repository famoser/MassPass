using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Tests.Data.Mocks
{
    public class PasswordServiceMock : IPasswordService
    {
        public async Task<byte[]> GetPasswordFor(Guid serverId)
        {
            return new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
            };
        }
    }
}
