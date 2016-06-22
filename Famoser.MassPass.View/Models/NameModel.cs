using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.MassPass.View.Models
{
    public class NameModel : BaseContentModel
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (Set(ref _name, value))
                    RaiseCanBeSaved();
            }
        }

        public override bool CanBeSaved()
        {
            return string.IsNullOrEmpty(Name);
        }
    }
}
