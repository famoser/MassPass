using System;
using System.Diagnostics.Tracing;
using System.Windows.Input;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Enums;
using Famoser.MassPass.View.Interfaces;
using Famoser.MassPass.View.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Famoser.MassPass.View.ViewModels
{
    public class PasswordPageViewModel : ViewModelBase
    {
        private readonly IHistoryNavigationService _historyNavigationService;
        private readonly IPasswordVaultService _passwordVaultService;
        private readonly IApiConfigurationService _apiConfigurationService;

        public PasswordPageViewModel(IHistoryNavigationService historyNavigationService, IPasswordVaultService passwordVaultService, IApiConfigurationService apiConfigurationService)
        {
            _historyNavigationService = historyNavigationService;
            _passwordVaultService = passwordVaultService;
            _apiConfigurationService = apiConfigurationService;

            _unlockCommand = new RelayCommand(Unlock);
            _initializeCommand = new RelayCommand(GoToInitializePage);

            if (IsInDesignMode)
            {
                Initialized = false;
            }
            else
            {
                InitializeAsync();
            }
        }

        private async void InitializeAsync()
        {
            Initialized = await _apiConfigurationService.IsConfigurationReady();
        }

        private bool _initialized;
        public bool Initialized
        {
            get { return _initialized; }
            set { Set(ref _initialized, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { Set(ref _password, value); }
        }

        private readonly RelayCommand _unlockCommand;
        public ICommand UnlockCommand => _unlockCommand;

        private async void Unlock()
        {
            var bo = await _passwordVaultService.TryUnlockVaultAsync(_password);
            if (bo && _passwordVaultService.IsVaultUnLocked())
                _historyNavigationService.NavigateTo(PageKeys.CollectionsPage.ToString());
            else
                WrongPasswordEvent.Invoke(this, new EventArgs());
        }

        private readonly RelayCommand _initializeCommand;
        public ICommand InitializeCommand => _initializeCommand;

        private void GoToInitializePage()
        {
            _historyNavigationService.NavigateTo(PageKeys.InitialisationPage.ToString());
        }

        public EventHandler WrongPasswordEvent;
    }
}
