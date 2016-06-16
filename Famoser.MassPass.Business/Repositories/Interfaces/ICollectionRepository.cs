using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.Business.Repositories.Interfaces
{
    public interface ICollectionRepository
    {
        Task<bool> InitializeVault(string masterPassword);
        ObservableCollection<ContentModel> GetCollectionsAndLoad();
        Task<bool> SyncAsync();
    }
}
