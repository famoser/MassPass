using System;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models.Base;

namespace Famoser.MassPass.Business.Content.Models
{
    public class NoteModel : BaseContentModel
    {
        public NoteModel(Guid id) : base(id, ContentType.Note)
        {
        }
    }
}
