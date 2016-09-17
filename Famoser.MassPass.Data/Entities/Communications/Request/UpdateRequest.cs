using Famoser.MassPass.Data.Entities.Communications.Request.Raw;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class UpdateRequest : RawUpdateRequest
    {
        public ContentEntity ContentEntity { get; set; }
    }
}
