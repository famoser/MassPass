using System;
using System.Collections.Generic;

namespace Famoser.MassPass.Business.Models.Storage
{
    public class PasswordVaultStorageModel
    {
        public PasswordVaultStorageModel()
        {
            Vault = new Dictionary<Guid, string>();
        }
    }
}
