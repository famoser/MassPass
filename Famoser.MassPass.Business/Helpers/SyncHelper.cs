using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Logging;
using Famoser.MassPass.Business.Managers;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Entities.Communications.Response.Entitites;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Services.Interfaces;
using RefreshEntity = Famoser.MassPass.Data.Entities.Communications.Request.Entities.RefreshEntity;

namespace Famoser.MassPass.Business.Helpers
{
    public class SyncHelper
    {
        private readonly IDataService _dataService;
        private readonly IErrorApiReportingService _errorApiReportingService;
        private readonly IContentRepository _contentRepository;

        public SyncHelper(IDataService dataService, IErrorApiReportingService errorApiReportingService, IContentRepository contentRepository)
        {
            _dataService = dataService;
            _errorApiReportingService = errorApiReportingService;
            _contentRepository = contentRepository;
        }

        public static async Task<ConcurrentStack<ContentModel>> GetLocallyChangedStack(IDataService dataService, RequestHelper requestHelper)
        {
            var changed = ContentManager.FlatContentModelCollection.Where(c => c.LocalStatus == LocalStatus.Changed || c.LocalStatus == LocalStatus.New).ToList();
            if (changed.Count < 2)
            {
                //shortcut because I'm awesome
                return new ConcurrentStack<ContentModel>(changed);
            }
            var changedStack = new ConcurrentStack<ContentModel>();
            var versions = changed.Select(s => new RefreshEntity()
            {
                ServerId = s.ApiInformations.ServerId,
                VersionId = s.ApiInformations.VersionId
            }).ToList();
            var onlineVersion = await dataService.RefreshAsync(requestHelper.RefreshRequest(versions));
            foreach (var contentModel in changed)
            {
                var refreshEntity =
                    onlineVersion.RefreshEntities.FirstOrDefault(
                        e => e.ServerId == contentModel.ApiInformations.ServerId);
                if (refreshEntity == null || refreshEntity.ApiStatus != ApiStatus.Changed)
                    changedStack.Push(contentModel);
                else
                    contentModel.LocalStatus = LocalStatus.Conflict;
            }
            return changedStack;
        }

        public static async Task<ConcurrentStack<ContentModel>> GetRemotelyChangedStack(IDataService dataService, RequestHelper requestHelper)
        {
            var changed = ContentManager.FlatContentModelCollection.Where(c => c.LocalStatus == LocalStatus.UpToDate).ToList();
            var changedStack = new ConcurrentStack<ContentModel>();
            var versions = changed.Select(s => new RefreshEntity()
            {
                ServerId = s.ApiInformations.ServerId,
                VersionId = s.ApiInformations.VersionId
            }).ToList();
            var onlineVersion = await dataService.RefreshAsync(requestHelper.RefreshRequest(versions));
            foreach (var contentModel in changed)
            {
                var refreshEntity =
                    onlineVersion.RefreshEntities.FirstOrDefault(
                        e => e.ServerId == contentModel.ApiInformations.ServerId);
                if (refreshEntity == null || refreshEntity.ApiStatus != ApiStatus.Changed)
                    changedStack.Push(contentModel);
            }
            return changedStack;
        }

        
        public async Task UploadChangedWorker(ConcurrentStack<ContentModel> stack, RequestHelper requestHelper)
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

        public async Task DownloadChangedWorker(ConcurrentStack<ContentModel> stack, RequestHelper requestHelper)
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

        public async Task ReadCollectionWorker(ConcurrentStack<Guid> stack, RequestHelper requestHelper, ConcurrentStack<CollectionEntryEntity> missingStack)
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

        public async Task DownloadMissingWorker(ConcurrentStack<CollectionEntryEntity> stack, RequestHelper requestHelper)
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
