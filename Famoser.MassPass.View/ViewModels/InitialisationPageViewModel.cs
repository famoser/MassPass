using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Models;
using Famoser.MassPass.Data.Models.Configuration;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Famoser.MassPass.View.ViewModels
{
    public class InitialisationPageViewModel : ViewModelBase
    {
        private readonly IApiConfigurationService _apiConfigurationService;

        public InitialisationPageViewModel(IApiConfigurationService apiConfigurationService)
        {
            _apiConfigurationService = apiConfigurationService;

            _setConfigurationCommand = new LoadingRelayCommand(SetConfigurationAsync);
        }

        internal async Task LoadExistigConfigurationAsync()
        {
            try
            {
                var newApiConfig = await _apiConfigurationService.GetApiConfigurationAsync();
                ApiConfiguration = newApiConfig ?? new ApiConfiguration();

                var newUserConfig = await _apiConfigurationService.GetUserConfigurationAsync();
                UserConfiguration = newUserConfig ?? new UserConfiguration();

                FullyConfigured = await _apiConfigurationService.IsConfigurationReady();
            }
            catch (Exception)
            {
                if (ApiConfiguration == null)
                    ApiConfiguration = new ApiConfiguration();

                if (UserConfiguration == null)
                    UserConfiguration = new UserConfiguration();
            }
        }

        private ApiConfiguration _apiConfiguration;
        public ApiConfiguration ApiConfiguration
        {
            get { return _apiConfiguration; }
            set { Set(ref _apiConfiguration, value); }
        }


        private UserConfiguration _userConfiguration;
        public UserConfiguration UserConfiguration
        {
            get { return _userConfiguration; }
            set { Set(ref _userConfiguration, value); }
        }

        private bool _fullyConfigured;
        public bool FullyConfigured
        {
            get { return _fullyConfigured; }
            set { Set(ref _fullyConfigured, value); }
        }

        private readonly LoadingRelayCommand _setConfigurationCommand;
        public ICommand SetConfigurationCommand => _setConfigurationCommand;

        private async Task SetConfigurationAsync()
        {
            await _apiConfigurationService.SetApiConfigurationAsync(ApiConfiguration);
            await _apiConfigurationService.SetUserConfigurationAsync(UserConfiguration);
            FullyConfigured = await _apiConfigurationService.IsConfigurationReady();
        }
    }
}
