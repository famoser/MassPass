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
        public NoteContentPageViewModel(IPasswordVaultService passwordVaultService, IHistoryNavigationService historyNavigationService, IContentRepository contentRepository)
            : base(passwordVaultService, historyNavigationService, contentRepository)
        {
            if (IsInDesignMode)
                SetContentModel(contentRepository.GetSampleModel(ContentTypes.Note));
            else
                SetContentModel(ContentModel = contentRepository.GetRootModelAndLoad());
        }
        
        private NoteModel _noteModel;
        public NoteModel NoteModel
        {
            get { return _noteModel; }
            set { Set(ref _noteModel, value); }
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
