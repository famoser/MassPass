using System;
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
            //todo: storageFolderService
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
            //var relationIds =
            var userConfig = await _apiConfigurationService.GetUserConfigurationAsync();
            var relationStack = new ThreadSafeStack<Guid>();
            foreach (var relationId in userConfig.ReleationIds)
            {
                await relationStack.Push(relationId);
            }
            
            return true;
        }

        private async Task SyncRelationsWorker()
        {
            
        }
    }
}
