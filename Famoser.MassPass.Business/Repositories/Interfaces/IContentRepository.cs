using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Models.Content.Base;

namespace Famoser.MassPass.Business.Repositories.Interfaces
{
    public interface IContentRepository
    {
        ObservableCollection<GroupedCollectionModel> GetGroupedCollectionModels();

        Task<bool> SyncAsync();
        Task<bool> LoadValues(BaseContentModel model);
        Task<bool> FillHistory(BaseContentModel model);
        Task<bool> Save(BaseContentModel model);
    }
}
