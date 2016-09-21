using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.View.Commands;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Repositories.Interfaces;
using Famoser.MassPass.View.Content.Interfaces;
using Famoser.MassPass.View.Models.Base;
using GalaSoft.MvvmLight;

namespace Famoser.MassPass.View.ViewModels
{
    public abstract class ContentPageViewModel<T1, T2> : ViewModelBase
        where T1 : BaseContentModel
        where T2 : ViewContentModel
    {
        private readonly IContentRepository _contentRepository;
        private IViewContentModelProvider _provider;

        protected ContentPageViewModel(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;

            _fillHistoryCommand = new LoadingRelayCommand(FillHistory);
            _saveCommand = new LoadingRelayCommand(Save, () => CanSave);
        }

        public void SetViewContentModelProvider(IViewContentModelProvider provider)
        {
            _provider = provider;
        }
        
        //set content model, load if applicable
        public async Task SetContentModelAsync(T1 contentModel)
        {
            BaseContentModel = contentModel;
            await _contentRepository.LoadValues(BaseContentModel);
            ViewContentModel = _provider.GetViewContentModel(BaseContentModel) as T2;

            _saveCommand.RaiseCanExecuteChanged();
        }

        private static T1 _baseContentModel;
        public T1 BaseContentModel
        {
            get { return _baseContentModel; }
            set { Set(ref _baseContentModel, value); }
        }

        private static T2 _viewContentModel;
        public T2 ViewContentModel
        {
            get { return _viewContentModel; }
            set
            {
                if (_viewContentModel?.CanBeSavedChanged != null)
                    _viewContentModel.CanBeSavedChanged -= CanBeSavedChanged;

                Set(ref _viewContentModel, value);

                if (_viewContentModel != null)
                    _viewContentModel.CanBeSavedChanged += CanBeSavedChanged;
            }
        }

        private void CanBeSavedChanged(object sender, EventArgs eventArgs)
        {
            _saveCommand.RaiseCanExecuteChanged();
        }


        private readonly LoadingRelayCommand _fillHistoryCommand;
        public ICommand FillHistoryCommand => _fillHistoryCommand;
        public async Task FillHistory()
        {
            await _contentRepository.FillHistory(BaseContentModel);
        }

        private readonly LoadingRelayCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand;
        public bool CanSave => ViewContentModel != null && ViewContentModel.CanBeSaved() && ViewContentModel.ContentChanged();
        public async Task Save()
        {
            _provider.SaveValues(BaseContentModel, ViewContentModel);
            await _contentRepository.Save(BaseContentModel);
        }
    }
}
