using System;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.FrameworkEssentials.View.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Enums;
using Famoser.MassPass.View.Events;
using Famoser.MassPass.View.Helpers;
using Famoser.MassPass.View.Models.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Famoser.MassPass.View.ViewModels.ContentPageViewModels
{
    public abstract class BaseContentPageViewModel : ViewModelBase, INavigationBackNotifier, IContentViewModel
    {
        private readonly IPasswordVaultService _passwordVaultService;
        private readonly IContentRepository _contentRepository;
        private readonly IHistoryNavigationService _historyNavigationService;
        private readonly IProgressService _progressService;

        protected BaseContentPageViewModel(IPasswordVaultService passwordVaultService, IHistoryNavigationService historyNavigationService, IContentRepository contentRepository, IProgressService progressService)
        {
            _passwordVaultService = passwordVaultService;
            _historyNavigationService = historyNavigationService;
            _contentRepository = contentRepository;
            _progressService = progressService;

            _syncCommand = new RelayCommand(Sync, () => CanSync);
            _shareCommand = new RelayCommand(Share);
            _addCommand = new RelayCommand(Add);
            _lockCommand = new RelayCommand(Lock);
            _navigateToCommand = new RelayCommand<ContentModel>(NavigateTo, CanExecuteNavigateTo);
            _fillHistoryCommand = new RelayCommand(FillHistory, () => CanFillHistory);
            _saveCommand = new RelayCommand(Save, () => CanSave);

            ContentModelChanged += SetContentModel;
            ContentModelLoaded += PrepareContentModel;
        }

        private void PrepareContentModel(object sender, ContentModelEventArgs eventArgs)
        {
            var model = eventArgs.ContentModel;
            if (IsContentTypeApplicable(model.ContentType))
            {
                CustomContentModel = PrepareCustomContentModel();
            }
        }

        //set content model, load if applicable
        private async void SetContentModel(object sender, ContentModelEventArgs eventArgs)
        {
            var model = eventArgs.ContentModel;
            if (IsContentTypeApplicable(model.ContentType))
            {
                //applicable
                RaisePropertyChanged(() => ContentModel);
                var load = false;
                //check for loading state, do stuff as applicable
                lock (ContentModelLoadingLock)
                {
                    if (model.ContentLoadingState < LoadingState.Loading)
                    {
                        //set to loading, load it outside of lock
                        model.ContentLoadingState = LoadingState.Loading;
                        load = true;
                    }
                    else if (model.ContentLoadingState == LoadingState.Loading)
                        //return as another viewmodel is already loading this object
                        return;
                }
                //load
                if (load)
                {
                    await _contentRepository.FillValues(model);
                    model.ContentLoadingState = LoadingState.Loaded;
                }
                //raise event
                ContentModelLoaded.Invoke(this, new ContentModelEventArgs(_contentModel));
            }
        }

        protected static void SetContentModelStatic(ContentModel model)
        {
            _contentModel = model;
            ContentModelChanged.Invoke(null, new ContentModelEventArgs(_contentModel));
        }

        private static ContentModel _contentModel;
        public ContentModel ContentModel
        {
            get { return _contentModel; }
            set
            {
                var nm = value;
                if (NavigateToCommand.CanExecute(nm))
                    NavigateToCommand.Execute(nm);
            }
        }

        protected static EventHandler<ContentModelEventArgs> ContentModelChanged;
        protected static EventHandler<ContentModelEventArgs> ContentModelLoaded;
        protected static object ContentModelLoadingLock = new object();

        private ICustomContentModel _customContentModel;
        private ICustomContentModel CustomContentModel
        {
            get { return _customContentModel; }
            set
            {
                if (_customContentModel?.CanBeSavedChanged != null)
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
        private bool CanSync => !IsSyncing;
        private bool IsSyncing { get; set; }
        private async void Sync()
        {
            using (new IndeterminateProgressDisposable<ProgressKeys, object>(_syncCommand, z => IsSyncing = z, ProgressKeys.Sync, _progressService))
            {
                await _contentRepository.SyncAsync();
            }
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
        private bool CanExecuteNavigateTo(ContentModel model)
        {
            return model != null;
        }
        private void NavigateTo(ContentModel model)
        {
            var oldContent = ContentModel;
            if (model.ContentType == ContentType.Folder)
                _historyNavigationService.NavigateTo(PageKeys.FolderContentPage.ToString(), this, oldContent);
            else if (model.ContentType == ContentType.Note)
                _historyNavigationService.NavigateTo(PageKeys.NoteContentPage.ToString(), this, oldContent);
            SetContentModelStatic(model);
        }

        private readonly RelayCommand _fillHistoryCommand;
        public ICommand FillHistoryCommand => _fillHistoryCommand;
        private bool CanFillHistory => !IsFillingHistory;
        private bool IsFillingHistory { get; set; }
        public async void FillHistory()
        {
            using (new IndeterminateProgressDisposable<ProgressKeys, object>(_fillHistoryCommand, z => IsFillingHistory = z, ProgressKeys.FillHistory, _progressService))
            {
                if (ContentModel.HistoryLoadingState < LoadingState.Loading)
                {
                    ContentModel.HistoryLoadingState = LoadingState.Loading;
                    await _contentRepository.FillHistory(ContentModel);
                    ContentModel.HistoryLoadingState = LoadingState.Loaded;
                }
            }
        }

        private readonly RelayCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand;
        public bool CanSave => CustomContentModel != null && CustomContentModel.CanBeSaved() && CustomContentModel.ContentChanged() && !IsSaving;
        private bool IsSaving { get; set; }
        public async void Save()
        {
            using (new IndeterminateProgressDisposable<ProgressKeys, object>(_saveCommand, z => IsSaving = z, ProgressKeys.Saving, _progressService))
            {
                SaveToContentModel();
                await _contentRepository.Save(ContentModel);
                CustomContentModel = PrepareCustomContentModel();
            }
        }
        #endregion

        public void HandleNavigationBack(object message)
        {
            var coll = message as ContentModel;
            if (coll != null)
            {
                SetContentModelStatic(coll);
            }
        }

        public abstract bool IsContentTypeApplicable(ContentType type);

        public abstract ICustomContentModel PrepareCustomContentModel();

        public abstract bool SaveToContentModel();
    }
}
