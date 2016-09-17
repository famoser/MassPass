using Famoser.FrameworkEssentials.Attributes;

namespace Famoser.MassPass.Data.Enum
{
    /// <summary>
    /// response from the server; indicates if contentmodel is up to date or not
    /// </summary>
    public enum ServerStatus
    {
        [Description("no version on server")]
        New = 0,
        [Description("same version on server")]
        UpToDate = 1,
        [Description("older version on server")]
        Changed = 2
    }
}
