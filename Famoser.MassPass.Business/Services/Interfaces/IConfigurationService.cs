using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.Business.Services.Interfaces
{
    public interface IConfigurationService
    {
        Task<object> GetConfiguration(SettingKey key, object fallback);
        Task<bool> SetConfiguration(SettingKey key, object value);
    }
}
