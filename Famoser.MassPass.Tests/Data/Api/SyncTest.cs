using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Base;
using Famoser.MassPass.Data.Entities;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Tests.Data.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.MassPass.Tests.Data.Api
{
    [TestClass]
    public class SyncTest
    {
        public async Task TestAddEntity()
        {
            using (var helper = new ApiHelper())
            {
                var ds = helper.GetDataService();
                var guids = await helper.CreateValidatedDevice();
                var serverId = Guid.NewGuid();
                var relationId = Guid.NewGuid();
                var entity = EntityMockHelper.GetContentEntity();

                var newEntity = new UpdateRequest()
                {
                   UserId = guids.Item1,
                   DeviceId = guids.Item2,
                   ServerId = serverId,
                   RelationId = relationId,
                   ContentEntity = entity
                };
                var entityRequest = new ContentEntityRequest()
                {
                    UserId = guids.Item1,
                    DeviceId = guids.Item2,
                    ServerId = serverId
                };

                var res1 = await ds.Update(newEntity);
                entityRequest.VersionId = res1.VersionId;
                var res2 = await ds.Read(entityRequest);


                AssertionHelper.CheckForSuccessfull(res1, "res1");
                AssertionHelper.CheckForSuccessfull(res2, "res2");
                AssertionHelper.CheckForEquality(res2.ContentEntity, entity);

                Assert.IsTrue(res1.VersionId != null, "versionId undefined");
                Assert.IsTrue(res1.ServerId == serverId, "server id was modified, not intended behaviour");
                Assert.IsTrue(res1.ServerRelationId != null, "server relation id undefined");
                ;
            }
        }

    }
}
