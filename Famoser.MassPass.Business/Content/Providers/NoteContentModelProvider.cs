using System;
using Famoser.MassPass.Business.Content.Providers.Base;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Content;

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

       protected override Guid GetTypeGuid()
        {
            return Guid.Parse("a63b4e52-0a45-4ca5-8327-4ef800e00e57");
        }
    }
}
