using Famoser.FrameworkEssentials.Attributes;

namespace Famoser.MassPass.Business.Enums
{
    public enum FileKeys
    {
        [Description("Assets/Configuration/configuration.json")]
        AssetConfiguration,
        [Description("cache.V1")]
        EncryptedCache,
        [Description("passwords.V1")]
        EncryptedPasswords,
        [Description("configuration.V1")]
        EncryptedConfiguration,
    }
}
