using Famoser.FrameworkEssentials.Attributes;

namespace Famoser.MassPass.Data.Enum
{
    /// <summary>
    /// indicates if the local file has been changed since the last sync, or has conflicts with server versions
    /// </summary>
    public enum LocalStatus
    {
        [Description("new")]
        New = 0,
        [Description("up to date")]
        UpToDate = 1,
        [Description("changed")]
        Changed = 2,
        [Description("unresolved conflict")]
        Conflict = 3,
        [Description("immutable")] //for example a history entry is immutable
        Immutable = 4
    }
}
