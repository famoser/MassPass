using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.MassPass.Business.Content.Providers.Helpers;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Helpers;
using Famoser.MassPass.Business.Managers;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Models.Content;
using Famoser.MassPass.Business.Models.Content.Base;
using Famoser.MassPass.Business.Models.Storage;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Data.Entities.Communications.Request.Entities;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace Famoser.MassPass.Business.Repositories
{
    public class ContentRepository : BaseRepository, IContentRepository
    {
        private const string ContentFolder = "content";

        private readonly IFolderStorageService _folderStorageService;
        private readonly IPasswordVaultService _passwordVaultService;
        private readonly IApiConfigurationService _apiConfigurationService;
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IApiEncryptionService _apiEncryptionService;

        public ContentRepository(IFolderStorageService folderStorageService, IPasswordVaultService passwordVaultService, IApiConfigurationService apiConfigurationService, IAuthorizationRepository authorizationRepository, IApiEncryptionService apiEncryptionService)
        {
            _folderStorageService = folderStorageService;
            _passwordVaultService = passwordVaultService;
            _authorizationRepository = authorizationRepository;
            _apiEncryptionService = apiEncryptionService;
            _apiConfigurationService = apiConfigurationService;
        }

        private async Task InitializeAsync()
        {
            using (await _initAsyncLock.LockAsync())
            {
                if (_isInitialized)
                    return;

                _isInitialized = true;

                //read from cache
                var byteCache = await _folderStorageService.GetCachedFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.EncryptedCache).Description);
                var decryptedBytes = await _passwordVaultService.DecryptWithMasterPasswordAsync(byteCache);
                var jsonCache = StorageHelper.ByteToString(decryptedBytes);

                if (jsonCache != null)
                {
                    var cacheModel = JsonConvert.DeserializeObject<EncryptedStorageModel>(jsonCache);
                    foreach (var collectionCacheModel in cacheModel.CollectionCacheModels)
                    {
                        var model = CacheHelper.ReadCache(collectionCacheModel);
                        if (model != null)
                            CollectionManager.AddCollectionModel(model);
                    }
                }
            }
        }

        private readonly AsyncLock _initAsyncLock = new AsyncLock();
        private bool _isInitialized;
        public ObservableCollection<GroupedCollectionModel> GetGroupedCollectionModels()
        {
#pragma warning disable 4014
            InitializeAsync(); //valid at this point, as the initialisation is in the background
#pragma warning restore 4014
            return CollectionManager.GroupedCollectionModels;
        }

        public Task<bool> SyncAsync()
        {
            return ExecuteSafe(async () =>
            {
                //update all remotely changed
                var client = await _authorizationRepository.GetAuthorizedApiClientAsync();
                var syncRequest = new SyncRequest();
                foreach (var baseContentModel in CollectionManager.ContentModels)
                {
                    syncRequest.RefreshEntities.Add(new RefreshEntity()
                    {
                        ContentId = baseContentModel.ContentApiInformations.ContentId,
                        VersionId = baseContentModel.ContentApiInformations.VersionId,
                        CollectionId = baseContentModel.Collection.Id
                    });
                }
                var userConfig = await _apiConfigurationService.GetUserConfigurationAsync();
                syncRequest.CollectionIds = userConfig.AuthorizationContent.CollectionIds;
                var resp1 = await client.SyncAsync(syncRequest);
                if (!resp1.IsSuccessfull)
                    return false;

                foreach (var refreshEntity in resp1.RefreshEntities)
                {
                    var bcm = CollectionManager.ContentModels.FirstOrDefault(c => c.Id == refreshEntity.ContentId);
                    if (bcm != null)
                    {
                        if (refreshEntity.ServerVersion == ServerVersion.Older ||
                            refreshEntity.ServerVersion == ServerVersion.None)
                        {
                            var uploadRequest = new UpdateRequest()
                            {
                                CollectionId = bcm.Collection.Id,
                                ContentId = bcm.Id,
                                VersionId = bcm.ContentApiInformations.VersionId
                            };
                            var json = JsonConvert.SerializeObject(bcm);
                            var encrypted = await _apiEncryptionService.EncryptFromStringAsync(json, bcm.Collection.Id);
                            var resp2 = await client.UpdateAsync(uploadRequest, encrypted);
                            if (!resp2.IsSuccessfull)
                                return false;
                        }
                    }
                    if (refreshEntity.ServerVersion == ServerVersion.Newer)
                    {
                        var collection = CollectionManager.CollectionModels.FirstOrDefault(c => c.Id == refreshEntity.CollectionId);
                        var resp3 = await client.ReadAsync(new ContentEntityRequest()
                        {
                            ContentId = refreshEntity.ContentId,
                            VersionId = refreshEntity.VersionId
                        });
                        if (resp3.IsSuccessfull)
                        {
                            var json = await _apiEncryptionService.DecryptToStringAsync(resp3.EncryptedBytes,
                                refreshEntity.CollectionId);
                            var content = ContentHelper.Deserialize(json);
                            if (collection != null)
                            {
                                if (bcm != null)
                                    CollectionManager.ReplaceContentModel(content);
                                else
                                    CollectionManager.AddContentModel(content, collection);
                            }
                            else
                            {
                                var coll = new CollectionModel(refreshEntity.CollectionId);
                                coll.ContentModels.Add(content);
                                CollectionManager.AddCollectionModel(coll);
                            }
                        }
                    }
                }

                return true;
            });
        }

        public Task<bool> LoadValues(BaseContentModel model)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> FillHistory(BaseContentModel model)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Save(BaseContentModel model)
        {
            throw new System.NotImplementedException();
        }

        //public async Task<bool> SyncAsync()
        //{
        //    try
        //    {
        //        await InitializeAsync();


        //        var userConfig = await _apiConfigurationService.GetUserConfigurationAsync();
        //        var requestHelper = await GetRequestHelper();

        //        var res = await _dataService.GetAuthorizationStatusAsync(requestHelper.AuthorizationStatusRequest());
        //        if (res.IsSuccessfull && res.IsAuthorized)
        //        {
        //            /*
        //             * Sync:
        //             * 1. Upload locally changed if possible
        //             * 2. Refresh all changed ones online
        //             * 3. collect missing from collections
        //             * 4. download missing from collections
        //             * */

        //            var syncHelper = new SyncHelper(_dataService, _errorApiReportingService, this);

        //            // 1. check if version online is same, if yes, prepare for upload
        //            var locallyChangedStack = await SyncHelper.GetLocallyChangedStack(_dataService, requestHelper);
        //            var tasks = new List<Task>();
        //            for (int i = 0; i < workerConfig.IntValue && i < userConfig.CollectionIds.Count; i++)
        //            {
        //                tasks.Add(syncHelper.UploadChangedWorker(locallyChangedStack, requestHelper));
        //            }
        //            await Task.WhenAll(tasks);
        //            tasks.Clear();

        //            // 2. refresh all changed ones
        //            var remotelyChangedStack = await SyncHelper.GetRemotelyChangedStack(_dataService, requestHelper);
        //            for (int i = 0; i < workerConfig.IntValue && i < userConfig.CollectionIds.Count; i++)
        //            {
        //                tasks.Add(syncHelper.DownloadChangedWorker(remotelyChangedStack, requestHelper));
        //            }
        //            await Task.WhenAll(tasks);
        //            tasks.Clear();

        //            // 3. collect missing
        //            var collectionStack = new ConcurrentStack<Guid>(
        //                ContentManager.FlatContentModelCollection.Select(c => c.ApiInformations.ServerRelationId).Distinct());
        //            var missingStack = new ConcurrentStack<CollectionEntryEntity>();
        //            for (int i = 0; i < workerConfig.IntValue && i < userConfig.CollectionIds.Count; i++)
        //            {
        //                tasks.Add(syncHelper.ReadCollectionWorker(collectionStack, requestHelper, missingStack));
        //            }
        //            await Task.WhenAll(tasks);
        //            tasks.Clear();

        //            // 4. download missing
        //            for (int i = 0; i < workerConfig.IntValue && i < userConfig.CollectionIds.Count; i++)
        //            {
        //                tasks.Add(syncHelper.DownloadMissingWorker(missingStack, requestHelper));
        //            }
        //            await Task.WhenAll(tasks);
        //            tasks.Clear();

        //            var cacheConfig = await _configurationService.GetConfiguration(SettingKeys.EnableCachingOfCollectionNames);
        //            if (cacheConfig.BoolValue)
        //            {
        //                await CreateCache();
        //            }

        //            return true;
        //        }
        //        _errorApiReportingService.ReportUnhandledApiError(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Instance.LogException(ex);
        //    }
        //    return false;
        //}

        //private async Task<bool> CreateCache()
        //{
        //    var cacheModel = ContentManager.CreateCacheModel();
        //    var str = JsonConvert.SerializeObject(cacheModel);
        //    var bytes = StorageHelper.StringToBytes(str);
        //    var encryptedBytes = await _passwordVaultService.EncryptWithMasterPasswordAsync(bytes);
        //    return await _folderStorageService.SetCachedFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.ApiConfiguration).Description, encryptedBytes);
        //}


        //public async Task<bool> FillValues(ContentModel model)
        //{
        //    try
        //    {
        //        var content = await _folderStorageService.GetFile(ContentFolder, model.Id.ToString());
        //        var decryptedBytes = await _passwordVaultService.DecryptWithMasterPasswordAsync(content);
        //        var jsonCache = StorageHelper.ByteToString(decryptedBytes);
        //        if (jsonCache != null)
        //        {
        //            var contentModel = JsonConvert.DeserializeObject<ContentModel>(jsonCache);
        //            CacheHelper.WriteAllValues(model, contentModel);
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Instance.LogException(ex);
        //    }
        //    return false;
        //}

        //public async Task<bool> FillHistory(ContentModel model)
        //{
        //    try
        //    {
        //        var reqHelper = await GetRequestHelper();
        //        var resp = await _dataService.GetHistoryAsync(reqHelper.ContentEntityHistoryRequest(model.ApiInformations.ServerId));
        //        if (resp.IsSuccessfull)
        //        {
        //            var users = await _devicesRepository.GetDevices();
        //            foreach (var historyEntry in resp.HistoryEntries)
        //            {
        //                var exstingHistory = model.History.FirstOrDefault(d => d.VersionId == historyEntry.VersionId);
        //                if (exstingHistory == null)
        //                {
        //                    var newHistory = new HistoryModel
        //                    {
        //                        DeviceModel = users.FirstOrDefault(u => u.DeviceId == historyEntry.DeviceId),
        //                        Parent = model
        //                    };
        //                    model.History.Add(newHistory);
        //                    exstingHistory = newHistory;
        //                }
        //                EntityConversionHelper.WriteValues(historyEntry, exstingHistory);
        //            }
        //            model.History.OrderByDescendingInside(h => h.CreationDateTime);
        //            return true;
        //        }
        //        _errorApiReportingService.ReportUnhandledApiError(resp, model);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Instance.LogException(ex);
        //    }
        //    return false;
        //}

        //private async Task<List<ContentModel>> ReadOutAll()
        //{
        //    var res = new List<ContentModel>();
        //    var files = await _folderStorageService.GetFiles(ContentFolder);
        //    foreach (var file in files)
        //    {
        //        var content = await _folderStorageService.GetFile(ContentFolder, file);
        //        var decryptedBytes = await _passwordVaultService.DecryptWithMasterPasswordAsync(content);
        //        var jsonCache = StorageHelper.ByteToString(decryptedBytes);
        //        if (jsonCache != null)
        //        {
        //            var contentModel = JsonConvert.DeserializeObject<ContentModel>(jsonCache);
        //            res.Add(contentModel);
        //        }
        //    }
        //    return res;
        //}

        //public async Task<bool> GetContentModelForHistory(HistoryModel model)
        //{
        //    try
        //    {
        //        var requestHelper = await GetRequestHelper();
        //        var response = await _dataService.ReadAsync(requestHelper.ContentEntityRequest(model.Parent.ApiInformations.ServerId, model.Parent.ApiInformations.ServerRelationId, model.VersionId));
        //        if (response.IsSuccessfull)
        //        {
        //            var newModel = new ContentModel();
        //            ResponseHelper.WriteIntoModel(response.TransferEntity, newModel);
        //            newModel.ApiInformations = response.ContentApiInformations;
        //            newModel.LocalStatus = LocalStatus.Immutable;
        //            model.ContentModel = newModel;
        //            return true;
        //        }
        //        _errorApiReportingService.ReportUnhandledApiError(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Instance.LogException(ex);
        //    }
        //    return false;
        //}

        //public async Task<bool> Save(ContentModel model)
        //{
        //    try
        //    {
        //        var maxReptitions = 60;
        //        while (model.RuntimeStatus != RuntimeStatus.Idle && model.RuntimeStatus != RuntimeStatus.SavingFailed && model.RuntimeStatus != RuntimeStatus.SyncingFailed)
        //        {
        //            await Task.Delay(TimeSpan.FromSeconds(1));
        //            if (maxReptitions-- <= 0)
        //            {
        //                LogHelper.Instance.LogFatalError("cannot save contentmodel as it is used elsewhere", this);
        //                return false;
        //            }
        //        }

        //        var requestHelper = await GetRequestHelper();
        //        var syncHelper = new SyncHelper(_dataService, _errorApiReportingService, this);
        //        await syncHelper.UploadChangedWorker(new ConcurrentStack<ContentModel>(new List<ContentModel>() { model }), requestHelper);
        //        return model.RuntimeStatus == RuntimeStatus.Idle;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Instance.LogException(ex);
        //    }
        //    LogHelper.Instance.LogFatalError("cannot save contentmodel!", this);
        //    return true;
        //}

        //public async Task<bool> SaveLocally(ContentModel model)
        //{
        //    try
        //    {
        //        var json = JsonConvert.SerializeObject(model);
        //        var bytes = StorageHelper.StringToBytes(json);
        //        var encrytedBytes = await _passwordVaultService.EncryptWithMasterPasswordAsync(bytes);
        //        if (await _folderStorageService.SaveFile(ContentFolder, model.Id.ToString(), encrytedBytes))
        //            return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Instance.LogException(ex);
        //    }
        //    LogHelper.Instance.LogFatalError("cannot save contentmodel!", this);
        //    return true;
        //}

        //private async Task<bool> DeleteAll()
        //{
        //    var tr = true;
        //    var files = await _folderStorageService.GetFiles(ContentFolder);
        //    foreach (var file in files)
        //    {
        //        tr &= await _folderStorageService.DeleteFile(ContentFolder, file);
        //    }
        //    return tr;
        //}
    }
}
