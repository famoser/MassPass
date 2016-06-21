﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Enums;
using Famoser.MassPass.View.Models.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Famoser.MassPass.View.ViewModels
{
    public abstract class ContentPageViewModel : ViewModelBase, INavigationBackNotifier, IContentViewModel
    {
        private readonly IPasswordVaultService _passwordVaultService;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IHistoryNavigationService _historyNavigationService;

        protected ContentPageViewModel(IPasswordVaultService passwordVaultService, ICollectionRepository collectionRepository, IHistoryNavigationService historyNavigationService, IContentRepository contentRepository)
        {
            _passwordVaultService = passwordVaultService;
            _collectionRepository = collectionRepository;
            _historyNavigationService = historyNavigationService;
            _contentRepository = contentRepository;

            _syncCommand = new RelayCommand(Sync, () => CanSync);
            _shareCommand = new RelayCommand(Share);
            _addCommand = new RelayCommand(Add);
            _lockCommand = new RelayCommand(Lock);
            _navigateToCommand = new RelayCommand<ContentModel>(NavigateTo);
            _fillHistoryCommand = new RelayCommand(FillHistory);
            _saveCommand = new RelayCommand(Save, () => CanSave);
        }

        private ContentModel _contentModel;
        protected ContentModel ContentModel
        {
           get { return _contentModel; }
            set
            {
                if (Set(ref _contentModel, value))
                    LoadIfApplicable();
            }
        }

        private ICustomContentModel _customContentModel;
        private ICustomContentModel CustomContentModel
        {
            get { return _customContentModel; }
            set
            {
                if (_customContentModel != null)
                    _customContentModel.CanBeSavedChanged -= CanBeSavedChanged;

                Set(ref _customContentModel, value);

                if (_customContentModel != null)
                    _customContentModel.CanBeSavedChanged += CanBeSavedChanged;
            }
        }

        private void CanBeSavedChanged(object sender, EventArgs eventArgs)
        {
            _saveCommand.RaiseCanExecuteChanged();
        }

        #region commands
        private readonly RelayCommand _syncCommand;
        public ICommand SyncCommand => _syncCommand;
        private bool CanSync { get; set; } = true;
        private async void Sync()
        {
            CanSync = false;
            _syncCommand.RaiseCanExecuteChanged();

            await _collectionRepository.SyncAsync();

            CanSync = true;
            _syncCommand.RaiseCanExecuteChanged();
        }

        private readonly RelayCommand _shareCommand;
        public ICommand ShareCommand => _shareCommand;
        private void Share()
        {
            _historyNavigationService.NavigateTo(PageKeys.SharePage.ToString());
        }

        private readonly RelayCommand _lockCommand;
        public ICommand LockCommand => _lockCommand;
        private void Lock()
        {
            _passwordVaultService.LockVault();
            _historyNavigationService.GoBack();
        }

        private readonly RelayCommand _addCommand;
        public ICommand AddCommand => _addCommand;
        private void Add()
        {
            _historyNavigationService.NavigateTo(PageKeys.AddPage.ToString());
        }

        private readonly RelayCommand<ContentModel> _navigateToCommand;
        public ICommand NavigateToCommand => _navigateToCommand;
        private void NavigateTo(ContentModel model)
        {
            var oldContent = ContentModel;
            if (model.ContentType == ContentTypes.Folder)
                _historyNavigationService.NavigateTo(PageKeys.CollectionsPage.ToString(), this, oldContent);
            else if (model.ContentType == ContentTypes.Note)
                _historyNavigationService.NavigateTo(PageKeys.NotePage.ToString(), this, oldContent);
            ContentModel = model;
        }

        private readonly RelayCommand _fillHistoryCommand;
        public ICommand FillHistoryCommand => _fillHistoryCommand;
        public async void FillHistory()
        {
            if (ContentModel.HistoryLoadingState < LoadingState.Loading)
            {
                ContentModel.HistoryLoadingState = LoadingState.Loading;
                await _contentRepository.FillHistory(ContentModel);
                ContentModel.HistoryLoadingState = LoadingState.Loaded;
            }
        }

        private readonly RelayCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand;
        public bool CanSave => CustomContentModel != null && CustomContentModel.CanBeSaved();
        public async void Save()
        {
            SaveModel();
        }
        #endregion

        public void HandleNavigationBack(object message)
        {
            var coll = message as ContentModel;
            if (coll != null)
            {
                ContentModel = coll;
            }
        }
        
        private async void LoadIfApplicable()
        {
            if (ContentModel.ContentLoadingState < LoadingState.Loading)
            {
                ContentModel.ContentLoadingState = LoadingState.Loading;
                await _contentRepository.FillValues(ContentModel);
                ContentModel.ContentLoadingState = LoadingState.Loaded;
            }
            CustomContentModel = PrepareModel();
        }

        
        public abstract ICustomContentModel PrepareModel();
        public abstract bool SaveModel();
    }
}
