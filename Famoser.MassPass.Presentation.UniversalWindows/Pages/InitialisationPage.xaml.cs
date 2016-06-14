using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
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

        private MobileBarcodeScanner _scanner;
        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _scanner = new MobileBarcodeScanner(this.Dispatcher);
            _scanner.UseCustomOverlay = false;
            _scanner.TopText = "Hold camera up to QR code";
            _scanner.BottomText = "Camera will automatically scan QR code\r\n\rPress the 'Back' button to cancel";

            var result = await _scanner.Scan();
            ProcessScanResult(result);
        }

        private async void ProcessScanResult(Result result)
        {
            var message = (result != null && !string.IsNullOrEmpty(result.Text)) ? "Found QR code: " + result.Text : "Scanning cancelled";

            MessageDialog diag = new MessageDialog(message);
            await diag.ShowAsync();
        }
    }
}
