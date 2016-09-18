using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public async Task AddAndReadEntity()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var client = await helper.CreateAuthorizedClient1Async();
                var entityGuid = Guid.NewGuid();
                var entityBytes = new byte[] { 12, 2, 3, 14, 61, 23, 73 };
                var collectionGuid = Guid.NewGuid();
                var versionId1 = Guid.NewGuid().ToString();

                //act
                var resp1 = await client.UpdateAsync(new UpdateRequest()
                {
                    VersionId = versionId1,
                    ContentId = entityGuid,
                    CollectionId = collectionGuid
                }, entityBytes);
                AssertionHelper.CheckForSuccessfull(resp1, "resp1");

                var resp2 = await client.ReadAsync(new ContentEntityRequest()
                {
                    VersionId = versionId1,
                    ContentId = entityGuid
                });
                AssertionHelper.CheckForSuccessfull(resp2, "resp2");
                AssertionHelper.CheckForEquality(resp2.EncryptedBytes, entityBytes);
            }
        }

        [TestMethod]
        public async Task Sync()
        {
            using (var helper = new ApiHelper())
            {
                //arrange
                var client = await helper.CreateAuthorizedClient1Async();
                var entityGuid = Guid.NewGuid();
                var entityBytes = new byte[] { 12, 2, 3, 14, 61, 23, 73 };
                var collectionGuid = Guid.NewGuid();
                var versionId1 = Guid.NewGuid().ToString();
                var versionId2 = Guid.NewGuid().ToString();
                var versionId3 = Guid.NewGuid().ToString();

                //act
                //empty test
                var resp1 = await client.SyncAsync(new SyncRequest()
                {
                    RefreshEntities = new List<RefreshEntity>()
                });
                AssertionHelper.CheckForSuccessfull(resp1, "resp1");
                Assert.IsTrue(resp1.RefreshEntities.Count == 0, "count not 0 at resp1");

                var resp2 = await client.SyncAsync(new SyncRequest()
                {
                    RefreshEntities = new List<RefreshEntity>()
                    {
                        new RefreshEntity()
                        {
                            ContentId = entityGuid,
                            VersionId = versionId1
                        }
                    }
                });
                AssertionHelper.CheckForSuccessfull(resp2, "resp2");
                Assert.IsTrue(resp2.RefreshEntities.Count == 1, "count not 1 at resp2");
                Assert.IsTrue(resp2.RefreshEntities[0].ServerVersion == ServerVersion.None, "ServerVersion not None at resp2");
                Assert.IsTrue(resp2.RefreshEntities[0].VersionId == versionId1, "VersionId wrong at resp2");
                Assert.IsTrue(resp2.RefreshEntities[0].ContentId == entityGuid, "ContentId wrong at resp2");

                //put it on server
                var resp3 = await client.UpdateAsync(new UpdateRequest()
                {
                    VersionId = versionId1,
                    ContentId = entityGuid
                }, entityBytes);
                AssertionHelper.CheckForSuccessfull(resp3, "resp3");

                //check if on server
                var resp4 = await client.SyncAsync(new SyncRequest()
                {
                    RefreshEntities = new List<RefreshEntity>()
                });
                AssertionHelper.CheckForSuccessfull(resp4, "resp4");
                Assert.IsTrue(resp4.RefreshEntities.Count == 1, "count not 1 at resp4");
                Assert.IsTrue(resp4.RefreshEntities[0].ServerVersion == ServerVersion.Newer, "ServerVersion not Newer at resp4");
                Assert.IsTrue(resp4.RefreshEntities[0].VersionId == versionId1, "VersionId wrong at resp4");
                Assert.IsTrue(resp4.RefreshEntities[0].ContentId == entityGuid, "ContentId wrong at resp4");

                //check if sync works variante 1
                var resp5 = await client.SyncAsync(new SyncRequest()
                {
                    RefreshEntities = new List<RefreshEntity>()
                    {
                        new RefreshEntity()
                        {
                            VersionId = versionId1,
                            ContentId = entityGuid
                        }
                    }
                });
                AssertionHelper.CheckForSuccessfull(resp5, "resp5");
                Assert.IsTrue(resp5.RefreshEntities.Count == 0, "count not 0 at resp5");

                //check if sync works variante 2
                var resp6 = await client.SyncAsync(new SyncRequest()
                {
                    RefreshEntities = new List<RefreshEntity>()
                    {
                        new RefreshEntity()
                        {
                            VersionId = versionId2,
                            ContentId = entityGuid
                        }
                    }
                });
                AssertionHelper.CheckForSuccessfull(resp6, "resp6");
                Assert.IsTrue(resp6.RefreshEntities.Count == 1, "count not 1 at resp6");
                Assert.IsTrue(resp6.RefreshEntities[0].ServerVersion == ServerVersion.Older, "ServerVersion not Older at resp6");
                Assert.IsTrue(resp6.RefreshEntities[0].VersionId == versionId1, "VersionId wrong at resp6");
                Assert.IsTrue(resp6.RefreshEntities[0].ContentId == entityGuid, "ContentId wrong at resp6");

                //put it on server
                var resp7 = await client.UpdateAsync(new UpdateRequest()
                {
                    VersionId = versionId2,
                    ContentId = entityGuid
                }, entityBytes);
                AssertionHelper.CheckForSuccessfull(resp7, "resp7");

                //check if on server
                var resp8 = await client.SyncAsync(new SyncRequest()
                {
                    RefreshEntities = new List<RefreshEntity>()
                });
                AssertionHelper.CheckForSuccessfull(resp8, "resp8");
                Assert.IsTrue(resp8.RefreshEntities.Count == 1, "count not 1 at resp8");
                Assert.IsTrue(resp8.RefreshEntities[0].ServerVersion == ServerVersion.Newer, "ServerVersion not Newer at resp8");
                Assert.IsTrue(resp8.RefreshEntities[0].VersionId == versionId1, "VersionId wrong at resp8");
                Assert.IsTrue(resp8.RefreshEntities[0].ContentId == entityGuid, "ContentId wrong at resp8");

                //check if sync works variante 1
                var resp9 = await client.SyncAsync(new SyncRequest()
                {
                    RefreshEntities = new List<RefreshEntity>()
                    {
                        new RefreshEntity()
                        {
                            VersionId = versionId2,
                            ContentId = entityGuid
                        }
                    }
                });
                AssertionHelper.CheckForSuccessfull(resp9, "resp9");
                Assert.IsTrue(resp9.RefreshEntities.Count == 0, "count not 0 at resp9");

                //check if sync works variante 2
                var resp10 = await client.SyncAsync(new SyncRequest()
                {
                    RefreshEntities = new List<RefreshEntity>()
                    {
                        new RefreshEntity()
                        {
                            VersionId = versionId1,
                            ContentId = entityGuid
                        }
                    }
                });
                AssertionHelper.CheckForSuccessfull(resp10, "resp10");
                Assert.IsTrue(resp10.RefreshEntities.Count == 1, "count not 1 at resp10");
                Assert.IsTrue(resp10.RefreshEntities[0].ServerVersion == ServerVersion.Newer, "ServerVersion not Newer at resp10");
                Assert.IsTrue(resp10.RefreshEntities[0].VersionId == versionId2, "VersionId wrong at resp10");
                Assert.IsTrue(resp10.RefreshEntities[0].ContentId == entityGuid, "ContentId wrong at resp10");

                //get history
                var resp11 = await client.GetHistoryAsync(new ContentEntityHistoryRequest()
                {
                    ContentId = entityGuid
                });
                AssertionHelper.CheckForSuccessfull(resp11, "resp11");
                Assert.IsTrue(resp11.HistoryEntries.Count == 2, "count not 2 at resp11");
            }
        }
    }
}
