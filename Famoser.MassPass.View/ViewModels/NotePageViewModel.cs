using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Helpers;
using Famoser.MassPass.View.Models;
using Famoser.MassPass.View.Models.Interfaces;

namespace Famoser.MassPass.View.ViewModels
{
    public class NotePageViewModel : ContentPageViewModel
    {
        public NotePageViewModel(IPasswordVaultService passwordVaultService, ICollectionRepository collectionRepository,
            IHistoryNavigationService historyNavigationService, IContentRepository contentRepository)
            : base(passwordVaultService, collectionRepository, historyNavigationService, contentRepository)
        {
        }
        
        private NoteModel _noteModel;
        private NoteModel NoteModel
        {
            get { return _noteModel; }
            set { Set(ref _noteModel, value); }
        }

        public override ICustomContentModel PrepareModel()
        {
            NoteModel = ModelConverter.ConvertToNoteModel(ContentModel);
            return NoteModel;
        }

        public override bool SaveModel()
        {
            return ModelSaver.SaveNoteModel(ContentModel, NoteModel);
        }
    }
}
