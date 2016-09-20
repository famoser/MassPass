using System;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.FrameworkEssentials.View.Interfaces;
using Famoser.MassPass.Business.Services.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Famoser.MassPass.View.ViewModels
{
    public class PasswordPageViewModel : ViewModelBase, INavigationBackNotifier
    {
        private readonly IHistoryNavigationService _historyNavigationService;
        private readonly IPasswordVaultService _passwordVaultService;
        private readonly IApiConfigurationService _apiConfigurationService;
        private readonly IProgressService _progressService;

        public PasswordPageViewModel(IHistoryNavigationService historyNavigationService, IPasswordVaultService passwordVaultService, IApiConfigurationService apiConfigurationService, IProgressService progressService)
        {
            _historyNavigationService = historyNavigationService;
            _passwordVaultService = passwordVaultService;
            _apiConfigurationService = apiConfigurationService;
            _progressService = progressService;

            _unlockCommand = new RelayCommand(Unlock, () => CanUnlock);
            _initializeCommand = new RelayCommand(GoToInitializePage);
            _aboutCommand = new RelayCommand(GoToAboutPage);
            

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
        private bool IsUnlocking { get; set; }
        private bool CanUnlock => !IsUnlocking;
        private async void Unlock()
        {
            using (new IndeterminateProgressDisposable<ProgressKeys, object>(_unlockCommand, z => IsUnlocking = z, ProgressKeys.Unlocking, _progressService))
            {
                var bo = await _passwordVaultService.TryUnlockVaultAsync(_password);
                if (bo && _passwordVaultService.IsVaultUnLocked())
                    _historyNavigationService.NavigateTo(PageKeys.RootContentPage.ToString());
                else
                    WrongPasswordEvent?.Invoke(this, new EventArgs());
            }
        }

        private readonly RelayCommand _initializeCommand;
        public ICommand InitializeCommand => _initializeCommand;

        private void GoToInitializePage()
        {
            _historyNavigationService.NavigateTo(PageKeys.InitialisationPage.ToString(), this);
        }

        private readonly RelayCommand _aboutCommand;
        public ICommand AboutCommand => _aboutCommand;

        private void GoToAboutPage()
        {
            _historyNavigationService.NavigateTo(PageKeys.AboutPage.ToString(), this);
        }

        public void HandleNavigationBack(object message)
        {
            InitializeAsync();
        }

        public EventHandler WrongPasswordEvent;
    }
}
