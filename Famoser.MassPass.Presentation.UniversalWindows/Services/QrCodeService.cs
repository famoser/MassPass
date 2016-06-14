using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
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
                IBarcodeWriter writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = 160,
                        Width = 160
                    }
                };

                var result = writer.Write(content);
                var wb = result.ToBitmap() as WriteableBitmap;
                return wb?.PixelBuffer.ToArray();
            });
        }
    }
}
