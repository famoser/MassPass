using System;
using Famoser.FrameworkEssentials.Logging;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.View.Models;
using Newtonsoft.Json;

namespace Famoser.MassPass.View.Helpers
{
    public class ModelSaver
    {
        private static string SerializeSafe(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, "ModelConverter");
            }
            return null;
        }

        public static bool SaveNoteModel(ContentModel model, NoteModel note)
        {
            model.ContentJson = SerializeSafe(note);
            return SavenameModel(model, note);
        }

        public static bool SaveRootModel(ContentModel model, RootModel root)
        {
            return SavenameModel(model, root);
        }

        public static bool SaveFolderModel(ContentModel model, FolderModel folder)
        {
            return SavenameModel(model, folder);
        }

        private static bool SavenameModel(ContentModel model, NameModel name)
        {
            model.Name = name.Name;
            return true;
        }
    }
}
