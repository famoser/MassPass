using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Models.Interfaces;

namespace Famoser.MassPass.View.ViewModels.ContentPageViewModels
{
    public class RootContentPageViewModel : BaseContentPageViewModel
    {
        public RootContentPageViewModel(IPasswordVaultService passwordVaultService, IHistoryNavigationService historyNavigationService, IContentRepository contentRepository, IProgressService progressService) : base(passwordVaultService, historyNavigationService, contentRepository, progressService)
        {
            if (IsInDesignMode)
                SetContentModelStatic(contentRepository.GetSampleModel(ContentType.Root));
            else
                SetContentModelStatic(contentRepository.GetRootModelAndLoad());
        }

        private RootModel _rootModel;
        public RootModel RootModel
        {
            get { return _rootModel; }
            set { Set(ref _rootModel, value); }
        }

        public override bool IsContentTypeApplicable(ContentType type)
        {
            return type == ContentType.Root;
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
