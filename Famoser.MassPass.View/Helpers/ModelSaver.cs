using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.View.Models;
using Newtonsoft.Json;

namespace Famoser.MassPass.View.Helpers
{
    public class ModelSaver
    {
        public static bool SaveNoteModel(ContentModel model, NoteModel note)
        {
            model.ContentJson = JsonConvert.SerializeObject(note);
            return true;
        }
    }
}
