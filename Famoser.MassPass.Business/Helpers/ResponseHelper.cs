using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Data.Entities;

namespace Famoser.MassPass.Business.Helpers
{
    public class ResponseHelper
    {
        public static void WriteIntoModel(ContentEntity entity, ContentModel model)
        {
            model.ContentFile = entity.ContentFile;
            model.ContentJson = entity.ContentJson;
            model.LivecycleStatus = entity.LivecycleStatus;
            model.Name = entity.Name;
            model.TypeId = entity.TypeId;
            model.ParentId = entity.ParentId;
            model.Id = entity.Id;
        }
    }
}
