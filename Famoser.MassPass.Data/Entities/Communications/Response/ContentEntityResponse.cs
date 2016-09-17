using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class ContentEntityResponse : ApiResponse
    {
        public ContentEntity ContentEntity { get; set; }
        public ContentApiInformations ContentApiInformations { get; set; }
    }
}
