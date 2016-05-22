using System.Threading.Tasks;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IConfigurationService
    {
        Task<ApiConfiguration> GetApiConfiguration();

        Task<bool> SetApiConfiguration(ApiConfiguration config);
    }
}
