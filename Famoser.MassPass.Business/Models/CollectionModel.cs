using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models.Base;
using Famoser.MassPass.Data.Models;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Models
{
    public class CollectionModel : BaseModel
    {
        public CollectionModel(Guid id, Guid typeId, ServerInformations serverInformations, CollectionModel parent = null)
        {
            Id = id;
            TypeId = typeId;
            ServerInformations = serverInformations;
            Parent = parent;
            Contents = new ObservableCollection<ContentModel>();
        }

        public Guid Id { get; }

        public Guid TypeId { get; }

        public ServerInformations ServerInformations { get; }

        public CollectionModel Parent { get; }

        public ObservableCollection<ContentModel> Contents { get; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        public bool IsCollection => Id == Guid.Empty;
        
        public ContentModel CreateNewContent(Guid typeId)
        {
            return new ContentModel(Guid.NewGuid(), typeId, null, this);
        }
    }
}
