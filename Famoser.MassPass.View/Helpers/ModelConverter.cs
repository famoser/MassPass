using System;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services.Base;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.View.Models;
using Newtonsoft.Json;

namespace Famoser.MassPass.View.Helpers
{
    public class ModelConverter : BaseService
    {
        private static T DeserializeSafe<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex, "ModelConverter");
            }
            return default(T);
        }

        public static NoteModel ConvertToNoteModel(ContentModel entity)
        {
            var model = DeserializeSafe<NoteModel>(entity.ContentJson);
            FillNameModel(model, entity);
            return model;
        }

        public static RootModel ConvertToRootModel(ContentModel entity)
        {
            var model = new RootModel() { Children = entity.Contents };
            FillNameModel(model, entity);
            return model;
        }

        public static FolderModel ConvertToFolderModel(ContentModel entity)
        {
            var model = new FolderModel() { Children = entity.Contents };
            FillNameModel(model, entity);
            return model;
        }

        private static void FillNameModel(NameModel model, ContentModel entity)
        {
            model.Name = entity.Name;
        }
    }
}
