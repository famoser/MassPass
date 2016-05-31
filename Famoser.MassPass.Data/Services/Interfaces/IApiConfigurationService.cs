using System.Threading.Tasks;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IApiConfigurationService
    {
        Task<ApiConfiguration> GetApiConfigurationAsync();

        Task<bool> SetApiConfigurationAsync(ApiConfiguration config);
    }
}
