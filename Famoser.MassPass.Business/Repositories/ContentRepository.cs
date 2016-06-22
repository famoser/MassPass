using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Extensions;
using Famoser.MassPass.Business.Helpers;
using Famoser.MassPass.Business.Managers;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Models.Storage;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Entities.Communications.Response.Entitites;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Models;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Repositories
{
    public class ContentRepository : BaseRepository, IContentRepository
    {
        private const string ContentFolder = "content";

        private readonly IFolderStorageService _folderStorageService;
        private readonly IErrorApiReportingService _errorApiReportingService;
        private readonly IPasswordVaultService _passwordVaultService;
        private readonly IDataService _dataService;
        private readonly IDevicesRepository _devicesRepository;
        private readonly IConfigurationService _configurationService;
        private readonly IApiConfigurationService _apiConfigurationService;

        public ContentRepository(IFolderStorageService folderStorageService, IPasswordVaultService passwordVaultService, IDataService dataService, IApiConfigurationService apiConfigurationService, IErrorApiReportingService errorApiReportingService, IDevicesRepository devicesRepository, IConfigurationService configurationService) : base(apiConfigurationService)
        {
            _folderStorageService = folderStorageService;
            _passwordVaultService = passwordVaultService;
            _dataService = dataService;
            _errorApiReportingService = errorApiReportingService;
            _devicesRepository = devicesRepository;
            _configurationService = configurationService;
            _apiConfigurationService = apiConfigurationService;
        }

        private Task _fillCollectionsTask;
        private bool _initialisationStarted;
        public async Task<bool> InitializeVault(string masterPassword)
        {
            try
            {
                await DeleteAll();

                await _passwordVaultService.CreateNewVault(masterPassword);

                var serverRelationGuid = Guid.NewGuid();
                await _passwordVaultService.RegisterPasswordAsync(serverRelationGuid, Guid.NewGuid().ToString());

                var parentGuid = Guid.NewGuid();
                var content = new ContentModel()
                {
                    ContentJson = @"{'Content': 'This is a note!'}",
                    Name = "Example Note",
                    Id = parentGuid,
                    ApiInformations = new ApiInformations()
                    {
                        ServerRelationId = serverRelationGuid
                    }
                };
                ContentManager.AddOrReplaceContent(content);
                await Save(content);
                content = new ContentModel()
                {
                    ContentJson = @"{'Content': 'This is a note in a note!'}",
                    Name = "Example Note",
                    Id = Guid.NewGuid(),
                    ParentId = parentGuid,
                    ApiInformations = new ApiInformations()
                    {
                        ServerRelationId = serverRelationGuid
                    }
                };
                ContentManager.AddOrReplaceContent(content);
                await Save(content);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
                return false;
            }
            return true;
        }

        public ContentModel GetRootModelAndLoad()
        {
            lock (this)
            {
                if (!_initialisationStarted)
                {
                    _initialisationStarted = true;
                    _fillCollectionsTask = FillRootContentModel();
                }
            }
            return ContentManager.RootContentModel;
        }

        private async Task FillRootContentModel()
        {
            try
            {
                var cacheConfig = await _configurationService.GetConfiguration(SettingKeys.EnableCachingOfCollectionNames);
                if (cacheConfig.BoolValue)
                {
                    //read from cache
                    var byteCache = await _folderStorageService.GetCachedFileAsync(
                                ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(
                                    FileKeys.ApiConfiguration).Description);
                    var decryptedBytes = await _passwordVaultService.DecryptAsync(byteCache);
                    var jsonCache = StorageHelper.ByteToString(decryptedBytes);

                    if (jsonCache != null)
                    {
                        var cacheModel = JsonConvert.DeserializeObject<CollectionCacheModel>(jsonCache);
                        ContentManager.AddFromCache(cacheModel);
                        return;
                    }
                }
                var contentModels = await ReadOutAll();
                foreach (var contentModel in contentModels)
                {
                    ContentManager.AddOrReplaceContent(contentModel);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
        }

        public ContentModel GetSampleModel(ContentTypes type)
        {
            var cm = new ContentModel()
            {
                Name = "Note",
                Contents =
                {
                    new ContentModel()
                    {
                        Name = "Additional1"
                    },
                    new ContentModel()
                    {
                        Name = "Additional2"
                    }
                }
            };
            if (type == ContentTypes.Note)
            {
                cm.ContentJson = @"{'Content': 'This is a note!'}";
            }
            cm.SetContentType(type);
            return cm;
        }

        public async Task<bool> SyncAsync()
        {
            try
            {
                if (_fillCollectionsTask != null)
                {
                    await _fillCollectionsTask;
                    _fillCollectionsTask = null;
                }
                var workerConfig = await _configurationService.GetConfiguration(SettingKeys.MaximumWorkerNumber);
                var userConfig = await _apiConfigurationService.GetUserConfigurationAsync();
                var requestHelper = await GetRequestHelper();

                var res = await _dataService.GetAuthorizationStatusAsync(requestHelper.AuthorizationStatusRequest());
                if (res.IsSuccessfull && res.IsAuthorized)
                {
                    /*
                     * Sync:
                     * 1. Upload locally changed if possible
                     * 2. Refresh all changed ones online
                     * 3. collect missing from collections
                     * 4. download missing from collections
                     * */

                    var syncHelper = new SyncHelper(_dataService, _errorApiReportingService, this);

                    // 1. check if version online is same, if yes, prepare for upload
                    var locallyChangedStack = await SyncHelper.GetLocallyChangedStack(_dataService, requestHelper);
                    var tasks = new List<Task>();
                    for (int i = 0; i < workerConfig.IntValue && i < userConfig.RelationIds.Count; i++)
                    {
                        tasks.Add(syncHelper.UploadChangedWorker(locallyChangedStack, requestHelper));
                    }
                    await Task.WhenAll(tasks);
                    tasks.Clear();

                    // 2. refresh all changed ones
                    var remotelyChangedStack = await SyncHelper.GetRemotelyChangedStack(_dataService, requestHelper);
                    for (int i = 0; i < workerConfig.IntValue && i < userConfig.RelationIds.Count; i++)
                    {
                        tasks.Add(syncHelper.DownloadChangedWorker(remotelyChangedStack, requestHelper));
                    }
                    await Task.WhenAll(tasks);
                    tasks.Clear();

                    // 3. collect missing
                    var collectionStack = new ConcurrentStack<Guid>(
                        ContentManager.FlatContentModelCollection.Select(c => c.ApiInformations.ServerRelationId).Distinct());
                    var missingStack = new ConcurrentStack<CollectionEntryEntity>();
                    for (int i = 0; i < workerConfig.IntValue && i < userConfig.RelationIds.Count; i++)
                    {
                        tasks.Add(syncHelper.ReadCollectionWorker(collectionStack, requestHelper, missingStack));
                    }
                    await Task.WhenAll(tasks);
                    tasks.Clear();

                    // 4. download missing
                    for (int i = 0; i < workerConfig.IntValue && i < userConfig.RelationIds.Count; i++)
                    {
                        tasks.Add(syncHelper.DownloadMissingWorker(missingStack, requestHelper));
                    }
                    await Task.WhenAll(tasks);
                    tasks.Clear();

                    var cacheConfig = await _configurationService.GetConfiguration(SettingKeys.EnableCachingOfCollectionNames);
                    if (cacheConfig.BoolValue)
                    {
                        await CreateCache();
                    }

                    return true;
                }
                _errorApiReportingService.ReportUnhandledApiError(res);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        private async Task<bool> CreateCache()
        {
            var cacheModel = ContentManager.CreateCacheModel();
            var str = JsonConvert.SerializeObject(cacheModel);
            var bytes = StorageHelper.StringToBytes(str);
            var encryptedBytes = await _passwordVaultService.EncryptAsync(bytes);
            return await _folderStorageService.SetCachedFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.ApiConfiguration).Description, encryptedBytes);
        }


        public async Task<bool> FillValues(ContentModel model)
        {
            try
            {
                var content = await _folderStorageService.GetFile(ContentFolder, model.Id.ToString());
                var decryptedBytes = await _passwordVaultService.DecryptAsync(content);
                var jsonCache = StorageHelper.ByteToString(decryptedBytes);
                if (jsonCache != null)
                {
                    var contentModel = JsonConvert.DeserializeObject<ContentModel>(jsonCache);
                    CacheHelper.WriteAllValues(model, contentModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        public async Task<bool> FillHistory(ContentModel model)
        {
            try
            {
                var reqHelper = await GetRequestHelper();
                var resp = await _dataService.GetHistoryAsync(reqHelper.ContentEntityHistoryRequest(model.ApiInformations.ServerId));
                if (resp.IsSuccessfull)
                {
                    var users = await _devicesRepository.GetDevices();
                    foreach (var historyEntry in resp.HistoryEntries)
                    {
                        var exstingHistory = model.History.FirstOrDefault(d => d.VersionId == historyEntry.VersionId);
                        if (exstingHistory == null)
                        {
                            var newHistory = new HistoryModel
                            {
                                DeviceModel = users.FirstOrDefault(u => u.DeviceId == historyEntry.DeviceId),
                                Parent = model
                            };
                            model.History.Add(newHistory);
                            exstingHistory = newHistory;
                        }
                        EntityConversionHelper.WriteValues(historyEntry, exstingHistory);
                    }
                    model.History.OrderByDescendingInside(h => h.CreationDateTime);
                    return true;
                }
                _errorApiReportingService.ReportUnhandledApiError(resp, model);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        private async Task<List<ContentModel>> ReadOutAll()
        {
            var res = new List<ContentModel>();
            var files = await _folderStorageService.GetFiles(ContentFolder);
            foreach (var file in files)
            {
                var content = await _folderStorageService.GetFile(ContentFolder, file);
                var decryptedBytes = await _passwordVaultService.DecryptAsync(content);
                var jsonCache = StorageHelper.ByteToString(decryptedBytes);
                if (jsonCache != null)
                {
                    var contentModel = JsonConvert.DeserializeObject<ContentModel>(jsonCache);
                    res.Add(contentModel);
                }
            }
            return res;
        }

        public async Task<bool> GetContentModelForHistory(HistoryModel model)
        {
            try
            {
                var requestHelper = await GetRequestHelper();
                var response = await _dataService.ReadAsync(requestHelper.ContentEntityRequest(model.Parent.ApiInformations.ServerId, model.Parent.ApiInformations.ServerRelationId, model.VersionId));
                if (response.IsSuccessfull)
                {
                    var newModel = new ContentModel();
                    ResponseHelper.WriteIntoModel(response.ContentEntity, newModel);
                    newModel.ApiInformations = response.ApiInformations;
                    newModel.LocalStatus = LocalStatus.Immutable;
                    model.ContentModel = newModel;
                    return true;
                }
                _errorApiReportingService.ReportUnhandledApiError(response);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }

        public async Task<bool> Save(ContentModel model)
        {
            try
            {
                var maxReptitions = 60;
                while (model.RuntimeStatus != RuntimeStatus.Idle && model.RuntimeStatus != RuntimeStatus.SavingFailed && model.RuntimeStatus != RuntimeStatus.SyncingFailed)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    if (maxReptitions-- <= 0)
                    {
                        LogHelper.Instance.LogFatalError("cannot save contentmodel as it is used elsewhere", this);
                        return false;
                    }
                }

                var requestHelper = await GetRequestHelper();
                var syncHelper = new SyncHelper(_dataService, _errorApiReportingService, this);
                await syncHelper.UploadChangedWorker(new ConcurrentStack<ContentModel>(new List<ContentModel>() {model}), requestHelper);
                return model.RuntimeStatus == RuntimeStatus.Idle;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            LogHelper.Instance.LogFatalError("cannot save contentmodel!", this);
            return true;
        }

        public async Task<bool> SaveLocally(ContentModel model)
        {
            try
            {
                var json = JsonConvert.SerializeObject(model);
                var bytes = StorageHelper.StringToBytes(json);
                var encrytedBytes = await _passwordVaultService.EncryptAsync(bytes);
                if (await _folderStorageService.SaveFile(ContentFolder, model.Id.ToString(), encrytedBytes))
                    return true;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            LogHelper.Instance.LogFatalError("cannot save contentmodel!", this);
            return true;
        }

        private async Task<bool> DeleteAll()
        {
            var tr = true;
            var files = await _folderStorageService.GetFiles(ContentFolder);
            foreach (var file in files)
            {
                tr &= await _folderStorageService.DeleteFile(ContentFolder, file);
            }
            return tr;
        }
    }
}
