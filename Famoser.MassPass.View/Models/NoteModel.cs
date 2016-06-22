using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models.Base;
using Famoser.MassPass.View.Models.Interfaces;

namespace Famoser.MassPass.View.Models
{
    public class NoteModel : NameModel
    {
        private string _content;
        public string Content
        {
            get { return _content; }
            set
            {
                if (Set(ref _content, value))
                    RaiseCanBeSaved();
            }
        }
    }
}
