using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Helpers;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Business.Repositories
{
    public class BaseRepository
    {
        private readonly IApiConfigurationService _apiConfigurationService;

        public BaseRepository(IApiConfigurationService apiConfigurationService)
        {
            _apiConfigurationService = apiConfigurationService;
        }

        private static RequestHelper _requestHelper;
        protected async Task<RequestHelper> GetRequestHelper()
        {
            if (_requestHelper == null)
            {
                var userConfig = await _apiConfigurationService.GetUserConfigurationAsync();
                _requestHelper = new RequestHelper(userConfig);
            }
            return _requestHelper;
        }
    }
}
