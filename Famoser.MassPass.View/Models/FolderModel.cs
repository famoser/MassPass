using System.Collections.ObjectModel;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.View.Models
{
    public class FolderModel : NameModel
    {
        private ObservableCollection<ContentModel> _children;
        public ObservableCollection<ContentModel> Children
        {
            get { return _children; }
            set { Set(ref _children, value); }
        }
    }
}
