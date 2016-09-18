using Famoser.FrameworkEssentials.Attributes;

namespace Famoser.MassPass.Data.Enum
{
    /// <summary>
    /// response from the server; indicates if contentmodel is up to date or not
    /// </summary>
    public enum ServerVersion
    {
        [Description("no version on server")]
        None = 0,
        [Description("same version on server")]
        Same = 1,
        [Description("older version on server")]
        Older = 2,
        [Description("newer version on server")]
        Newer = 3
    }
}
