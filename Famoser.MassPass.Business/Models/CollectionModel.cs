using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models.Base;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Models
{
    public class CollectionModel : BaseSyncModel
    {
        [JsonConstructor]
        public CollectionModel() { }

        public CollectionModel(Guid id, Guid typeId)
            : base(id)
        {
            Type = typeId;
            Contents = new ObservableCollection<ContentModel>();    
        }

        public ObservableCollection<ContentModel> Contents { get; }

        public Guid Type { get; }

        public bool IsCollection => Id == Guid.Empty;
    }
}
