using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Services.Interfaces;
using Nito.AsyncEx;

namespace Famoser.MassPass.Business.Repositories
{
    public class DevicesRepository : BaseRepository, IDevicesRepository
    {
        private readonly IErrorApiReportingService _errorApiReportingService;
        private IAuthorizationRepository _authorizationRepository;
        private IApiConfigurationService _apiConfigurationService;

        public DevicesRepository(IErrorApiReportingService errorApiReportingService, IApiConfigurationService apiConfigurationService, IAuthorizationRepository authorizationRepository)
        {
            _errorApiReportingService = errorApiReportingService;
            _apiConfigurationService = apiConfigurationService;
            _authorizationRepository = authorizationRepository;
        }

        private readonly ObservableCollection<DeviceModel> _device = new ObservableCollection<DeviceModel>();
        private bool _hasLoadedDevices;
        private readonly AsyncLock _asyncLock = new AsyncLock();
        public async Task<ObservableCollection<DeviceModel>> GetDevices(bool forceRefresh = false)
        {
            using (await _asyncLock.LockAsync())
            {
                if (!_hasLoadedDevices)
                {
                    _hasLoadedDevices = true;

                    var client = await _authorizationRepository.GetAuthorizedApiClientAsync();
                    var resp = await client.GetAuthorizedDevicesAsync(new AuthorizedDevicesRequest());
                    if (resp.IsSuccessfull)
                    {
                        foreach (var authorizedDeviceEntity in resp.AuthorizedDeviceEntities)
                        {
                            var exitingDevice =
                                _device.FirstOrDefault(d => d.DeviceId == authorizedDeviceEntity.DeviceId);
                            if (exitingDevice == null)
                            {
                                var newDevice = new DeviceModel();
                                _device.Add(newDevice);
                                exitingDevice = newDevice;
                            }

                            exitingDevice.DeviceId = authorizedDeviceEntity.DeviceId;
                            exitingDevice.DeviceName = authorizedDeviceEntity.DeviceName;
                            exitingDevice.AuthorizationDateTime = authorizedDeviceEntity.AuthorizationDateTime;
                            exitingDevice.LastModificationDateTime = authorizedDeviceEntity.LastModificationDateTime;
                            exitingDevice.LastRequestDateTime = authorizedDeviceEntity.LastRequestDateTime;
                        }
                    }
                    else
                    {
                        _errorApiReportingService.ReportUnhandledApiError(resp);
                    }
                }
            }
            return _device;
        }
    }
}
