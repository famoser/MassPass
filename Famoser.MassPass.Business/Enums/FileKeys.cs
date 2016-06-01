using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Attributes;

namespace Famoser.MassPass.Business.Enums
{
    public enum FileKeys
    {
        [Description("passwordVault.V1")]
        PasswordVault,
        [Description("apiConfiguration.V1")]
        ApiConfiguration,
        [Description("configuration.V1")]
        Configuration,
        [Description("Assets/Configuration/configuration.json")]
        AssetConfiguration,
        [Description("cache.V1")]
        CollectionCache,
    }
}
