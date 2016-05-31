using System;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Models;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Tests.Data.Mocks
{
    internal class ApiConfigurationServiceMock : IApiConfigurationService
    {
        private ApiConfiguration _config;
        private ApiConfiguration GetConfig()
        {
            if (_config == null)
                _config = new ApiConfiguration()
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
                };
            return _config;
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<ApiConfiguration> GetApiConfigurationAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return GetConfig();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> SetApiConfigurationAsync(ApiConfiguration config)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _config = config;
            return true;
        }
    }
}
