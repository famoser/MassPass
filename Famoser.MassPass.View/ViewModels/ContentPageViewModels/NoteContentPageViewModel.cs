using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Helpers;
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
                SetContentModelStatic(contentRepository.GetSampleModel(ContentTypes.Note));
        }
        
        private NoteModel _noteModel;
        public NoteModel NoteModel
        {
            get { return _noteModel; }
            set { Set(ref _noteModel, value); }
        }

        public override bool IsContentTypeApplicable(ContentTypes type)
        {
            return type == ContentTypes.Note;
        }

        public override ICustomContentModel PrepareCustomContentModel()
        {
            NoteModel = ModelConverter.ConvertToNoteModel(ContentModel);
            return NoteModel;
        }

        public override bool SaveToContentModel()
        {
            return ModelSaver.SaveNoteModel(ContentModel, NoteModel);
        }
    }
}
