using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Content.Base;
using Famoser.MassPass.Business.Models.Sync;

namespace Famoser.MassPass.Business.Models.Content
{
    public class NoteModel : BaseContentModel
    {
        public NoteModel(Guid id) : base(id, ContentTypes.Note)
        {
        }
    }
}
