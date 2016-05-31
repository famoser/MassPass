using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.Business.Services.Interfaces
{
    public interface IConfigurationService
    {
        Task<ObservableCollection<ConfigurationModel>> GetConfiguration();
        Task<ConfigurationModel> GetConfiguration(SettingKeys key);
        Task<bool> SaveConfiguration();
    }
}
