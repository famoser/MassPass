using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.Business.Repositories.Interfaces
{
    public interface IContentRepository
    {
        Task<bool> InitializeVault(string masterPassword);

        ContentModel GetRootModelAndLoad();
        ContentModel GetSampleModel(ContentTypes type);

        Task<bool> SyncAsync();
        Task<bool> FillValues(ContentModel model);
        Task<bool> FillHistory(ContentModel model);
        Task<bool> GetContentModelForHistory(HistoryModel model);
        Task<bool> Save(ContentModel model);
        Task<bool> SaveLocally(ContentModel model);
    }
}
