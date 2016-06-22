using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Helpers;
using Famoser.MassPass.View.Models;
using Famoser.MassPass.View.Models.Interfaces;

namespace Famoser.MassPass.View.ViewModels.ContentPageViewModels
{
    class FolderContentPageViewModel : ContentPageViewModel
    {
        public FolderContentPageViewModel(IPasswordVaultService passwordVaultService, IHistoryNavigationService historyNavigationService, IContentRepository contentRepository) : base(passwordVaultService, historyNavigationService, contentRepository)
        {
            ContentModel = contentRepository.GetRootModelAndLoad();
        }

        private FolderModel _folderModel;
        protected FolderModel FolderModel
        {
            get { return _folderModel; }
            set { Set(ref _folderModel, value); }
        }

        public override ICustomContentModel PrepareCustomContentModel()
        {
            FolderModel = ModelConverter.ConvertToFolderModel(ContentModel);
            return FolderModel;
        }

        public override bool SaveToContentModel()
        {
            return ModelSaver.SaveFolderModel(ContentModel, FolderModel);
        }
    }
}
