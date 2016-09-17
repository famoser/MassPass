using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Base;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Models;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Models.Sync
{
    public class BaseSyncModel : BaseModel
    {
        public BaseSyncModel(Guid id)
        {
            Id = id;
            Contents = new ObservableCollection<BaseContentModel>();
            History = new ObservableCollection<HistoryModel>();
        }

        public Guid Id { get; private set; }

        public ApiInformations ApiInformations { get; set; }

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
        
        public ObservableCollection<BaseContentModel> Contents { get; }

        private LoadingState _contentLoadingState;
        public LoadingState ContentLoadingState
        {
            get { return _contentLoadingState; }
            set { Set(ref _contentLoadingState, value); }
        }

        public ObservableCollection<HistoryModel> History { get; }

        private LoadingState _historyLoadingState;
        public LoadingState HistoryLoadingState
        {
            get { return _historyLoadingState; }
            set { Set(ref _historyLoadingState, value); }
        }


        private RuntimeStatus _runtimeStatus;
        public RuntimeStatus RuntimeStatus
        {
            get { return _runtimeStatus; }
            set { Set(ref _runtimeStatus, value); }
        }
    }
}
