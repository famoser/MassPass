﻿using System.Windows.Input;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.Data.Services.Interfaces;
using GalaSoft.MvvmLight.Command;

namespace Famoser.MassPass.View.ViewModels
{
    public class ContentPageViewModel : CollectionsPageViewModel
    {
        private readonly IContentRepository _contentRepository;

        public ContentPageViewModel(IContentRepository contentRepository, IPasswordVaultService passwordVaultService, ICollectionRepository collectionRepository, IHistoryNavigationService historyNavigationService) : base (passwordVaultService, collectionRepository, historyNavigationService)
        {
            _contentRepository = contentRepository;

            _fillHistoryCommand = new RelayCommand(FillHistory);

            if (IsInDesignMode)
            {
                _contentModel = _contentRepository.GetSampleContent();
            }
        }

        private ContentModel _contentModel;
        public ContentModel ContentModel
        {
            get { return _contentModel; }
            set
            {
                if (Set(ref _contentModel, value))
                    LoadIfApplicable();
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
    }
}
