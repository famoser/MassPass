using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.View.Services.Interfaces
{
    public interface IQrCodeService
    {
        Task<byte[]> GenerateQrCode(string content);
    }
}
