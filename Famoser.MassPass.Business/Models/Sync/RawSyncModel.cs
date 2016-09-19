using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Business.Models.Sync
{
    public class RawSyncModel
    {
        public LivecycleStatus LivecycleStatus { get; set; }
        public string ContentJson { get; set; }
        public byte[] ContentFile { get; set; }
    }
}
