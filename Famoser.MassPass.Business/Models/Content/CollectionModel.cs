using System;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Content.Base;

namespace Famoser.MassPass.Business.Models.Content
{
    public class CollectionModel : BaseContentModel
    {
        public CollectionModel(Guid id) : base(id, ContentTypes.Note)
        {

        }
    }
}
