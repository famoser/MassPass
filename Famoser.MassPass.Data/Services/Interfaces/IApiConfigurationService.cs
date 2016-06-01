using System.Threading.Tasks;
using Famoser.MassPass.Data.Models;
using Famoser.MassPass.Data.Models.Storage;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    /// <summary>
    /// provides the application with the configuration of the api
    /// </summary>
    public interface IApiConfigurationService
    {
        Task<ApiConfiguration> GetApiConfigurationAsync();
        Task<bool> SetApiConfigurationAsync(ApiConfiguration config);
        Task<bool> SetApiConfigurationAsync(string config);
    }
}
