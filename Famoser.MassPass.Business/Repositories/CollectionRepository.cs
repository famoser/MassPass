﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Helpers;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Models.Storage;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Common;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        private IStorageService _storageService;
        private IDataService _dataService;
        private IConfigurationService _configurationService;
        private IPasswordVaultService _passwordVaultService;
        private IApiConfigurationService _apiConfigurationService;

        public CollectionRepository(IStorageService storageService, IDataService dataService, IConfigurationService configurationService, IPasswordVaultService passwordVaultService, IApiConfigurationService apiConfigurationService)
        {
            _storageService = storageService;
            _dataService = dataService;
            _configurationService = configurationService;
            _passwordVaultService = passwordVaultService;
            _apiConfigurationService = apiConfigurationService;
        }

        private ObservableCollection<ContentModel> _collections;

        private async void FillCollection()
        {
            var cacheConfig = await _configurationService.GetConfiguration(SettingKeys.EnableCachingOfCollectionNames);
            if (cacheConfig.BoolValue)
            {
                //read from cache
                var byteCache = await _storageService.GetCachedFileAsync(
                            ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(
                                FileKeys.ApiConfiguration).Description);
                var decryptedBytes = await _passwordVaultService.DecryptAsync(byteCache);
                var jsonCache = StorageHelper.ByteToString(decryptedBytes);
               
                if (jsonCache != null)
                {
                    var cacheModel = JsonConvert.DeserializeObject<CollectionCacheModel>(jsonCache);
                    ConversionHelper.FillCollectionFromCache(_collections, cacheModel);
                    return;
                }
            }
            //todo: storageFolderService: Fill cache with all items from folder in cache
        }

        public ObservableCollection<ContentModel> GetCollectionsAndLoad()
        {
            lock (this)
            {
                if (_collections == null)
                {
                    _collections = new ObservableCollection<ContentModel>();
                    FillCollection();
                }
                return _collections;
            }
        }

        public async Task<bool> SyncAsync()
        {
            var workerConfig = await _configurationService.GetConfiguration(SettingKeys.MaximumWorkerNumber);
            var userConfig = await _apiConfigurationService.GetUserConfigurationAsync();

            //refresh relations
            var relationStack = new FastThreadSafeStack<Guid>(userConfig.ReleationIds);
            var tasks = new List<Task>();
            for (int i = 0; i < workerConfig.IntValue && i < userConfig.ReleationIds.Count; i++)
            {
                tasks.Add(SyncRelationsWorker(relationStack));
            }
            await Task.WhenAll(tasks);
            tasks.Clear();

            //refresh content
            var items = _collections.SelectMany(s => s.Contents).ToList();
            var contentStack = new FastThreadSafeStack<ContentModel>(items);
            for (int i = 0; i < workerConfig.IntValue && i < items.Count; i++)
            {
                tasks.Add(SyncItemsWorker(contentStack));
            }
            await Task.WhenAll(tasks);

            //todo: save & cache etc
            return true;
        }

        private async Task SyncRelationsWorker(FastThreadSafeStack<Guid> stack)
        {
            //todo: do work
        }

        private async Task SyncItemsWorker(FastThreadSafeStack<ContentModel> models)
        {
            //todo: do work
        }
    }
}
