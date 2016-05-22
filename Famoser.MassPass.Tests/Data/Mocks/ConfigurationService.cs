using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Models;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Tests.Data.Mocks
{
    class ConfigurationServiceMock : IConfigurationService
    {
        private ApiConfiguration _config;
        private ApiConfiguration GetConfig()
        {
            if (_config == null)
                _config = new ApiConfiguration()
                {
                    GenerationKeyInterations = 5000,
                    Uri = new Uri("https://api.masspass.famoser.ch"),
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
        public async Task<ApiConfiguration> GetApiConfiguration()
        {
            return GetConfig();
        }

        public async Task<bool> SetApiConfiguration(ApiConfiguration config)
        {
            _config = config;
            return true;
        }
    }
}
