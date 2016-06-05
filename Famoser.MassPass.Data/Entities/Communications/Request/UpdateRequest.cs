using System;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class UpdateRequest : RawUpdateRequest
    {
        public ContentEntity ContentEntity { get; set; }
    }
}
