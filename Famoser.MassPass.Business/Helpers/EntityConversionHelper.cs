using System.Collections.Generic;
using System.Linq;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Data.Entities;
using Famoser.MassPass.Data.Entities.Communications.Request.Entities;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Business.Helpers
{
    public class EntityConversionHelper
    {
        public static ContentModel Convert(ContentEntity entity, ApiInformations infos)
        {
            return new ContentModel()
            {
                Id = entity.Id,
                TypeId = entity.TypeId,
                ContentFile = entity.ContentFile,
                ContentJson = entity.ContentJson,
                Name = entity.Name,
                ApiInformations = infos
            };
        }

        public static List<RefreshEntity> GetRefreshEntities(List<ContentModel> models)
        {
            return models.Select(Convert).ToList();
        }

        private static RefreshEntity Convert(ContentModel model)
        {
            return new RefreshEntity()
            {
                ServerId = model.ApiInformations.ServerId,
                VersionId = model.ApiInformations.VersionId
            };
        }
    }
}
