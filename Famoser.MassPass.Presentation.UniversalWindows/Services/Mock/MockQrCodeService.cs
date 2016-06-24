using System;
using System.Threading.Tasks;
using Famoser.MassPass.View.Services.Interfaces;

namespace Famoser.MassPass.Presentation.UniversalWindows.Services.Mock
{
    public class MockQrCodeService : IQrCodeService
    {
        public Task<byte[]> GenerateQrCode(string content)
        {
            throw new NotImplementedException();
        }
    }
}
