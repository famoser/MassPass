using Famoser.MassPass.Business.Models;
using Famoser.MassPass.View.Models;
using Newtonsoft.Json;

namespace Famoser.MassPass.View.Helpers
{
    public class ModelSaver
    {
        public static bool SaveNoteModel(ContentModel model, NoteModel note)
        {
            model.ContentJson = JsonConvert.SerializeObject(note);
            return true;
        }
        public static bool SaveRootModel(ContentModel model, RootModel root)
        {
            model.Name = root.Name;
            return true;
        }
        public static bool SaveFolderModel(ContentModel model, FolderModel folder)
        {
            model.Name = folder.Name;
            return true;
        }
    }
}
