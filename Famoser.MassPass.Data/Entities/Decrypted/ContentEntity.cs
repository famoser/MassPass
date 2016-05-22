namespace Famoser.MassPass.Data.Entities.Decrypted
{
    public class ContentEntity : CollectionEntity
    {
        public string ContentJson { get; set; }
        public byte[] ContentFile { get; set; }
    }
}
