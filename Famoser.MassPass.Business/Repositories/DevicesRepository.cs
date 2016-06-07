using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Helpers;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;

namespace Famoser.MassPass.Business.Repositories
{
    public class DevicesRepository : BaseRepository, IDevicesRepository
    {
        private readonly IDataService _dataService;
        private readonly IErrorApiReportingService _errorApiReportingService;
        private IApiConfigurationService _apiConfigurationService;

        public DevicesRepository(IDataService dataService, IErrorApiReportingService errorApiReportingService, IApiConfigurationService apiConfigurationService) : base(apiConfigurationService)
        {
            _dataService = dataService;
            _errorApiReportingService = errorApiReportingService;
            _apiConfigurationService = apiConfigurationService;
        }

        private readonly ObservableCollection<DeviceModel> _device = new ObservableCollection<DeviceModel>();
        private bool _hasLoadedDevices;
        public async Task<ObservableCollection<DeviceModel>> GetDevices(bool forceRefresh = false)
        {
            if (!_hasLoadedDevices)
            {
                _hasLoadedDevices = true;

                var resp = await _dataService.GetAuthorizedDevicesAsync((await GetRequestHelper()).AuthorizedDevicesRequest());
                if (resp.IsSuccessfull)
                {
                    foreach (var authorizedDeviceEntity in resp.AuthorizedDeviceEntities)
                    {
                        var exitingDevice = _device.FirstOrDefault(d => d.DeviceId == authorizedDeviceEntity.DeviceId);
                        if (exitingDevice == null)
                        {
                            var newDevice = new DeviceModel();
                            _device.Add(newDevice);
                            exitingDevice = newDevice;
                        }
                        EntityConversionHelper.WriteValues(authorizedDeviceEntity, exitingDevice);
                    }
                }
                else
                {
                    _errorApiReportingService.ReportUnhandledApiError(resp);
                }
            }
            return _device;
        }
    }
}
