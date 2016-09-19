using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models.Storage.Cache;
using Famoser.MassPass.Data.Models.Configuration;

namespace Famoser.MassPass.Business.Models.Storage
{
    public class EncryptedStorageModel
    {
        public ApiConfiguration ApiConfiguration { get; set; }
        public UserConfiguration UserConfiguration { get; set; }
        public Dictionary<Guid, string> Vault { get; set; }
        public ObservableCollection<CollectionCacheModel> CollectionCacheModels { get; set; }
    }
}
