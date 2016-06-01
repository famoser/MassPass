using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.Business.Repositories.Interfaces
{
    public interface IDevicesRepository
    {
        Task<ObservableCollection<DeviceModel>> GetDevices();
    }
}
