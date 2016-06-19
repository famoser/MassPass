using System.Threading.Tasks;

namespace Famoser.MassPass.View.Services.Interfaces
{
    public interface IQrCodeService
    {
        Task<byte[]> GenerateQrCode(string content);
    }
}
