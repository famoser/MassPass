using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [TestMethod]
        public async Task TestAddAndReadEntity()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
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

                //act
                var res1 = await ds.UpdateAsync(newEntity);
                entityRequest.VersionId = res1.VersionId;
                var res2 = await ds.ReadAsync(entityRequest);

                //assert
                AssertionHelper.CheckForSuccessfull(res1, "res1");
                AssertionHelper.CheckForSuccessfull(res2, "res2");
                AssertionHelper.CheckForEquality(res2.ContentEntity, entity);

                Assert.IsTrue(res1.VersionId != null, "versionId undefined");
                Assert.IsTrue(res1.ServerId == serverId, "server id was modified, not intended behaviour");
                Assert.IsTrue(res1.ServerRelationId != null, "server relation id undefined");
            }
        }

        [TestMethod]
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
                var res1 = await ds.UpdateAsync(newEntity);
                var res2 = await ds.UpdateAsync(newEntity2);
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
                var versionRes1 = await ds.RefreshAsync(refreshRequest1);
                var versionRes2 = await ds.RefreshAsync(refreshRequest2);
                var versionRes3 = await ds.RefreshAsync(refreshRequest3);


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
        
        [TestMethod]
        public async Task TestReadCollection()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var ds = helper.GetDataService();
                var guids = await helper.CreateValidatedDevice();
                var relationId = Guid.NewGuid();
                var entityGuids1 = await helper.AddEntity(guids.Item1, guids.Item2, relationId);
                var entityGuids2 = await helper.AddEntity(guids.Item1, guids.Item2, relationId);
                var entityGuids3 = await helper.AddEntity(guids.Item1, guids.Item2, relationId);

                var collectionEntriesRequestEmpty = new CollectionEntriesRequest()
                {
                    UserId = guids.Item1,
                    DeviceId = guids.Item2,
                    RelationId = relationId
                };
                var collectionEntriesRequestFull = new CollectionEntriesRequest()
                {
                    UserId = guids.Item1,
                    DeviceId = guids.Item2,
                    RelationId = relationId,
                    KnownServerIds = new List<Guid>() { entityGuids1.Item2, entityGuids2.Item2, entityGuids3.Item2 }
                };
                var collectionEntriesRequest2Of3 = new CollectionEntriesRequest()
                {
                    UserId = guids.Item1,
                    DeviceId = guids.Item2,
                    RelationId = relationId,
                    KnownServerIds = new List<Guid>() { entityGuids1.Item2, entityGuids2.Item2 }
                };

                //act
                var res1 = await ds.ReadAsync(collectionEntriesRequestEmpty);
                var res2 = await ds.ReadAsync(collectionEntriesRequestFull);
                var res3 = await ds.ReadAsync(collectionEntriesRequest2Of3);

                //assert
                AssertionHelper.CheckForSuccessfull(res1, "res1");
                AssertionHelper.CheckForSuccessfull(res2, "res2");
                AssertionHelper.CheckForSuccessfull(res3, "res3");

                Assert.IsTrue(res1.CollectionEntryEntities.Count == 3, "not all entities returned (1)");
                Assert.IsTrue(res2.CollectionEntryEntities.Count == 0, "not all entities returned (2)");
                Assert.IsTrue(res3.CollectionEntryEntities.Count == 1, "not all entities returned (3)");
                Assert.IsTrue(res3.CollectionEntryEntities[0].ServerId == entityGuids3.Item2, "not correct serverId returned (1)");
                Assert.IsTrue(res3.CollectionEntryEntities[0].VersionId == entityGuids3.Item3, "not correct versionId returned (1)");
                Assert.IsTrue(res1.CollectionEntryEntities.Any(s => s.ServerId == entityGuids3.Item2 && s.VersionId == entityGuids3.Item3), "not correct serverId or versionId returned (2)");
                Assert.IsTrue(res1.CollectionEntryEntities.Any(s => s.ServerId == entityGuids2.Item2 && s.VersionId == entityGuids3.Item3), "not correct serverId or versionId returned (3)");
                Assert.IsTrue(res1.CollectionEntryEntities.Any(s => s.ServerId == entityGuids1.Item2 && s.VersionId == entityGuids3.Item3), "not correct serverId or versionId returned (4)");
            }
        }

        [TestMethod]
        public async Task TestCollectionHistory()
        {
            using (var helper = new ApiHelper())
            {
                var ds = helper.GetDataService();
                var guids = await helper.CreateValidatedDevice();
                var relationId = Guid.NewGuid();
                var serverId = Guid.NewGuid();
                var entityGuids1 = await helper.AddEntity(guids.Item1, guids.Item2, relationId, serverId);
                var entityGuids2 = await helper.AddEntity(guids.Item1, guids.Item2, relationId, serverId);
                var entityGuids3 = await helper.AddEntity(guids.Item1, guids.Item2, relationId, serverId);

                var collectionEntriesRequestEmpty = new ContentEntityHistoryRequest()
                {
                    UserId = guids.Item1,
                    DeviceId = guids.Item2,
                    ServerId = serverId
                };
                

                //act
                var res1 = await ds.GetHistoryAsync(collectionEntriesRequestEmpty);

                //assert
                AssertionHelper.CheckForSuccessfull(res1, "res1");

                Assert.IsTrue(res1.HistoryEntries.Count == 3, "not all entities returned");
                Assert.IsTrue(res1.HistoryEntries.Any(s => s.VersionId == entityGuids3.Item3), "not correct versionId returned (2)");
                Assert.IsTrue(res1.HistoryEntries.Any(s => s.VersionId == entityGuids2.Item3), "not correct versionId returned (3)");
                Assert.IsTrue(res1.HistoryEntries.Any(s => s.VersionId == entityGuids1.Item3), "not correct versionId returned (4)");
                foreach (var historyEntry in res1.HistoryEntries)
                {
                    Assert.IsTrue(historyEntry.CreationDateTime < DateTime.Now + TimeSpan.FromSeconds(20) && historyEntry.CreationDateTime > DateTime.Now - TimeSpan.FromSeconds(20), "Datetime not correct");
                    Assert.IsTrue(historyEntry.DeviceId == guids.Item2, "DeviceId not correct");
                }
            }
        }
    }
}
