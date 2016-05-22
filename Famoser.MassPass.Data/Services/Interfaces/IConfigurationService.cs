using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Data.Services.Interfaces
{
    public interface IConfigurationService
    {
        Task<ApiConfiguration> GetApiConfiguration();
    }
}
