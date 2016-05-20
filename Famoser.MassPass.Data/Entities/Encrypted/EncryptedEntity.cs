using Famoser.MassPass.Data.Entities.Sync;

namespace Famoser.MassPass.Data.Entities.Encrypted
{
    public class EncryptedEntity : SyncEntity
    {
        public byte[] Content { get; set; }
    }
}
