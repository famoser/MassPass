using System.Collections.ObjectModel;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using Famoser.MassPass.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Famoser.MassPass.View.ViewModels
{
    public class CollectionsPageViewModel : ViewModelBase, INavigationBackNotifier
    {
        private IPasswordVaultService _passwordVaultService;
        private readonly ICollectionRepository _collectionRepository;
        private readonly IHistoryNavigationService _historyNavigationService;

        public CollectionsPageViewModel(IPasswordVaultService passwordVaultService, ICollectionRepository collectionRepository, IHistoryNavigationService historyNavigationService)
        {
            _passwordVaultService = passwordVaultService;
            _collectionRepository = collectionRepository;
            _historyNavigationService = historyNavigationService;

            _collections = _collectionRepository.GetCollectionsAndLoad();

            _syncCommand = new RelayCommand(Sync, () => CanSync);
            _shareCommand = new RelayCommand(Share);
            _addCommand = new RelayCommand(Add);
            _lockCommand = new RelayCommand(Lock);

            if (IsInDesignMode)
            {
                Collections = _collectionRepository.GetSampleCollections();
            }
        }

        private ObservableCollection<ContentModel> _collections;
        public ObservableCollection<ContentModel> Collections
        {
            get { return _collections; }
            set { Set(ref _collections, value); }
        }

        private ContentModel _selectedModel;
        public ContentModel SelecetedModel
        {
            get { return _selectedModel; }
            set
            {
                if (Set(ref _selectedModel, value))
                {
                    if (_selectedModel != null)
                    {
                        if (_selectedModel.ContentType == ContentTypes.Folder)
                        {
                            var mod = _selectedModel;
                            _selectedModel = null;
                            _historyNavigationService.NavigateTo(PageKeys.CollectionsPage.ToString(), this, mod.Contents);
                        }
                        else
                        {
                            //todo: change archtiecture; make a root model 
                            var mod = _selectedModel;
                            _selectedModel = null;
                            _historyNavigationService.NavigateTo(PageKeys.ContentPage.ToString(), this, mod);
                        }
                    }
                }
            }
        }

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

        public void HandleNavigationBack(object message)
        {
            var coll = message as ObservableCollection<ContentModel>;
            if (coll != null)
            {
                Collections = coll;
            }
        }
    }
}
