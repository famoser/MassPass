using System.Threading.Tasks;
using Famoser.MassPass.View.Services.Interfaces;
using ZXing;

namespace Famoser.MassPass.Presentation.UniversalWindows.Services
{
    public class QrCodeService : IQrCodeService
    {
        public Task<byte[]> GenerateQrCode(string content)
        {
            return Task.Run(() =>
            {
                var writer = new BarcodeWriter()
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = 160,
                        Width = 160
                    }
                };

                var result = writer.Encode(content); 
                /*
                var wb = result.ToBitmap() as WriteableBitmap;
                return wb.PixelBuffer.ToArray();
                */
                return new byte[2];
            });
        }
    }
}
