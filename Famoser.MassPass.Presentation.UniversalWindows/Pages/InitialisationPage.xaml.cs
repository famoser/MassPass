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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var dev = new EasClientDeviceInformation();
            //DeviceName.Text = dev.SystemProductName;
            //UserName.Text = dev.OperatingSystem + " User";
        }
    }
}
