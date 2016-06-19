using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.UniversalWindows.Platform;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.Presentation.UniversalWindows.Pages;
using Famoser.MassPass.Presentation.UniversalWindows.Services;
using Famoser.MassPass.View.Enums;
using Famoser.MassPass.View.Services.Interfaces;
using Famoser.MassPass.View.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace Famoser.MassPass.Presentation.UniversalWindows.ViewModels
{
    public class ViewModelLocator : BaseViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IErrorApiReportingService, ErrorApiReportingService>();
            SimpleIoc.Default.Register<IFolderStorageService, FolderStorageService>();
            SimpleIoc.Default.Register<IQrCodeService, QrCodeService>();
            var ns = GetNavigationService();
            SimpleIoc.Default.Register(() => ns);
        }

        private IHistoryNavigationService GetNavigationService()
        {
            var navigationService = new HistoryNavigationServices();
            navigationService.Configure(PageKeys.InitialisationPage.ToString(), typeof(InitialisationPage));
            navigationService.Configure(PageKeys.CollectionsPage.ToString(), typeof(CollectionsPage));
            navigationService.Configure(PageKeys.ContentPage.ToString(), typeof(ContentPage));
            navigationService.Configure(PageKeys.SharePage.ToString(), typeof(SharePage));
            navigationService.Configure(PageKeys.UnlockPage.ToString(), typeof(UnlockPage));
            return navigationService;
        }
    }
}
