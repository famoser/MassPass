using System;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Models.Storage;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Famoser.MassPass.View.ViewModels
{
    public class InitialisationPageViewModel : ViewModelBase
    {
        private readonly IApiConfigurationService _apiConfigurationService;
        private readonly IHistoryNavigationService _historyNavigationService;
        private readonly IContentRepository _contentRepository;

        public InitialisationPageViewModel(IApiConfigurationService apiConfigurationService, IHistoryNavigationService historyNavigationService, IContentRepository contentRepository)
        {
            _apiConfigurationService = apiConfigurationService;
            _historyNavigationService = historyNavigationService;
            _contentRepository = contentRepository;

            _trySetApiConfigurationCommand = new RelayCommand<string>(SetApiConfiguration, CanSetApiConfguration);
            _trySetUserConfigurationCommand = new RelayCommand<string>(SetUserConfiguration, CanSetUserConfguration);
            _confirmCommand = new RelayCommand(Confirm, () => CanConfirm);

            if (IsInDesignMode)
            {
                CanSetApiConfiguration = true;
            }
        }

        private readonly RelayCommand<string> _trySetApiConfigurationCommand;
        public ICommand TrySetApiConfigurationCommand => _trySetApiConfigurationCommand;

        private bool CanSetApiConfguration(string config)
        {
            return !_isSettingApiConfiguration && !_isConfirming;
        }

        private bool _isSettingApiConfiguration;

        private bool _canSetApiConfiguration;
        public bool CanSetApiConfiguration
        {
            get { return _canSetApiConfiguration; }
            set
            {
                if (Set(ref _canSetApiConfiguration, value))
                    RaisePropertyChanged(() => CanConfirm);
            }
        }

        private string _lastApiConfiguration;
        private void SetApiConfiguration(string config)
        {
            _isSettingApiConfiguration = true;
            _trySetApiConfigurationCommand.RaiseCanExecuteChanged();

            CanSetApiConfiguration = _apiConfigurationService.CanSetApiConfigurationAsync(config);
            _lastApiConfiguration = config;

            _isSettingApiConfiguration = false;
            _trySetApiConfigurationCommand.RaiseCanExecuteChanged();
            _confirmCommand.RaiseCanExecuteChanged();
        }

        private readonly RelayCommand<string> _trySetUserConfigurationCommand;
        public ICommand TrySetUserConfigurationCommand => _trySetUserConfigurationCommand;

        private bool CanSetUserConfguration(string config)
        {
            return !_isSettingUserConfiguration && !_isConfirming;
        }

        private bool _isSettingUserConfiguration;

        private bool _canSetUserConfiguration;
        public bool CanSetUserConfiguration
        {
            get { return _canSetUserConfiguration; }
            set
            {
                if (Set(ref _canSetUserConfiguration, value))
                    RaisePropertyChanged(() => CanConfirm);
            }
        }
        private string _lastUserConfiguration;
        private void SetUserConfiguration(string content)
        {
            _isSettingUserConfiguration = true;
            _trySetUserConfigurationCommand.RaiseCanExecuteChanged();

            CanSetUserConfiguration = _apiConfigurationService.CanSetUserConfigurationAsync(content);
            _lastUserConfiguration = content;

            _isSettingUserConfiguration = false;
            _trySetUserConfigurationCommand.RaiseCanExecuteChanged();
            _confirmCommand.RaiseCanExecuteChanged();
        }

        private bool _createNewUserConfiguration;
        public bool CreateNewUserConfiguration
        {
            get { return _createNewUserConfiguration; }
            set
            {
                if (Set(ref _createNewUserConfiguration, value))
                {
                    _confirmCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged(() => CanConfirm);
                }
            }
        }

        private string _masterPassword;
        public string MasterPassword
        {
            get { return _masterPassword; }
            set
            {
                if (Set(ref _masterPassword, value))
                    RaisePropertyChanged(() => CanConfirm);
            }
        }

        private readonly RelayCommand _confirmCommand;
        public ICommand ConfirmCommand => _confirmCommand;

        public bool CanConfirm => CanSetApiConfiguration && (CanSetUserConfiguration || CreateNewUserConfiguration) && !_isConfirming && !string.IsNullOrEmpty(MasterPassword);

        private bool _isConfirming;
        private async void Confirm()
        {
            _isConfirming = true;
            _confirmCommand.RaiseCanExecuteChanged();
            _trySetUserConfigurationCommand.RaiseCanExecuteChanged();
            _trySetApiConfigurationCommand.RaiseCanExecuteChanged();

            bool res1 = await _apiConfigurationService.TrySetApiConfigurationAsync(_lastApiConfiguration);
            bool res2;
            if (CreateNewUserConfiguration)
            {
                res2 = await _apiConfigurationService.SetUserConfigurationAsync(new UserConfiguration()
                {
                    UserId = Guid.NewGuid()
                },
                Guid.NewGuid());
            }
            else
            {
                res2 = await _apiConfigurationService.TrySetUserConfigurationAsync(_lastUserConfiguration, Guid.NewGuid());
            }
            if (res1 && res2)
            {
                await _contentRepository.InitializeVault(MasterPassword);
                _historyNavigationService.NavigateTo(PageKeys.UnlockPage.ToString());
            }
            else
            {
                if (!res1)
                    CanSetApiConfiguration = false;
                if (!res2)
                    CanSetUserConfiguration = false;
            }

            _isConfirming = false;
            _confirmCommand.RaiseCanExecuteChanged();
            _trySetUserConfigurationCommand.RaiseCanExecuteChanged();
            _trySetApiConfigurationCommand.RaiseCanExecuteChanged();
        }
    }
}
