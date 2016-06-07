using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.Business.Repositories.Interfaces
{
    public interface IDevicesRepository
    {
        Task<ObservableCollection<DeviceModel>> GetDevices(bool forceRefresh = false);
    }
}
