using System;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Content.Base;

namespace Famoser.MassPass.Business.Models.Content
{
    public class NoteModel : BaseContentModel
    {
        public NoteModel(Guid id) : base(id, ContentType.Note)
        {
        }
    }
}
