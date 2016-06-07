﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.MassPass.Business.Extensions;
using Famoser.MassPass.Business.Helpers;
using Famoser.MassPass.Business.Managers;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Enum;
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

        public ContentRepository(IFolderStorageService folderStorageService, IPasswordVaultService passwordVaultService, IDataService dataService, IApiConfigurationService apiConfigurationService, IErrorApiReportingService errorApiReportingService, IDevicesRepository devicesRepository) : base(apiConfigurationService)
        {
            _folderStorageService = folderStorageService;
            _passwordVaultService = passwordVaultService;
            _dataService = dataService;
            _errorApiReportingService = errorApiReportingService;
            _devicesRepository = devicesRepository;
        }

        public async Task<bool> FillValues(ContentModel model)
        {
            var content = await _folderStorageService.GetFile(ContentFolder, model.Id.ToString());
            var decryptedBytes = await _passwordVaultService.DecryptAsync(content);
            var jsonCache = StorageHelper.ByteToString(decryptedBytes);
            if (jsonCache != null)
            {
                var contentModel = JsonConvert.DeserializeObject<ContentModel>(jsonCache);
                CacheHelper.WriteAllValues(model, contentModel);
            }
            return true;
        }

        public async Task<bool> FillHistory(ContentModel model)
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
            else
            {
                _errorApiReportingService.ReportUnhandledApiError(resp, model);
                return false;
            }
        }

        public async Task<List<ContentModel>> ReadOutAll()
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
                    newModel.LocalStatus = LocalStatus.History;
                    model.ContentModel = newModel;
                    return true;
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
            return false;
        }

        public async Task<bool> SaveAll()
        {
            var toBeSaved = ContentManager.UnsavedModels();
            foreach (var contentModel in toBeSaved)
            {
                var json = JsonConvert.SerializeObject(contentModel);
                var bytes = StorageHelper.StringToBytes(json);
                var encrytedBytes = await _passwordVaultService.EncryptAsync(bytes);
                if (!await _folderStorageService.SaveFile(ContentFolder, contentModel.Id.ToString(), encrytedBytes))
                    LogHelper.Instance.LogFatalError("cannot save contentmodel!", this);
            }
            return true;
        }
    }
}
