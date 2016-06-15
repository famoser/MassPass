using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using com.google.zxing;
using com.google.zxing.multi;
using ZXing;
using ZXing.Mobile;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Famoser.MassPass.Presentation.UniversalWindows.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InitialisationPage : Page
    {
        public InitialisationPage()
        {
            this.InitializeComponent();
        }

        private MediaCapture _mediaCapture;
        private async Task InitializeQrCode()
        {
            // Find all available webcams
            DeviceInformationCollection webcamList = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the proper webcam (default one)
            DeviceInformation backWebcam = (from webcam in webcamList
                                            where webcam.IsEnabled
                                            select webcam).FirstOrDefault();

            // Initializing MediaCapture
            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
            {
                VideoDeviceId = backWebcam.Id,
                AudioDeviceId = "",
                StreamingCaptureMode = StreamingCaptureMode.Video,
                PhotoCaptureSource = PhotoCaptureSource.VideoPreview
            });

            // Set the source of CaptureElement to MediaCapture
            BarcodeCaptureElement.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync();
        }


        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.VideoCapture);

                if (devices.Count > 1)
                {
                    await InitializeQrCode();

                    var imgProp = new ImageEncodingProperties { Subtype = "BMP", Width = 600, Height = 800 };
                    var bcReader = new BarcodeReader();

                    while (true)
                    {
                        var stream = new InMemoryRandomAccessStream();
                        await _mediaCapture.CapturePhotoToStreamAsync(imgProp, stream);

                        stream.Seek(0);
                        var wbm = new WriteableBitmap(600, 800);
                        await wbm.SetSourceAsync(stream);

                        var result = bcReader.Decode(wbm);

                        if (result != null)
                        {
                            var msgbox = new MessageDialog(result.Text);
                            await msgbox.ShowAsync();
                        }
                    }
                }
                else
                {
                    var dialog = new MessageDialog("No camera could be found; you have to type in the url manually, sorry :(");
                    await dialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }
}
