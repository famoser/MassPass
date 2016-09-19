using System;
using System.Collections.ObjectModel;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Base;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Business.Models.Content.Base
{
    public abstract class BaseContentModel : BaseModel
    {
        protected BaseContentModel(Guid id, ContentType type)
        {
            Id = id;
            ContentType = type;
            History = new ObservableCollection<HistoryModel>();
        }

        public Guid Id { get; private set; }
        public ContentApiInformations ContentApiInformations { get; set; }
        public ContentType ContentType { get; private set; }
        
        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        private LocalStatus _localStatus;
        public LocalStatus LocalStatus
        {
            get { return _localStatus; }
            set { Set(ref _localStatus, value); }
        }

        private LivecycleStatus _livecycleStatus;
        public LivecycleStatus LivecycleStatus
        {
            get { return _livecycleStatus; }
            set { Set(ref _livecycleStatus, value); }
        }

        private LoadingState _contentLoadingState;
        public LoadingState ContentLoadingState
        {
            get { return _contentLoadingState; }
            set { Set(ref _contentLoadingState, value); }
        }

        public ObservableCollection<HistoryModel> History { get; }
        public CollectionModel Collection { get; set; }

        private LoadingState _historyLoadingState;
        public LoadingState HistoryLoadingState
        {
            get { return _historyLoadingState; }
            set { Set(ref _historyLoadingState, value); }
        }
        
        protected RuntimeStatus RuntimeStatus { get; set; }

        
    }
}
