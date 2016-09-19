using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models.Content.Base;

namespace Famoser.MassPass.Business.Models
{
    public class GroupedCollectionModel
    {
        public GroupedCollectionModel(string name, ObservableCollection<BaseContentModel> contentModels)
        {
            Name = name;
            ContentModels = contentModels;
        }
        public string Name { get; }
        public ObservableCollection<BaseContentModel> ContentModels { get; }
    }
}
