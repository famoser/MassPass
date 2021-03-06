﻿using System.Threading.Tasks;
using Famoser.MassPass.Data.Models.Configuration;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    /// <summary>
    /// provides the application with the configuration of the api
    /// </summary>
    public interface IApiConfigurationService
    {
        Task<ApiConfiguration> GetApiConfigurationAsync();
        Task<bool> SetApiConfigurationAsync(ApiConfiguration config);

        Task<UserConfiguration> GetUserConfigurationAsync();
        Task<bool> SetUserConfigurationAsync(UserConfiguration config);
        Task<bool> IsConfigurationReady();
    }
}
