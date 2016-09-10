using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.UniversalWindows.Platform;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Presentation.UniversalWindows.Pages;
using Famoser.MassPass.Presentation.UniversalWindows.Pages.ContentPages;
using Famoser.MassPass.Presentation.UniversalWindows.Services;
using Famoser.MassPass.Presentation.UniversalWindows.Services.Mock;
using Famoser.MassPass.View.Enums;
using Famoser.MassPass.View.Services.Interfaces;
using Famoser.MassPass.View.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using FolderContentPage = Famoser.MassPass.Presentation.UniversalWindows.Pages.ContentPages.FolderContentPage;
using RootContentPage = Famoser.MassPass.Presentation.UniversalWindows.Pages.ContentPages.RootContentPage;

namespace Famoser.MassPass.Presentation.UniversalWindows.ViewModels
{
    public class ViewModelLocator : BaseViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (IsInDesignMode)
            {
                SimpleIoc.Default.Register<IErrorApiReportingService, MockErrorApiReportingService>();
                SimpleIoc.Default.Register<IFolderStorageService, MockFolderStorageService>();
                SimpleIoc.Default.Register<IQrCodeService, MockQrCodeService>();
                SimpleIoc.Default.Register<IHistoryNavigationService, HistoryNavigationServices>();
            }
            else
            {
                SimpleIoc.Default.Register<IErrorApiReportingService, ErrorApiReportingService>();
                SimpleIoc.Default.Register<IFolderStorageService, FolderStorageService>();
                SimpleIoc.Default.Register<IQrCodeService, QrCodeService>();

                var ns = GetNavigationService();
                SimpleIoc.Default.Register(() => ns);
            }
        }

        private IHistoryNavigationService GetNavigationService()
        {
            var navigationService = new HistoryNavigationServices();
            navigationService.Configure(PageKeys.InitialisationPage.ToString(), typeof(InitialisationPage));
            navigationService.Configure(PageKeys.RootContentPage.ToString(), typeof(RootContentPage));
            navigationService.Configure(PageKeys.FolderContentPage.ToString(), typeof(FolderContentPage));
            navigationService.Configure(PageKeys.NoteContentPage.ToString(), typeof(NoteContentPage));
            navigationService.Configure(PageKeys.SharePage.ToString(), typeof(SharePage));
            navigationService.Configure(PageKeys.UnlockPage.ToString(), typeof(UnlockPage));
            navigationService.Configure(PageKeys.AboutPage.ToString(), typeof(AboutPage));
            return navigationService;
        }
    }
}
