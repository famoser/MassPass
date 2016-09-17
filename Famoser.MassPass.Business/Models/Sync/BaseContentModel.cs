using System;

namespace Famoser.MassPass.Business.Models.Sync
{
    public class BaseContentModel : BaseSyncModel
    {
        public BaseContentModel(Guid id) : base(id)
        {

        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }
    }
}
