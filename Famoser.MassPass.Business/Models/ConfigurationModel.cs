using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Base;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Models
{
    public class ConfigurationModel : BaseModel
    {
        public Guid Guid { get; set; }
        public SettingKeys SettingKey { get; set; }
        public bool Immutable { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                if (Set(ref _value, value))
                {
                    RaisePropertyChanged(() => BoolValue);
                    RaisePropertyChanged(() => IntValue);
                }
            }
        }

        [JsonIgnore]
        public bool BoolValue { get { return Convert.ToBoolean(Value); } set { Value = value.ToString(); } }

        [JsonIgnore]
        public int IntValue { get { return Convert.ToInt32(Value); } set { Value = value.ToString(); } }
    }
}
