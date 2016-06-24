using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Services.Interfaces;
using Newtonsoft.Json;
using Nito.AsyncEx;

namespace Famoser.MassPass.Business.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IFolderStorageService _folderStorageService;

        public ConfigurationService(IFolderStorageService folderStorageService)
        {
            _folderStorageService = folderStorageService;
        }

        private ObservableCollection<ConfigurationModel> _models = new ObservableCollection<ConfigurationModel>();
        private bool _initializedConfiguration;
        private readonly AsyncLock _initializedConfigurationLock = new AsyncLock();
        public async Task<ObservableCollection<ConfigurationModel>> GetConfiguration()
        {
            using (await _initializedConfigurationLock.LockAsync())
            {
                if (!_initializedConfiguration)
                {
                    _models = new ObservableCollection<ConfigurationModel>();
                    var jsonAssets = await _folderStorageService.GetAssetTextFileAsync(
                        ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.AssetConfiguration)
                            .Description);
                    var defaultSettings =
                        JsonConvert.DeserializeObject<ObservableCollection<ConfigurationModel>>(jsonAssets);

                    try
                    {
                        var json = await _folderStorageService.GetCachedTextFileAsync(
                            ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.Configuration)
                                .Description);

                        if (!string.IsNullOrEmpty(json))
                        {
                            var savedSettings =
                                JsonConvert.DeserializeObject<ObservableCollection<ConfigurationModel>>(json);
                            foreach (var configurationModel in savedSettings)
                            {
                                var def = defaultSettings.FirstOrDefault(s => s.Guid == configurationModel.Guid);
                                if (def != null)
                                {
                                    configurationModel.SettingKey = def.SettingKey;
                                    if (configurationModel.Immutable != def.Immutable)
                                    {
                                        configurationModel.Immutable = def.Immutable;
                                        configurationModel.Value = def.Value;
                                    }
                                    configurationModel.Name = def.Name;

                                    _models.Add(configurationModel);
                                    defaultSettings.Remove(def);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Instance.LogException(ex);
                    }
                    foreach (var configurationModel in defaultSettings)
                    {
                        _models.Add(configurationModel);
                    }

                    _initializedConfiguration = true;
                }

                SaveConfiguration();
                return _models;
            }
        }

        public async Task<ConfigurationModel> GetConfiguration(SettingKeys key)
        {
            var config = await GetConfiguration();
            return config.FirstOrDefault(s => s.SettingKey == key);
        }

        public async Task<bool> SaveConfiguration()
        {
            var json = JsonConvert.SerializeObject(_models);
            return await _folderStorageService.SetCachedTextFileAsync(
                ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.Configuration).Description, json);
        }
    }
}
