using System;
using System.Collections.Generic;

namespace Famoser.MassPass.Business.Models.Storage
{
    public class VaultStorageModel
    {
        public VaultStorageModel()
        {
            Vault = new Dictionary<Guid, string>();
        }
        public Dictionary<Guid, string> Vault { get; set; }
    }
}
