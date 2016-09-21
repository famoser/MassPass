using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.FrameworkEssentials.View.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace Famoser.MassPass.View.ViewModels
{
    public class PasswordPageViewModel : ViewModelBase, INavigationBackNotifier
    {
        private readonly IHistoryNavigationService _historyNavigationService;
        private readonly IPasswordVaultService _passwordVaultService;
        private readonly IApiConfigurationService _apiConfigurationService;

        public PasswordPageViewModel(IHistoryNavigationService historyNavigationService, IPasswordVaultService passwordVaultService, IApiConfigurationService apiConfigurationService)
        {
            _historyNavigationService = historyNavigationService;
            _passwordVaultService = passwordVaultService;
            _apiConfigurationService = apiConfigurationService;

            _unlockCommand = new LoadingRelayCommand(Unlock);
            _initializeCommand = new LoadingRelayCommand(GoToInitializePage, () => Initialized);
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
            Initialized = await _passwordVaultService.IsInitializedAsync() && await _apiConfigurationService.IsConfigurationReady();
        }

        private bool _initialized;
        public bool Initialized
        {
            get { return _initialized; }
            set
            {
                if (Set(ref _initialized, value))
                    _initializeCommand.RaiseCanExecuteChanged();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { Set(ref _password, value); }
        }

        private readonly LoadingRelayCommand _unlockCommand;
        public ICommand UnlockCommand => _unlockCommand;
        private async Task Unlock()
        {
            var bo = await _passwordVaultService.TryUnlockVaultAsync(_password);
            if (bo && _passwordVaultService.IsVaultUnLocked())
                _historyNavigationService.NavigateToAndForget(PageKeys.ListContentPage.ToString());
            else
                WrongPasswordEvent?.Invoke(this, EventArgs.Empty);
        }

        private readonly LoadingRelayCommand _initializeCommand;
        public ICommand InitializeCommand => _initializeCommand;

        private async Task GoToInitializePage()
        {
            await _passwordVaultService.CreateNewVaultAsync(Password);
            if (await _passwordVaultService.TryUnlockVaultAsync(Password))
            {
                var vm = SimpleIoc.Default.GetInstance<InitialisationPageViewModel>();
                await vm.LoadExistigConfigurationAsync();
                _historyNavigationService.NavigateTo(PageKeys.InitialisationPage.ToString(), this);
            }
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
