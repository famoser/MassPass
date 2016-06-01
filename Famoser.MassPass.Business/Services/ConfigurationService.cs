using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;
using Famoser.FrameworkEssentials.Helpers;
using Famoser.FrameworkEssentials.Logging;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IStorageService _storageService;

        public ConfigurationService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        private ObservableCollection<ConfigurationModel> _models;
        public async Task<ObservableCollection<ConfigurationModel>> GetConfiguration()
        {
            try
            {
                if (_models == null)
                {
                    _models = new ObservableCollection<ConfigurationModel>();
                    var jsonAssets = await _storageService.GetAssetTextFileAsync(
                        ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.AssetConfiguration).Description);
                    var defaultSettings = JsonConvert.DeserializeObject<ObservableCollection<ConfigurationModel>>(jsonAssets);

                    var json = await _storageService.GetCachedTextFileAsync(
                        ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.Configuration).Description);

                    if (json == null)
                    {
                        foreach (var configurationModel in defaultSettings)
                        {
                            _models.Add(configurationModel);
                        }
                    }
                    else
                    {
                        var save = false;
                        var savedSettings = JsonConvert.DeserializeObject<ObservableCollection<ConfigurationModel>>(json);
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
                            else
                                save = true;
                        }
                        foreach (var configurationModel in defaultSettings)
                        {
                            _models.Add(configurationModel);
                        }
                        if (save)
                            await SaveConfiguration();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return _models;
        }

        public async Task<ConfigurationModel> GetConfiguration(SettingKeys key)
        {
            var config = await GetConfiguration();
            return config.FirstOrDefault(s => s.SettingKey == key);
        }

        public async Task<bool> SaveConfiguration()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_models);
                return await _storageService.SetCachedTextFileAsync(
                    ReflectionHelper.GetAttributeOfEnum<DescriptionAttribute, FileKeys>(FileKeys.Configuration).Description, json);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.LogException(ex);
            }
            return false;
        }
    }
}
