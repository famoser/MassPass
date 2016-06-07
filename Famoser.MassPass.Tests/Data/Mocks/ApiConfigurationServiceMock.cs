using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Models.Storage;
using Famoser.MassPass.Data.Services.Interfaces;
using Newtonsoft.Json;

namespace Famoser.MassPass.Tests.Data.Mocks
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    internal class ApiConfigurationServiceMock : IApiConfigurationService
    {
        private ApiConfiguration _config;
        private ApiConfiguration GetConfig()
        {
            return _config ?? (_config = new ApiConfiguration()
            {
                GenerationKeyInterations = 5000,
                Uri = new Uri("https://api.masspass.famoser.ch/tests"),
                InitialisationVector =
                    new byte[]
                    {
                        2, 1, 42, 14, 1, 2, 12, 4, 51, 21, 12, 3, 12, 3, 14, 12
                    },
                GenerationKeyLenghtInBytes = 32,
                GenerationSalt =
                    new byte[]
                    {
                        2, 1, 42, 14, 1, 2, 12, 4, 51, 24, 25, 245, 3, 25, 24, 65, 25, 45, 23, 54, 235, 235, 23, 24
                    }
            });
        }

        public async Task<ApiConfiguration> GetApiConfigurationAsync()
        {
            return GetConfig();
        }
        
        public async Task<bool> SetApiConfigurationAsync(ApiConfiguration config)
        {
            _config = config;
            return true;
        }

        public async Task<bool> SetApiConfigurationAsync(string config)
        {
            _config = JsonConvert.DeserializeObject<ApiConfiguration>(config);
            return true;
        }

        private UserConfiguration _userConfig;
        public async Task<UserConfiguration> GetUserConfigurationAsync()
        {
            return _userConfig ?? (_userConfig = new UserConfiguration()
            {
                UserId = Guid.NewGuid(),
                DeviceId = Guid.NewGuid(),
                ReleationIds = new List<Guid>()
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid()
                }
            });
        }

        public async Task<bool> SetUserConfigurationAsync(UserConfiguration config)
        {
            _userConfig = config;
            return true;
        }

        public async Task<bool> SetUserConfigurationAsync(string config)
        {
            _userConfig = JsonConvert.DeserializeObject<UserConfiguration>(config);
            return true;
        }
    }
}
