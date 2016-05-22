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
        public async Task<ApiConfiguration> GetApiConfiguration()
        {
            return new ApiConfiguration()
            {
                GenerationKeyInterations = 5000,
                Uri = new Uri("https://	api.masspass.famoser.ch"),
                InitialisationVector =
                    new byte[]
                    {
                        2, 1, 42, 14, 1, 2, 12, 4, 51, 21, 12, 3, 12, 3, 14, 12, 12, 13, 23, 124, 141, 31, 24, 25, 245, 3,
                        25, 24, 65, 25, 45, 23, 54, 235, 235, 23, 24
                    },
                GenerationKeyLenghtInBytes = 30,
                GenerationSalt =
                    new byte[]
                    {
                        2, 1, 42, 14, 1, 2, 12, 4, 51, 24, 25, 245, 3, 25, 24, 65, 25, 45, 23, 54, 235, 235, 23, 24
                    }
            };
        }
    }
}
