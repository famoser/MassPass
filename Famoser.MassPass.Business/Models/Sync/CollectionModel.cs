namespace Famoser.MassPass.Business.Models.Sync
{
    public class CollectionModel : BaseSyncModel
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }
    }
}
