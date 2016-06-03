using System;
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
using Famoser.MassPass.Common;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly IFolderStorageService _folderStorageService;
        private readonly IDataService _dataService;
        private readonly IConfigurationService _configurationService;
        private readonly IPasswordVaultService _passwordVaultService;
        private readonly IApiConfigurationService _apiConfigurationService;

        private const string ContentFolder = "content";

        public CollectionRepository(IDataService dataService, IConfigurationService configurationService, IPasswordVaultService passwordVaultService, IApiConfigurationService apiConfigurationService, IFolderStorageService folderStorageService)
        {
            _dataService = dataService;
            _configurationService = configurationService;
            _passwordVaultService = passwordVaultService;
            _apiConfigurationService = apiConfigurationService;
            _folderStorageService = folderStorageService;
        }


        private async Task FillCollection()
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
            var files = await _folderStorageService.GetFiles(ContentFolder);
            foreach (var file in files)
            {
                var content = await _folderStorageService.GetFile(ContentFolder, file);
                var decryptedBytes = await _passwordVaultService.DecryptAsync(content);
                var jsonCache = StorageHelper.ByteToString(decryptedBytes);
                if (jsonCache != null)
                {
                    var contentModel = JsonConvert.DeserializeObject<ContentModel>(jsonCache);
                    ContentManager.AddContent(contentModel);
                }
            }
        }


        private Task _fillCollectionsTask;
        private bool _initialisationStarted;
        public ObservableCollection<ContentModel> GetCollectionsAndLoad()
        {
            if (!_initialisationStarted)
            {
                _initialisationStarted = true;
                _fillCollectionsTask = FillCollection();
            }
            return ContentManager.ContentModelCollection;
        }

        public async Task<bool> SyncAsync()
        {
            if (_fillCollectionsTask != null)
            {
                await _fillCollectionsTask;
                _fillCollectionsTask = null;
            }
            var workerConfig = await _configurationService.GetConfiguration(SettingKeys.MaximumWorkerNumber);
            var userConfig = await _apiConfigurationService.GetUserConfigurationAsync();
            var requestHelper = new RequestHelper(userConfig);

            var res = await _dataService.GetAuthorizationStatusAsync(requestHelper.AuthorizationStatusRequest());
            if (res.IsSuccessfull && res.IsAuthorized)
            {
                /*
                 * Sync:
                 * 1. Refresh all changed ones online
                 * 2. Upload locally changed locally if possible
                 * 3. download missing from collections
                 * */



                // 2.
                var changedStack = new ConcurrentStack<ContentModel>(ContentManager.FlatContentModelCollection.Where(c => c.LocalStatus == LocalStatus.Changed));


                //refresh existing


                //upload changed


                //add new ones
                var relationStack = new FastThreadSafeStack<Guid>(userConfig.ReleationIds);
                var tasks = new List<Task>();
                for (int i = 0; i < workerConfig.IntValue && i < userConfig.ReleationIds.Count; i++)
                {
                    //tasks.Add(SyncRelationsWorker(relationStack, requestHelper));
                }
                await Task.WhenAll(tasks);
                tasks.Clear();

                //refresh content
                var items = ContentManager.FlatContentModelCollection.SelectMany(s => s.Contents.Where(c => c.LocalStatus == LocalStatus.Changed)).ToList();
                var contentStack = new FastThreadSafeStack<ContentModel>(items);
                for (int i = 0; i < workerConfig.IntValue && i < items.Count; i++)
                {
                    tasks.Add(SyncItemsWorker(contentStack, requestHelper));
                }
                await Task.WhenAll(tasks);

                //todo: save & cache etc
                return true;
            }
            return false;
        }

        //todo: collection instance where all ContentModels are "flat", no caring about parentId. create a contentmanager for this
        private async Task UploadChangedWorker(FastThreadSafeStack<Guid> stack, RequestHelper requestHelper)
        {
            try
            {
                var relationGuid = stack.Pop();
                var items = ContentManager.FlatContentModelCollection.SelectMany(s => s.Contents.Where(c => c.ApiInformations.ServerRelationId == relationGuid)).ToList();
                var newItems = await _dataService.ReadAsync(requestHelper.CollectionEntriesRequest(items.Select(s => s.ApiInformations.ServerId).ToList(), relationGuid));
                var entities = EntityConversionHelper.GetRefreshEntities(items);
                var refreshRes = await _dataService.RefreshAsync(requestHelper.RefreshRequest(entities));
                if (refreshRes.IsSuccessfull)
                {
                    foreach (var refreshEntity in refreshRes.RefreshEntities)
                    {
                        //refreshEntity.ApiStatus =
                    }
                    //todo: do stuff
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
        }

        //todo: collection instance where all ContentModels are "flat", no caring about parentId. create a contentmanager for this
        private async Task ReadCollectionWorker(FastThreadSafeStack<Guid> stack, RequestHelper requestHelper)
        {
            try
            {
                var relationGuid = stack.Pop();
                var items = ContentManager.FlatContentModelCollection.SelectMany(
                        s => s.Contents.Where(c => c.ApiInformations.ServerRelationId == relationGuid)).ToList();
                var newItems = await _dataService.ReadAsync(requestHelper.CollectionEntriesRequest(items.Select(s => s.ApiInformations.ServerId).ToList(), relationGuid));
                

            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
        }

        private async Task SyncItemsWorker(FastThreadSafeStack<ContentModel> models, RequestHelper requestHelper)
        {
            //todo: do work
        }
    }
}
