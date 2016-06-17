using System;
using System.Collections.Generic;

namespace Famoser.MassPass.Business.Models.Storage
{
    public class PasswordVaultStorageModel
    {
        public PasswordVaultStorageModel()
        {
            LastModifiedDateTime = DateTime.Now;
            Vault = new Dictionary<Guid, byte[]>();
        }
        public DateTime LastModifiedDateTime { get; set; }
        public Dictionary<Guid, byte[]> Vault { get; set; }
    }
}
