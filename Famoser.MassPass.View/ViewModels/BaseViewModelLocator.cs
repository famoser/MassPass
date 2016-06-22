using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Repositories;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Business.Services;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Services;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.ViewModels.ContentPageViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using ApiEncryptionService = Famoser.MassPass.Data.Services.ApiEncryptionService;

namespace Famoser.MassPass.View.ViewModels
{
    public class BaseViewModelLocator : ViewModelBase
    {
        public BaseViewModelLocator()
        {
            //data
            SimpleIoc.Default.Register<IApiEncryptionService, ApiEncryptionService>();
            SimpleIoc.Default.Register<IApiConfigurationService, ApiConfigurationService>();
            SimpleIoc.Default.Register<IEncryptionService, EncryptionService>();
            SimpleIoc.Default.Register<IPasswordVaultService, PasswordVaultService>();
            SimpleIoc.Default.Register<IRestService, RestService>();
            SimpleIoc.Default.Register<IDataService, DataService>();

            //business
            SimpleIoc.Default.Register<IConfigurationService, ConfigurationService>();
            SimpleIoc.Default.Register<IContentRepository, ContentRepository>();
            SimpleIoc.Default.Register<IDevicesRepository, DevicesRepository>();

            //view
            SimpleIoc.Default.Register<ContentPageViewModel>();
            SimpleIoc.Default.Register<ContentPageViewModel>();
            SimpleIoc.Default.Register<InitialisationPageViewModel>();
            SimpleIoc.Default.Register<PasswordPageViewModel>();

            //to implement: IFolderStorageService, IErrorApiReportingService, INavigationService, IQrCodeService

            SimpleIoc.Default.Register<ContentPageViewModel>();
            SimpleIoc.Default.Register<ContentPageViewModel>();
            SimpleIoc.Default.Register<InitialisationPageViewModel>();
            SimpleIoc.Default.Register<PasswordPageViewModel>();
            SimpleIoc.Default.Register<SharePageViewModel>();
        }

        public NotePageViewModel ContentPageViewModel => SimpleIoc.Default.GetInstance<NotePageViewModel>();
        public InitialisationPageViewModel InitialisationPageViewModel => SimpleIoc.Default.GetInstance<InitialisationPageViewModel>();
        public PasswordPageViewModel PasswordPageViewModel => SimpleIoc.Default.GetInstance<PasswordPageViewModel>();
        public SharePageViewModel SharePageViewModel => SimpleIoc.Default.GetInstance<SharePageViewModel>();
    }
}
