using System.Collections.Generic;
using Famoser.MassPass.Business.Content.Providers;
using Famoser.MassPass.Business.Content.Providers.Interfaces;
using Famoser.MassPass.Business.Models.Content.Base;

namespace Famoser.MassPass.Business.Content.Helpers
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

        public static IContentModelProvider GetProvider(BaseContentModel model)
        {
            foreach (var contentModelProvider in ContentModelProviders)
            {
                if (contentModelProvider.GetContentType() == model.ContentType)
                    return contentModelProvider;
            }
            return null;
        }
    }
}
