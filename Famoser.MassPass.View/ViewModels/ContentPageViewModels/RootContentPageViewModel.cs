using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Helpers;
using Famoser.MassPass.View.Models;
using Famoser.MassPass.View.Models.Interfaces;

namespace Famoser.MassPass.View.ViewModels.ContentPageViewModels
{
    class RootContentPageViewModel : ContentPageViewModel
    {
        public RootContentPageViewModel(IPasswordVaultService passwordVaultService, IHistoryNavigationService historyNavigationService, IContentRepository contentRepository) : base(passwordVaultService, historyNavigationService, contentRepository)
        {
            ContentModel = contentRepository.GetRootModelAndLoad();
        }

        private RootModel _rootModel;
        protected RootModel RootModel
        {
            get { return _rootModel; }
            set { Set(ref _rootModel, value); }
        }

        public override ICustomContentModel PrepareCustomContentModel()
        {
            RootModel = ModelConverter.ConvertToRootModel(ContentModel);
            return RootModel;
        }

        public override bool SaveToContentModel()
        {
            return ModelSaver.SaveRootModel(ContentModel, RootModel);
        }
    }
}
