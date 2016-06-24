using Famoser.FrameworkEssentials.Attributes;

namespace Famoser.MassPass.View.Enums
{
    public enum ProgressKeys
    {
        [Description("Syncing in progress...")]
        Sync,
        [Description("Filling history...")]
        FillHistory,
        [Description("Saving...")]
        Saving,
        [Description("Initializing user...")]
        IsSettingUserConfiguration,
        [Description("Initializing application...")]
        IsInitializingApplication,
        [Description("Unlocking...")]
        Unlocking,

    }
}
