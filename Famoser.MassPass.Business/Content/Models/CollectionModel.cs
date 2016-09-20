using System;
using System.Collections.ObjectModel;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models.Base;

namespace Famoser.MassPass.Business.Content.Models
{
    public class CollectionModel : BaseContentModel
    {
        public CollectionModel(Guid id) : base(id, ContentType.Note)
        {
            ContentModels = new ObservableCollection<BaseContentModel>();
        }

        public ObservableCollection<BaseContentModel> ContentModels { get; }
    }
}
