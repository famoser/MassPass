﻿using System;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class ContentEntityRequest : ApiRequest
    {
        public Guid ServerId { get; set; }
        public string VersionId { get; set; }
        public Guid RelationId { get; set; }
    }
}
