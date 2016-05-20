using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Models.Base
{
    public class BaseSyncModel : BaseModel
    {
        [JsonConstructor]
        public BaseSyncModel()
        {
        }

        public BaseSyncModel(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }
    }
}
