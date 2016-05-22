namespace Famoser.MassPass.Data.Entities
{
    public class ContentEntity : CollectionEntity
    {
        public string ContentJson { get; set; }
        public byte[] ContentFile { get; set; }
    }
}
