using Famoser.MassPass.Business.Enums;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Models
{
    public class ContentModel : CollectionModel
    {
        private string _contentJson;
        public string ContentJson
        {
            get { return _contentJson; }
            set { Set(ref _contentJson, value); }
        }

        private byte[] _contentFile;
        public byte[] ContentFile
        {
            get { return _contentFile; }
            set { Set(ref _contentFile, value); }
        }

        private LoadingState _contentLoadingState;
        public LoadingState ContentLoadingState
        {
            get { return _contentLoadingState; }
            set { Set(ref _contentLoadingState, value); }
        }

        private LoadingState _historyLoadingState;
        public LoadingState HistoryLoadingState
        {
            get { return _historyLoadingState; }
            set { Set(ref _historyLoadingState, value); }
        }

        private CurrentStatus _currentStatus;
        [JsonIgnore]
        public CurrentStatus CurrentStatus
        {
            get { return _currentStatus; }
            set { Set(ref _currentStatus, value); }
        }
    }
}
