using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Models;
using Famoser.MassPass.View.Models.Interfaces;

namespace Famoser.MassPass.View.ViewModels.ContentPageViewModels
{
    public class NoteContentPageViewModel : BaseContentPageViewModel
    {
        public NoteContentPageViewModel(IPasswordVaultService passwordVaultService, IHistoryNavigationService historyNavigationService, IContentRepository contentRepository, IProgressService progressService)
            : base(passwordVaultService, historyNavigationService, contentRepository, progressService)
        {
            if (IsInDesignMode)
                SetContentModelStatic(contentRepository.GetSampleModel(ContentType.Note));
        }
        
        private ViewNoteModel _viewNoteModel;
        public ViewNoteModel ViewNoteModel
        {
            get { return _viewNoteModel; }
            set { Set(ref _viewNoteModel, value); }
        }

        public override bool IsContentTypeApplicable(ContentType type)
        {
            return type == ContentType.Note;
        }

        public override ICustomContentModel PrepareCustomContentModel()
        {
            ViewNoteModel = ModelConverter.ConvertToNoteModel(ContentModel);
            return ViewNoteModel;
        }

        public override bool SaveToContentModel()
        {
            return ModelSaver.SaveNoteModel(ContentModel, ViewNoteModel);
        }
    }
}
