﻿using System;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;

namespace Famoser.MassPass.Data.Entities.Communications.Request
{
    public class UpdateRequest : ApiRequest
    {
        public Guid ContentId { get; set; }
        public Guid CollectionId { get; set; }
        public string VersionId { get; set; }
    }
}
