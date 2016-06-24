using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Famoser.FrameworkEssentials.Services;
using Famoser.MassPass.View.ViewModels;
using ZXing;

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

        private InitialisationPageViewModel ViewModel => DataContext as InitialisationPageViewModel;

        private MediaCapture _mediaCapture;
        private async Task<bool> InitializeQrCode()
        {
            // Find all available webcams
            DeviceInformationCollection webcamList = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the proper webcam (default one)
            DeviceInformation backWebcam = (from webcam in webcamList
                                            where webcam.IsEnabled
                                            select webcam).FirstOrDefault();

            if (backWebcam != null)
            {
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
                return true;
            }
            return false;
        }

        private bool _readingQrCodes;

        private async void TakePictureButton(object sender, RoutedEventArgs e)
        {
            UrlGrid.Visibility = Visibility.Collapsed;
            PictureGrid.Visibility = Visibility.Visible;

            try
            {
                if (await InitializeQrCode())
                {
                    var imgProp = new ImageEncodingProperties { Subtype = "BMP", Width = 600, Height = 800 };
                    var bcReader = new BarcodeReader();
                    _readingQrCodes = true;

                    while (_readingQrCodes)
                    {
                        var stream = new InMemoryRandomAccessStream();
                        await _mediaCapture.CapturePhotoToStreamAsync(imgProp, stream);

                        stream.Seek(0);
                        var wbm = new WriteableBitmap(600, 800);
                        await wbm.SetSourceAsync(stream);

                        var result = bcReader.Decode(wbm);

                        if (result != null)
                        {
                            SetConfiguration(result.Text);
                        }
                    }
                }
                else
                {
                    ShowMessage("No camera could be found, sorry :(\n\nYou have to type in the url manually");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Unfortunately we cannot read the QR code, sorry :(\n\nYou have to type in the url manually");
            }
        }

        private async void ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }


        private void TypeInUrlButton(object sender, RoutedEventArgs e)
        {
            UrlGrid.Visibility = Visibility.Visible;
            PictureGrid.Visibility = Visibility.Collapsed;
        }

        private bool SetConfiguration(string config)
        {
            if (!ViewModel.CanSetApiConfiguration)
            {
                if (ViewModel.TrySetApiConfigurationCommand.CanExecute(config))
                    ViewModel.TrySetApiConfigurationCommand.Execute(config);
                if (!ViewModel.CanSetApiConfiguration)
                {
                    ShowMessage("Cannot use this particular configuration, sorry :(");
                    return false;
                }
            }
            else if (!ViewModel.CanSetUserConfiguration)
            {
                if (ViewModel.TrySetUserConfigurationCommand.CanExecute(config))
                    ViewModel.TrySetUserConfigurationCommand.Execute(config);
                if (!ViewModel.CanSetUserConfiguration)
                {
                    ShowMessage("Cannot use this particular configuration, sorry :(");
                    return false;
                }
            }

            UrlGrid.Visibility = Visibility.Collapsed;
            PictureGrid.Visibility = Visibility.Collapsed;
            return true;
        }

        private async void EvaluateUrlButton(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(UrlTextBox.Text))
            {
                try
                {
                    var rs = new RestService();
                    var res = await rs.PostAsync(new Uri(UrlTextBox.Text), new List<KeyValuePair<string, string>>());
                    if (res.IsRequestSuccessfull)
                    {
                        var resp = await res.GetResponseAsStringAsync();
                        if (SetConfiguration(resp))
                        {
                            UrlTextBox.Text = "";
                        }
                    }
                    else
                    {
                        ShowMessage("Request was not successfull, sorry :(\n\nCheck the Url and try again");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("No internet connection or webpage cannot be found, sorry :(\n\nCheck the Url and try again");
                }
            }
        }

        private void CreateUserButton(object sender, RoutedEventArgs e)
        {
            ViewModel.CreateNewUserConfiguration = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var dev = new EasClientDeviceInformation();
            DeviceName.Text = dev.SystemProductName;
            UserName.Text = dev.OperatingSystem + " User";
        }
    }
}
