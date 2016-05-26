using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Base;
using Famoser.MassPass.Data.Entities;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Data.Entities.Communications.Request.Entities;
using Famoser.MassPass.Data.Enum;
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
            }
        }

        public async Task TestRefresh()
        {
            using (var helper = new ApiHelper())
            {
                var ds = helper.GetDataService();
                var guids = await helper.CreateValidatedDevice();
                var serverId = Guid.NewGuid();
                var serverId2 = Guid.NewGuid();
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
                var newEntity2 = new UpdateRequest()
                {
                    UserId = guids.Item1,
                    DeviceId = guids.Item2,
                    ServerId = serverId2,
                    RelationId = relationId,
                    ContentEntity = entity
                };
                var res1 = await ds.Update(newEntity);
                var res2 = await ds.Update(newEntity2);
                AssertionHelper.CheckForSuccessfull(res1, "res1");
                AssertionHelper.CheckForSuccessfull(res2, "res2");
                var version1 = res1.VersionId;
                var version2 = res2.VersionId;
                var refreshRequest1 = new RefreshRequest()
                {
                    UserId = guids.Item1,
                    DeviceId = guids.Item2,
                    RefreshEntities = new List<RefreshEntity>()
                    {
                        new RefreshEntity()
                        {
                            ServerId = serverId,
                            VersionId = "invalidVersion"
                        },
                        new RefreshEntity()
                        {
                            ServerId = serverId2,
                            VersionId = version2
                        }
                    }
                };
                var refreshRequest2 = new RefreshRequest()
                {
                    UserId = guids.Item1,
                    DeviceId = guids.Item2,
                    RefreshEntities = new List<RefreshEntity>()
                    {
                        new RefreshEntity()
                        {
                            ServerId = serverId,
                            VersionId = version1
                        },
                        new RefreshEntity()
                        {
                            ServerId = serverId2,
                            VersionId = version2
                        }
                    }
                };
                var refreshRequest3 = new RefreshRequest()
                {
                    UserId = guids.Item1,
                    DeviceId = guids.Item2,
                    RefreshEntities = new List<RefreshEntity>()
                    {
                        new RefreshEntity()
                        {
                            ServerId = serverId,
                            VersionId = "invalid"
                        },
                        new RefreshEntity()
                        {
                            ServerId = serverId2,
                            VersionId = "invalid"
                        }
                    }
                };

                //act
                var versionRes1 = await ds.Refresh(refreshRequest1);
                var versionRes2 = await ds.Refresh(refreshRequest1);
                var versionRes3 = await ds.Refresh(refreshRequest1);


                //assert
                AssertionHelper.CheckForSuccessfull(versionRes1, "versionRes1");
                AssertionHelper.CheckForSuccessfull(versionRes2, "versionRes2");
                AssertionHelper.CheckForSuccessfull(versionRes3, "versionRes3");

                Assert.IsTrue(versionRes1.RefreshEntities.Count == 1, "versionRes1: invalid count");
                Assert.IsTrue(versionRes2.RefreshEntities.Count == 0, "versionRes2: invalid count");
                Assert.IsTrue(versionRes3.RefreshEntities.Count == 2, "versionRes3: invalid count");

                Assert.IsTrue(versionRes1.RefreshEntities[0].RemoteStatus == RemoteStatus.Changed, "versionRes1: invalid remote status");
                Assert.IsTrue(versionRes3.RefreshEntities[0].RemoteStatus == RemoteStatus.Changed, "versionRes3: invalid remote status");
                Assert.IsTrue(versionRes3.RefreshEntities[1].RemoteStatus == RemoteStatus.Changed, "versionRes3: invalid remote status (2)");

                Assert.IsTrue(versionRes1.RefreshEntities[0].VersionId == version1, "versionRes1: invalid version");
                if (versionRes3.RefreshEntities[0].VersionId == version1)
                {
                    Assert.IsTrue(versionRes3.RefreshEntities[1].VersionId == version2, "versionRes3: invalid version (1)");
                }
                else
                {
                    Assert.IsTrue(versionRes3.RefreshEntities[0].VersionId == version2, "versionRes3: invalid version (2)");
                    Assert.IsTrue(versionRes3.RefreshEntities[1].VersionId == version1, "versionRes3: invalid version (3)");
                }
            }
        }
    }
}
