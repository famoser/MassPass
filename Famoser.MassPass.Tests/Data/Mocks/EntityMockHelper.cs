using System;
using Famoser.MassPass.Data.Entities;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Tests.Data.Mocks
{
    internal class EntityMockHelper
    {
        public static ContentEntity GetContentEntity(Guid? parentId = null, Guid? id = null, Guid? typeId = null)
        {
            return new ContentEntity()
            {
                ContentFile = new byte[]  { 21,31,41,12,3,1,31,2,12},
                ContentJson = "{\"name=\"=\"hallo welt\", \"password\":\"30 tonnen mäuse\"}",
                ContentStatus = ContentStatus.Normal,
                Id = id ?? Guid.NewGuid(),
                ParentId = parentId ?? Guid.NewGuid(),
                Name = "Hallo Welt Passwort",
                TypeId = typeId ?? Guid.NewGuid()
            };
        }
    }
}
