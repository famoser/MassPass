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
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        private IStorageService _storageService;
        private IDataService _dataService;
        private IConfigurationService _configurationService;

        public CollectionRepository(IStorageService storageService, IDataService dataService, IConfigurationService configurationService)
        {
            _storageService = storageService;
            _dataService = dataService;
            _configurationService = configurationService;
        }

        private ObservableCollection<ContentModel> _collections;

        private async void FillCollection()
        {
            var cacheConfig = await _configurationService.GetConfiguration(SettingKeys.EnableCachingOfCollectionNames);
            if (cacheConfig.BoolValue)
            {
                //todo; encrypt cache
                //read from cache
                var jsonCache =
                    await
                        _storageService.GetCachedTextFileAsync(
                            ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(
                                FileKeys.ApiConfiguration).Description);
                if (jsonCache != null)
                {
                    var cacheModel = JsonConvert.DeserializeObject<CollectionCacheModel>(jsonCache);
                    ConversionHelper.FillCollectionFromCache(_collections, cacheModel);
                }
            }
        }

        public ObservableCollection<CollectionModel> GetCollectionsAndLoad()
        {
            lock (this)
            {
                if (_collections == null)
                {
                    _collections = new ObservableCollection<CollectionModel>();
                    FillCollection();
                }
                return _collections;
            }
        }

        public Task<bool> SyncAsync()
        {
            throw new NotImplementedException();
        }
    }
}
