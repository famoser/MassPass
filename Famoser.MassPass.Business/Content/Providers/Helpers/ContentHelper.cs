using System;
using System.Collections.Generic;
using System.Linq;
using Famoser.MassPass.Business.Content.Providers.Base;
using Famoser.MassPass.Business.Content.Providers.Interfaces;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Models.Content;
using Famoser.MassPass.Business.Models.Content.Base;
using Newtonsoft.Json;

namespace Famoser.MassPass.Business.Content.Providers.Helpers
{
    public class ContentHelper
    {
        public static readonly List<IContentModelProvider> ContentModelProviders = new List<IContentModelProvider>()
        {
            new CollectionContentModelProvider(),
            new CreditCardContentModelProvider(),
            new LoginContentModelProvider(),
            new NoteContentModelProvider()
        };

        public static BaseContentModel Deserialize(string json)
        {
            foreach (var contentModelProvider in ContentModelProviders)
            {
                if (contentModelProvider.CanDeserialize(json))
                    return contentModelProvider.Deserialize(json);
            }
            return null;
        }
    }
}
