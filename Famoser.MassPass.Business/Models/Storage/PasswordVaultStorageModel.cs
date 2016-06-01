using System;
using System.Collections.Generic;

namespace Famoser.MassPass.Business.Models.Storage
{
    public class PasswordVaultStorageModel
    {
        public DateTime LastModifiedDateTime { get; set; }
        public Dictionary<Guid, byte[]> Vault { get; set; }
    }
}
