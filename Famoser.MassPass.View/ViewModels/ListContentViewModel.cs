using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.View.Content;
using Famoser.MassPass.View.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Famoser.MassPass.View.ViewModels
{
    public class ListContentViewModel : ViewModelBase
    {
        private readonly IHistoryNavigationService _historyNavigationService;
        private readonly IContentRepository _contentRepository;

        public ListContentViewModel(IHistoryNavigationService historyNavigationService, IContentRepository contentRepository)
        {
            _historyNavigationService = historyNavigationService;
            _contentRepository = contentRepository;
            _syncCommand = new LoadingRelayCommand(Sync);
            _shareCommand = new RelayCommand(NavigateToSharePage);
            _addCommand = new RelayCommand<ContentType>(NavigateToAddPage);

            GroupedCollections = _contentRepository.GetGroupedCollectionModels();
        }

        public ObservableCollection<GroupedCollectionModel> GroupedCollections { get; }

        private readonly LoadingRelayCommand _syncCommand;
        public ICommand SyncCommand => _syncCommand;

        public async Task Sync()
        {
            await _contentRepository.SyncAsync();
        }

        private RelayCommand<BaseContentModel> _selectCommand;


        private readonly RelayCommand _shareCommand;
        public ICommand ShareCommand => _shareCommand;

        private void NavigateToSharePage()
        {
            _historyNavigationService.NavigateTo(PageKeys.AboutPage.ToString());
        }

        private readonly RelayCommand<ContentType> _addCommand;
        public ICommand AddCommand => _addCommand;

        private void NavigateToAddPage(ContentType type)
        {
            var provider = ViewContentHelper.GetProvider(type);
            _historyNavigationService.NavigateTo(provider.GetPageKey().ToString());
        }
    }
}
