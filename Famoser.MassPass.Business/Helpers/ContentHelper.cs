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
        private static Dictionary<Guid, ContentTypes> _types = new Dictionary<Guid, ContentTypes>()
        {
            { Guid.Parse("2576024e-1d60-4127-bf0f-f1a83a8af93c"), ContentTypes.Folder},
            { Guid.Parse("b7e6507f-3397-4565-9768-05267f4efced"), ContentTypes.Root}
        };

        public static ContentTypes GetType(CollectionModel model)
        {
            return _types.ContainsKey(model.TypeId) ? _types[model.TypeId] : ContentTypes.Unknown;
        }
    }
}
