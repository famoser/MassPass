using Famoser.FrameworkEssentials.Attributes;

namespace Famoser.MassPass.Data.Enum
{
    /// <summary>
    /// response from the server; indicates if contentmodel is up to date or not
    /// </summary>
    public enum ApiStatus
    {
        [Description("up to date")]
        UpToDate = 1,
        [Description("changed")]
        Changed = 2,
        [Description("not found on server")]
        NotFound = 3
    }
}
