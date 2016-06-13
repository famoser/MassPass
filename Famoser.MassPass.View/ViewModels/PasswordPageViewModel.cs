using System;
using System.Diagnostics.Tracing;
using System.Windows.Input;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Enums;
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

        public PasswordPageViewModel(IHistoryNavigationService historyNavigationService, IPasswordVaultService passwordVaultService)
        {
            _historyNavigationService = historyNavigationService;
            _passwordVaultService = passwordVaultService;

            _unlockCommand = new RelayCommand(Unlock);
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

        public EventHandler WrongPasswordEvent;
    }
}
