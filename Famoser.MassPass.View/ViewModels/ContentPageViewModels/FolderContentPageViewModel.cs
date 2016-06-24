using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Helpers;
using Famoser.MassPass.View.Models;
using Famoser.MassPass.View.Models.Interfaces;

namespace Famoser.MassPass.View.ViewModels.ContentPageViewModels
{
    public class FolderContentPageViewModel : BaseContentPageViewModel
    {
        public FolderContentPageViewModel(IPasswordVaultService passwordVaultService, IHistoryNavigationService historyNavigationService, IContentRepository contentRepository, IProgressService progressService) : base(passwordVaultService, historyNavigationService, contentRepository, progressService)
        {
            if (IsInDesignMode)
                SetContentModelStatic(contentRepository.GetSampleModel(ContentTypes.Folder));
        }

        private FolderModel _folderModel;
        public FolderModel FolderModel
        {
            get { return _folderModel; }
            set { Set(ref _folderModel, value); }
        }

        public override bool IsContentTypeApplicable(ContentTypes type)
        {
            return type == ContentTypes.Folder;
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
