using System.Collections.Generic;
using System.Linq;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Data.Entities;
using Famoser.MassPass.Data.Entities.Communications.Response.Entitites;
using Famoser.MassPass.Data.Models;
using RefreshEntity = Famoser.MassPass.Data.Entities.Communications.Request.Entities.RefreshEntity;

namespace Famoser.MassPass.Business.Helpers
{
    public class EntityConversionHelper
    {
        public static ContentModel Convert(ContentEntity entity, ContentApiInformations infos)
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

        public static void WriteValues(AuthorizedDeviceEntity entity, DeviceModel model)
        {
            model.DeviceId = entity.DeviceId;
            model.DeviceName = entity.DeviceName;
            model.AuthorizationDateTime = entity.AuthorizationDateTime;
            model.LastModificationDateTime = entity.LastModificationDateTime;
            model.LastRequestDateTime = entity.LastRequestDateTime;
        }

        public static void WriteValues(HistoryEntry entity, HistoryModel model)
        {
            model.CreationDateTime = entity.CreationDateTime;
            model.VersionId = entity.VersionId;
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
