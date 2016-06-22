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
        [JsonIgnore]
        public LoadingState ContentLoadingState
        {
            get { return _contentLoadingState; }
            set { Set(ref _contentLoadingState, value); }
        }

        private LoadingState _historyLoadingState;
        [JsonIgnore]
        public LoadingState HistoryLoadingState
        {
            get { return _historyLoadingState; }
            set { Set(ref _historyLoadingState, value); }
        }

        private RuntimeStatus _runtimeStatus;
        [JsonIgnore]
        public RuntimeStatus RuntimeStatus
        {
            get { return _runtimeStatus; }
            set { Set(ref _runtimeStatus, value); }
        }
    }
}
