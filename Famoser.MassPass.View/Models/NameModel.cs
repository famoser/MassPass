using Newtonsoft.Json;

namespace Famoser.MassPass.View.Models
{
    public class NameModel : BaseContentModel
    {
        private string _name;
        [JsonIgnore]
        public string Name
        {
            get { return _name; }
            set
            {
                if (Set(ref _name, value))
                    RegisterValueChange("Name", _name);
            }
        }

        public override bool CanBeSaved()
        {
            return !string.IsNullOrEmpty(Name);
        }
    }
}
