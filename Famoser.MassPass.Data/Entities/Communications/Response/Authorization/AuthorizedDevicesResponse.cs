﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Data.Entities.Communications.Request.Entities;
using Famoser.MassPass.Data.Entities.Communications.Response.Base;
using Famoser.MassPass.Data.Entities.Communications.Response.Entitites;

namespace Famoser.MassPass.Data.Entities.Communications.Response.Authorization
{
    public class AuthorizedDevicesResponse : ApiResponse
    {
        public List<AuthorizedDeviceEntity> AuthorizedDeviceEntities { get; set; }
    }
}