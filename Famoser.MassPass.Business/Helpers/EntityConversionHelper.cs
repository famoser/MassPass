using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Data.Entities;
using Famoser.MassPass.Data.Entities.Communications.Request.Entities;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Business.Helpers
{
    public class EntityConversionHelper
    {
        public ContentModel Convert(ContentEntity entity, EntityServerInformations infos)
        {
            return new ContentModel()
            {
                Id = entity.Id,
                TypeId = entity.TypeId,
                ContentFile = entity.ContentFile,
                ContentJson = entity.ContentJson,
                Name = entity.Name,
                EntityServerInformations = infos
            };
        }

        public List<RefreshEntity> GetRefreshEntities() 
    }
}
