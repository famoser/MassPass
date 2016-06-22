using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.View.Models
{
    public class RootModel : NameModel
    {
        private ObservableCollection<ContentModel> _children;
        public ObservableCollection<ContentModel> Children
        {
            get { return _children; }
            set { Set(ref _children, value); }
        }
    }
}
