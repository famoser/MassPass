using System;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Content.Providers.Base;
using Famoser.MassPass.Business.Enums;

namespace Famoser.MassPass.Business.Content.Providers
{
   public class NoteContentModelProvider : BaseContentModelProvider<NoteModel>
    {
       public override string GetListName()
       {
           return "notes";
       }

       public override ContentType GetContentType()
       {
           return ContentType.Note;
       }

       protected override BaseContentModel ConstructModel(Guid id)
       {
           return new NoteModel(id);
       }

       protected override Guid GetTypeGuid()
        {
            return Guid.Parse("a63b4e52-0a45-4ca5-8327-4ef800e00e57");
        }
    }
}
