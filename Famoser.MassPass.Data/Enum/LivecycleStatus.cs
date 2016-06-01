using Famoser.FrameworkEssentials.Attributes;

namespace Famoser.MassPass.Data.Enum
{
    /// <summary>
    /// indicates the status of the contentmodel.
    /// </summary>
    public enum LivecycleStatus
    {
        [Description("normal")]
        Normal = 1,
        [Description("deleted")]
        Deleted = 2
    }
}
