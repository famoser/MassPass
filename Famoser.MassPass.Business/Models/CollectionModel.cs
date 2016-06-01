using System;
using System.Collections.ObjectModel;
using Famoser.MassPass.Business.Models.Base;
using Famoser.MassPass.Data.Enum;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Business.Models
{
    public class CollectionModel : BaseModel
    {
        public CollectionModel()
        {
            Contents = new ObservableCollection<ContentModel>();
        }

        public Guid Id { get; set; }

        public Guid TypeId { get; set; }

        public EntityServerInformations EntityServerInformations { get; set; }

        public CollectionModel Parent { get; set; }

        public ObservableCollection<ContentModel> Contents { get; }

        private ContentStatus _contentStatus;
        public ContentStatus ContentStatus
        {
            get { return _contentStatus; }
            set { Set(ref _contentStatus, value); }
        }

        private LocalStatus _localStatus;
        public LocalStatus LocalStatus
        {
            get { return _localStatus; }
            set { Set(ref _localStatus, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }
    }
}
