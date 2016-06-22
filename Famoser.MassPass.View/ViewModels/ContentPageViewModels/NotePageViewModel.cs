using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Helpers;
using Famoser.MassPass.View.Models;
using Famoser.MassPass.View.Models.Interfaces;

namespace Famoser.MassPass.View.ViewModels.ContentPageViewModels
{
    public class NotePageViewModel : ContentPageViewModel
    {
        public NotePageViewModel(IPasswordVaultService passwordVaultService, IHistoryNavigationService historyNavigationService, IContentRepository contentRepository)
            : base(passwordVaultService, historyNavigationService, contentRepository)
        {
        }
        
        private NoteModel _noteModel;
        private NoteModel NoteModel
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
