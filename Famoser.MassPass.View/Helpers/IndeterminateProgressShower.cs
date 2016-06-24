﻿using System;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.View.Enums;
using GalaSoft.MvvmLight.Command;

namespace Famoser.MassPass.View.Helpers
{
    public class IndeterminateProgressShower : IDisposable
    {
        private readonly RelayCommand _relayCommand;
        private readonly Action<bool> _booleanSetter;
        private readonly ProgressKeys _progressKey;
        private readonly IProgressService _progressService;

        public IndeterminateProgressShower(RelayCommand command, Action<bool> booleanSetter, ProgressKeys key, IProgressService progressService)
        {
            _relayCommand = command;
            _booleanSetter = booleanSetter;
            _progressKey = key;
            _progressService = progressService;
        }

        public void Start()
        {
            _progressService.StartIndeterminateProgress(_progressKey);
            _booleanSetter(true);
            _relayCommand.RaiseCanExecuteChanged();
        }

        public void Dispose()
        {
            _progressService.StopIndeterminateProgress(_progressKey);
            _booleanSetter(false);
            _relayCommand.RaiseCanExecuteChanged();
        }
    }
}
