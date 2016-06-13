using System;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Models.Storage;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    /// <summary>
    /// provides the application with the configuration of the api
    /// </summary>
    public interface IApiConfigurationService
    {
        Task<bool> IsConfigurationReady();

        Task<ApiConfiguration> GetApiConfigurationAsync();
        Task<bool> SetApiConfigurationAsync(ApiConfiguration config);
        bool CanSetApiConfigurationAsync(string config);
        Task<bool> TrySetApiConfigurationAsync(string config);

        Task<UserConfiguration> GetUserConfigurationAsync();
        Task<bool> SetUserConfigurationAsync(UserConfiguration config, Guid deviceGuid);
        bool CanSetUserConfigurationAsync(string config);
        Task<bool> TrySetUserConfigurationAsync(string config, Guid deviceGuid);
    }
}
