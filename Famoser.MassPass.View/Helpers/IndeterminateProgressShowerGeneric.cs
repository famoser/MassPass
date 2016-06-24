using System;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.View.Enums;
using GalaSoft.MvvmLight.Command;

namespace Famoser.MassPass.View.Helpers
{
    public class IndeterminateProgressShower<T> : IDisposable
    {
        private readonly RelayCommand<T> _typedRelayCommand;
        private readonly Action<bool> _booleanSetter;
        private readonly ProgressKeys _progressKey;
        private readonly IProgressService _progressService;

        public IndeterminateProgressShower(RelayCommand<T> command, Action<bool> booleanSetter, ProgressKeys key, IProgressService progressService)
        {
            _typedRelayCommand = command;
            _booleanSetter = booleanSetter;
            _progressKey = key;
            _progressService = progressService;
        }

        public void Start()
        {
            _progressService.StartIndeterminateProgress(_progressKey);
            _booleanSetter(true);
            _typedRelayCommand.RaiseCanExecuteChanged();
        }

        public void Dispose()
        {
            _progressService.StopIndeterminateProgress(_progressKey);
            _booleanSetter(false);
            _typedRelayCommand.RaiseCanExecuteChanged();
        }
    }
}
