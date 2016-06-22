using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return DeserializeSafe<NoteModel>(entity.ContentJson);
        }

        public static RootModel ConvertToRootModel(ContentModel entity)
        {
            return new RootModel()
            {
                Name = entity.Name,
                Children = entity.Contents
            };
        }

        public static FolderModel ConvertToFolderModel(ContentModel entity)
        {
            return new FolderModel()
            {
                Name = entity.Name,
                Children = entity.Contents
            };
        }
    }
}
