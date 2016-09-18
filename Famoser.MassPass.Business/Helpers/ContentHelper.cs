using System;
using System.Collections.Generic;
using System.Linq;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Models.Content;
using Famoser.MassPass.Business.Models.Content.Base;
using Famoser.MassPass.Business.Models.Sync;
using Famoser.MassPass.Data.Entities;
using Famoser.MassPass.Data.Entities.Communications.Request;
using Famoser.MassPass.Data.Entities.Communications.Request.Authorization;
using Famoser.MassPass.Data.Entities.Communications.Request.Base;
using Famoser.MassPass.Data.Entities.Communications.Request.Entities;
using Famoser.MassPass.Data.Models.Storage;

namespace Famoser.MassPass.Business.Helpers
{
    public class ContentHelper
    {
        private static readonly Dictionary<Guid, ContentTypes> Types = new Dictionary<Guid, ContentTypes>()
        {
            { Guid.Parse("2576024e-1d60-4127-bf0f-f1a83a8af93c"), ContentTypes.Collection},
            { Guid.Parse("b7e6507f-3397-4565-9768-05267f4efced"), ContentTypes.CreditCard},
            { Guid.Parse("a63b4e52-0a45-4ca5-8327-4ef800e00e57"), ContentTypes.Note},
            { Guid.Parse("20044381-b364-432e-98b8-70b38ca45282"), ContentTypes.Login}
        };
        public static ContentTypes GetContentTypeByGuid(Guid guid)
        {
            return Types.ContainsKey(guid) ? Types[guid] : ContentTypes.Unknown;
        }

        public static Guid GetGuidOfContentType(ContentTypes type)
        {
            var res = Types.Where(s => s.Value == type).ToList();
            return res.Count > 0 ? res[0].Key : Guid.Empty;
        }
    }
}
