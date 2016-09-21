using Famoser.FrameworkEssentials.Services;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Content.Helpers;
using Famoser.MassPass.Business.Repositories;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Business.Services;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Services;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Content;
using Famoser.MassPass.View.Content.Providers;
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

            //business
            SimpleIoc.Default.Register<IContentRepository, ContentRepository>();
            SimpleIoc.Default.Register<IDevicesRepository, DevicesRepository>();
            SimpleIoc.Default.Register<IAuthorizationRepository, AuthorizationRepository>();

            //view
            SimpleIoc.Default.Register<InitialisationPageViewModel>();
            SimpleIoc.Default.Register<PasswordPageViewModel>();
            SimpleIoc.Default.Register<SharePageViewModel>();
            SimpleIoc.Default.Register<IProgressService, ProgressService>();

            ViewContentHelper.RegisterContentModelProvider(new CollectionContentModelProvider());
            ViewContentHelper.RegisterContentModelProvider(new CreditCardContentModelProvider());
            ViewContentHelper.RegisterContentModelProvider(new LoginContentModelProvider());
            ViewContentHelper.RegisterContentModelProvider(new NoteContentModelProvider());

            //to implement: IConfigurationService, IFolderStorageService, IErrorApiReportingService, INavigationService, IQrCodeService
        }
        
        public InitialisationPageViewModel InitialisationPageViewModel => SimpleIoc.Default.GetInstance<InitialisationPageViewModel>();
        public PasswordPageViewModel PasswordPageViewModel => SimpleIoc.Default.GetInstance<PasswordPageViewModel>();
        public SharePageViewModel SharePageViewModel => SimpleIoc.Default.GetInstance<SharePageViewModel>();
        public ProgressService ProgressViewModel => SimpleIoc.Default.GetInstance<IProgressService>() as ProgressService;
    }
}
