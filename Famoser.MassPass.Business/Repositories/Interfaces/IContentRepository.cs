using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.Business.Repositories.Interfaces
{
    public interface IContentRepository
    {
        Task<ObservableCollection<CollectionModel>> GetCollectionsAsync();
        Task<bool> SyncAsync();
    }
}
