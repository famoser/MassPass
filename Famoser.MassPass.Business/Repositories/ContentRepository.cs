using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.MassPass.Business.Content.Helpers;
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
        private readonly IDevicesRepository _devicesRepository;

        public ContentRepository(IFolderStorageService folderStorageService, IPasswordVaultService passwordVaultService, IApiConfigurationService apiConfigurationService, IAuthorizationRepository authorizationRepository, IApiEncryptionService apiEncryptionService, IDevicesRepository devicesRepository)
        {
            _folderStorageService = folderStorageService;
            _passwordVaultService = passwordVaultService;
            _authorizationRepository = authorizationRepository;
            _apiEncryptionService = apiEncryptionService;
            _devicesRepository = devicesRepository;
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
                    _cacheStorageModel = JsonConvert.DeserializeObject<CacheStorageModel>(jsonCache);
                    foreach (var collectionCacheModel in _cacheStorageModel.CollectionCacheModels)
                    {
                        var model = CacheHelper.ReadCache(collectionCacheModel);
                        if (model != null)
                            CollectionManager.AddCollectionModel(model);
                    }
                }
            }
        }

        private CacheStorageModel _cacheStorageModel;

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
                            var json = await _apiEncryptionService.DecryptToStringAsync(resp3.EncryptedBytes, refreshEntity.CollectionId);
                            var content = ContentHelper.Deserialize(json);
                            if (collection != null)
                            {
                                if (bcm != null)
                                {
                                    var provider = ContentHelper.GetProvider(bcm);
                                    provider?.WriteValues(bcm, content);
                                    await SaveLocally(bcm, true);
                                }
                                else
                                {
                                    CollectionManager.AddContentModel(content, collection);
                                    await SaveLocally(content, true);
                                }
                            }
                            else
                            {
                                var coll = new CollectionModel(refreshEntity.CollectionId);
                                coll.ContentModels.Add(content);
                                CollectionManager.AddCollectionModel(coll);
                                await SaveLocally(content, true);
                            }
                        }
                    }
                }
                await CreateCache();

                return true;
            });
        }

        private string GetFileName(BaseContentModel contentModel)
        {
            return contentModel.Id + "_" + contentModel.ContentApiInformations.VersionId;
        }

        public Task<bool> LoadValues(BaseContentModel model)
        {
            return ExecuteSafe(async () =>
            {
                model.ContentLoadingState = LoadingState.Loading;
                var content = await _folderStorageService.GetFile(ContentFolder, GetFileName(model));
                var decryptedBytes = await _passwordVaultService.DecryptWithMasterPasswordAsync(content);
                var jsonCache = StorageHelper.ByteToString(decryptedBytes);
                if (jsonCache != null)
                {
                    var source = ContentHelper.Deserialize(jsonCache);
                    var provider = ContentHelper.GetProvider(model);
                    provider?.WriteValues(model, source);
                    model.ContentLoadingState = LoadingState.Loaded;
                    return true;
                }
                model.ContentLoadingState = LoadingState.LoadingFailed;
                return false;
            });
        }

        public Task<bool> FillHistory(BaseContentModel model)
        {
            return ExecuteSafe(async () =>
            {
                model.HistoryLoadingState = LoadingState.Loading;
                var client = await _authorizationRepository.GetAuthorizedApiClientAsync();
                var req1 = await client.GetHistoryAsync(new ContentEntityHistoryRequest()
                {
                    ContentId = model.Id
                });
                if (req1.IsSuccessfull)
                {
                    var devices = await _devicesRepository.GetDevices();
                    model.History.Clear();
                    foreach (var historyEntry in req1.HistoryEntries)
                    {
                        model.History.Add(new HistoryModel()
                        {
                            VersionId = historyEntry.VersionId,
                            CreationDateTime = historyEntry.CreationDateTime,
                            DeviceModel = devices.FirstOrDefault(d => d.DeviceId == historyEntry.DeviceId)
                        });
                    }
                    model.HistoryLoadingState = LoadingState.Loaded;
                    return true;
                }
                model.HistoryLoadingState = LoadingState.LoadingFailed;
                return false;
            });
        }

        public Task<bool> Save(BaseContentModel model)
        {
            return ExecuteSafe(async () =>
            {
                //new version
                model.ContentApiInformations.VersionId = Guid.NewGuid().ToString();

                var uploadRequest = new UpdateRequest()
                {
                    CollectionId = model.Collection.Id,
                    ContentId = model.Id,
                    VersionId = model.ContentApiInformations.VersionId
                };
                var json = JsonConvert.SerializeObject(model);
                var encrypted = await _apiEncryptionService.EncryptFromStringAsync(json, model.Collection.Id);
                var client = await _authorizationRepository.GetAuthorizedApiClientAsync();
                var res = await client.UpdateAsync(uploadRequest, encrypted);

                return await SaveLocally(model) && res.IsSuccessfull;
            });
        }

        public Task<bool> SaveLocally(BaseContentModel model, bool skipCacheCreation = false)
        {
            return ExecuteSafe(async () =>
            {
                var json = JsonConvert.SerializeObject(model);
                var bytes = StorageHelper.StringToBytes(json);
                var encrytedBytes = await _passwordVaultService.EncryptWithMasterPasswordAsync(bytes);
                if (await _folderStorageService.SaveFile(ContentFolder, GetFileName(model), encrytedBytes))
                {
                    if (!skipCacheCreation)
                        return await CreateCache();
                    return true;
                }
                return false;
            });
        }


        private readonly AsyncLock _createCacheAsyncLock = new AsyncLock();
        private Task<bool> CreateCache()
        {
            return ExecuteSafe(async () =>
            {
                using (await _createCacheAsyncLock.LockAsync())
                {
                    var collections = CollectionManager.CollectionModels;
                    _cacheStorageModel.CollectionCacheModels.Clear();
                    foreach (var collectionModel in collections)
                        _cacheStorageModel.CollectionCacheModels.Add(CacheHelper.CreateCache(collectionModel));

                    var str = JsonConvert.SerializeObject(_cacheStorageModel);
                    var bytes = StorageHelper.StringToBytes(str);
                    var encryptedBytes = await _passwordVaultService.EncryptWithMasterPasswordAsync(bytes);
                    return
                        await
                            _folderStorageService.SetCachedFileAsync(
                                ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(
                                    FileKeys.EncryptedCache).Description, encryptedBytes);
                }
            });
        }

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
