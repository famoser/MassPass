﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.MassPass.Business.Enums;
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
    public class CollectionRepository : BaseRepository, ICollectionRepository
    {
        private readonly IFolderStorageService _folderStorageService;
        private readonly IDataService _dataService;
        private readonly IConfigurationService _configurationService;
        private readonly IPasswordVaultService _passwordVaultService;
        private readonly IApiConfigurationService _apiConfigurationService;
        private readonly IErrorApiReportingService _errorApiReportingService;
        private readonly IContentRepository _contentRepository;

        public CollectionRepository(IDataService dataService, IConfigurationService configurationService, IPasswordVaultService passwordVaultService, IApiConfigurationService apiConfigurationService, IFolderStorageService folderStorageService, IErrorApiReportingService errorApiReportingService, IContentRepository contentRepository) : base(apiConfigurationService)
        {
            _dataService = dataService;
            _configurationService = configurationService;
            _passwordVaultService = passwordVaultService;
            _apiConfigurationService = apiConfigurationService;
            _folderStorageService = folderStorageService;
            _errorApiReportingService = errorApiReportingService;
            _contentRepository = contentRepository;
        }
        
        private async Task FillCollection()
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
                var contentModels = await _contentRepository.ReadOutAll();
                foreach (var contentModel in contentModels)
                {
                    ContentManager.AddOrReplaceContent(contentModel, true);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
        }

        private async Task SaveCollection()
        {
            try
            {
                var cacheConfig = await _configurationService.GetConfiguration(SettingKeys.EnableCachingOfCollectionNames);
                if (cacheConfig.BoolValue)
                {
                    var cacheModel = ContentManager.CreateCacheModel();
                    var str = JsonConvert.SerializeObject(cacheModel);
                    var bytes = StorageHelper.StringToBytes(str);
                    var encryptedBytes = await _passwordVaultService.EncryptAsync(bytes);
                    await _folderStorageService.SetCachedFileAsync(ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.ApiConfiguration).Description, encryptedBytes);
                }

                await _contentRepository.SaveAll();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
        }


        private Task _fillCollectionsTask;
        private bool _initialisationStarted;
        public async Task<bool> InitializeVault(string masterPassword)
        {
            try
            {
                await _passwordVaultService.CreateNewVault(masterPassword);
                var serverRelationGuid = Guid.NewGuid();
                var parentGuid = Guid.NewGuid();
                ContentManager.AddOrReplaceContent(new ContentModel()
                {
                    ContentJson = @"{'Content': 'This is a note!'}",
                    Name = "Example Note",
                    Id = parentGuid,
                    ApiInformations = new ApiInformations()
                    {
                        ServerRelationId = serverRelationGuid
                    }
                });
                ContentManager.AddOrReplaceContent(new ContentModel()
                {
                    ContentJson = @"{'Content': 'This is a note in a note!'}",
                    Name = "Example Note",
                    Id = Guid.NewGuid(),
                    ParentId = parentGuid,
                    ApiInformations = new ApiInformations()
                    {
                        ServerRelationId = serverRelationGuid
                    }
                });

                await _contentRepository.DeleteAll();

                await _passwordVaultService.RegisterPasswordAsync(serverRelationGuid, Guid.NewGuid().ToString());
                await SaveCollection();
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
                return false;
            }
            return true;
        }

        public ObservableCollection<ContentModel> GetCollectionsAndLoad()
        {
            lock (this)
            {
                if (!_initialisationStarted)
                {
                    _initialisationStarted = true;
                    _fillCollectionsTask = FillCollection();
                }
            }
            return ContentManager.ContentModelCollection;
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

                    // 1. check if version online is same, if yes, prepare for upload
                    var locallyChangedStack = await SyncHelper.GetLocallyChangedStack(_dataService, requestHelper);
                    var tasks = new List<Task>();
                    for (int i = 0; i < workerConfig.IntValue && i < userConfig.RelationIds.Count; i++)
                    {
                        tasks.Add(UploadChangedWorker(locallyChangedStack, requestHelper));
                    }
                    await Task.WhenAll(tasks);
                    tasks.Clear();

                    // 2. refresh all changed ones
                    var remotelyChangedStack = await SyncHelper.GetRemotelyChangedStack(_dataService, requestHelper);
                    for (int i = 0; i < workerConfig.IntValue && i < userConfig.RelationIds.Count; i++)
                    {
                        tasks.Add(DownloadChangedWorker(remotelyChangedStack, requestHelper));
                    }
                    await Task.WhenAll(tasks);
                    tasks.Clear();

                    // 3. collect missing
                    var collectionStack =
                        new ConcurrentStack<Guid>(
                            ContentManager.FlatContentModelCollection.Select(c => c.ApiInformations.ServerRelationId)
                                .Distinct());
                    var missingStack = new ConcurrentStack<CollectionEntryEntity>();
                    for (int i = 0; i < workerConfig.IntValue && i < userConfig.RelationIds.Count; i++)
                    {
                        tasks.Add(ReadCollectionWorker(collectionStack, requestHelper, missingStack));
                    }
                    await Task.WhenAll(tasks);
                    tasks.Clear();

                    // 4. download missing
                    for (int i = 0; i < workerConfig.IntValue && i < userConfig.RelationIds.Count; i++)
                    {
                        tasks.Add(DownloadMissingWorker(missingStack, requestHelper));
                    }
                    await Task.WhenAll(tasks);
                    tasks.Clear();

                    await SaveCollection();
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

        public ObservableCollection<ContentModel> GetSampleCollections()
        {
            var obs = new ObservableCollection<ContentModel>();
            for (int i = 0; i < 5; i++)
            {
                obs.Add(new ContentModel()
                {
                    Name = "Titel" + i
                });
            }
            return obs;
        }

        private async Task UploadChangedWorker(ConcurrentStack<ContentModel> stack, RequestHelper requestHelper)
        {
            ContentModel changedContent;
            if (stack.TryPop(out changedContent))
            {
                try
                {
                    changedContent.SaveDisabled = true;
                    var req = await _dataService.UpdateAsync(requestHelper.UpdateRequest(
                        changedContent.ApiInformations.ServerId,
                        changedContent.ApiInformations.ServerRelationId,
                        changedContent.ApiInformations.VersionId,
                        changedContent));

                    if (req.IsSuccessfull)
                    {
                        changedContent.ApiInformations.VersionId = req.VersionId;
                        changedContent.ApiInformations.ServerId = req.ServerId;
                        changedContent.ApiInformations.ServerRelationId = req.ServerRelationId;
                        changedContent.LocalStatus = LocalStatus.UpToDate;
                    }
                    else
                    {
                        if (req.ApiError == ApiError.InvalidVersionId)
                            changedContent.LocalStatus = LocalStatus.Conflict;

                        _errorApiReportingService.ReportUnhandledApiError(req, changedContent);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.LogException(ex);
                }
                finally
                {
                    changedContent.SaveDisabled = false;
                }
            }
        }

        private async Task DownloadChangedWorker(ConcurrentStack<ContentModel> stack, RequestHelper requestHelper)
        {
            ContentModel changedContent;
            if (stack.TryPop(out changedContent))
            {
                try
                {
                    changedContent.SaveDisabled = true;
                    var req = await _dataService.ReadAsync(
                        requestHelper.ContentEntityRequest(
                            changedContent.ApiInformations.ServerId,
                            changedContent.ApiInformations.ServerRelationId,
                            changedContent.ApiInformations.VersionId));
                    if (req.IsSuccessfull)
                    {
                        ResponseHelper.WriteIntoModel(req.ContentEntity, changedContent);
                        changedContent.ApiInformations = req.ApiInformations;
                        changedContent.LocalStatus = LocalStatus.UpToDate;
                    }
                    else
                    {
                        _errorApiReportingService.ReportUnhandledApiError(req, changedContent);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.LogException(ex);
                }
                finally
                {
                    changedContent.SaveDisabled = false;
                }
            }
        }

        private async Task ReadCollectionWorker(ConcurrentStack<Guid> stack, RequestHelper requestHelper, ConcurrentStack<CollectionEntryEntity> missingStack)
        {
            Guid relationGuid;
            if (stack.TryPop(out relationGuid))
            {
                try
                {
                    var items = ContentManager.FlatContentModelCollection.SelectMany(
                            s => s.Contents.Where(c => c.ApiInformations.ServerRelationId == relationGuid)).ToList();
                    var newItems = await _dataService.ReadAsync(requestHelper.CollectionEntriesRequest(items.Select(s => s.ApiInformations.ServerId).ToList(), relationGuid));
                    if (newItems.IsSuccessfull)
                    {
                        foreach (var item in newItems.CollectionEntryEntities)
                        {
                            missingStack.Push(item);
                        }
                    }
                    else
                    {
                        _errorApiReportingService.ReportUnhandledApiError(newItems);
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.Instance.LogException(ex);
                }
            }
        }

        private async Task DownloadMissingWorker(ConcurrentStack<CollectionEntryEntity> stack, RequestHelper requestHelper)
        {
            CollectionEntryEntity entry;
            if (stack.TryPop(out entry))
            {
                try
                {
                    var response = await _dataService.ReadAsync(requestHelper.ContentEntityRequest(entry.ServerId, entry.RelationId, entry.VersionId));
                    if (response.IsSuccessfull)
                    {
                        var newModel = new ContentModel();
                        ResponseHelper.WriteIntoModel(response.ContentEntity, newModel);
                        newModel.ApiInformations = response.ApiInformations;
                        newModel.LocalStatus = LocalStatus.UpToDate;
                        ContentManager.AddOrReplaceContent(newModel);
                    }
                    else
                    {
                        _errorApiReportingService.ReportUnhandledApiError(response);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Instance.LogException(ex);
                }
            }
        }
    }
}
