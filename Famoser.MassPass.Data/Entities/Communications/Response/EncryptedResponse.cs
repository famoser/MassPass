﻿using System;

namespace Famoser.MassPass.Data.Entities.Communications.Response
{
    public class EncryptedResponse
    {
        public string VersionId { get; set; }
        public Uri DownloadUri { get; set; }
    }
}
