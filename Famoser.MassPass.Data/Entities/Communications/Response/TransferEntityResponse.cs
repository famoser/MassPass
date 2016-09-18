using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class TransferEntityResponse : ApiResponse
    {
        public byte[] EncryptedBytes { get; set; }
    }
}
