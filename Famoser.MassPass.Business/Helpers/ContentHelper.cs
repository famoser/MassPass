using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.Business.Helpers
{
    public class ContentHelper
    {
        private static readonly Dictionary<Guid, ContentTypes> Types = new Dictionary<Guid, ContentTypes>()
        {
            { Guid.Parse("2576024e-1d60-4127-bf0f-f1a83a8af93c"), ContentTypes.Folder},
            { Guid.Parse("b7e6507f-3397-4565-9768-05267f4efced"), ContentTypes.Root},
            { Guid.Parse("a63b4e52-0a45-4ca5-8327-4ef800e00e57"), ContentTypes.Note}
        };

        public static ContentTypes GetType(Guid guid)
        {
            return Types.ContainsKey(guid) ? Types[guid] : ContentTypes.Unknown;
        }

        public static Guid GetTypeId(ContentTypes type)
        {
            var res = Types.Where(s => s.Value == type).ToList();
            return res.Count > 0 ? res[0].Key : Guid.Empty;
        }
    }
}
