using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.Presentation.UniversalWindows.Services;
using Famoser.MassPass.View.Enums;
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
            var ns = GetNavigationService();
            SimpleIoc.Default.Register(() => ns);
        }

        private INavigationService GetNavigationService()
        {
            var navigationService = new CustomNavigationService();
            navigationService.Configure(PageKeys.Mainpage.ToString(), typeof(MainPage));
            navigationService.Configure(PageKeys.Video.ToString(), typeof(VideoPage));
            navigationService.Configure(PageKeys.Playlist.ToString(), typeof(PlaylistPage));
            navigationService.Configure(LocalPages.ChooseImagePage.ToString(), typeof(ChooseImagePage));
            return navigationService;
        }
    }
}
