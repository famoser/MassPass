using System;
using System.Collections.ObjectModel;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Helpers;
using Famoser.MassPass.Business.Models.Base;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Models;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Models
{
    public class CollectionModel : BaseModel
    {
        public CollectionModel()
        {
            Contents = new ObservableCollection<ContentModel>();
            History = new ObservableCollection<HistoryModel>();
        }

        public Guid Id { get; set; }

        public Guid TypeId { get; set; }

        public Guid ParentId { get; set; }

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

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        [JsonIgnore]
        public CollectionModel Parent { get; set; }

        [JsonIgnore]
        public ObservableCollection<ContentModel> Contents { get; }

        [JsonIgnore]
        public ObservableCollection<HistoryModel> History { get; }

        private bool _saveDisabled;
        [JsonIgnore]
        public bool SaveDisabled
        {
            get { return _saveDisabled; }
            set { Set(ref _saveDisabled, value); }
        }

        public ContentTypes ContentType
        {
            get { return ContentHelper.GetType(this); }
        }
    }
}
