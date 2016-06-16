using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Managers;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Data.Entities.Communications.Request.Entities;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Business.Helpers
{
    public class SyncHelper
    {
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
    }
}
