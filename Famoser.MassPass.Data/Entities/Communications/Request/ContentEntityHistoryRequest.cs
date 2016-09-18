using System;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class ContentEntityHistoryRequest : ApiRequest
    {
        public Guid ContentId { get; set; }
    }
}
