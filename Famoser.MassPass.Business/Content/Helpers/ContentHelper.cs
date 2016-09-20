using System.Collections.Generic;
using Famoser.MassPass.Business.Content.Enums;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Content.Providers;
using Famoser.MassPass.Business.Content.Providers.Interfaces;
using Famoser.MassPass.Business.Enums;

namespace Famoser.MassPass.Business.Content.Helpers
{
    public class ContentHelper
    {
        public static readonly List<IContentModelProvider> ContentModelProviders = new List<IContentModelProvider>();

        public static void RegisterContentModelProvider(IContentModelProvider provider)
        {
            ContentModelProviders.Add(provider);
        }

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
            return GetProvider(model.ContentType);
        }

        public static IContentModelProvider GetProvider(ContentType type)
        {
            foreach (var contentModelProvider in ContentModelProviders)
            {
                if (contentModelProvider.GetContentType() == type)
                    return contentModelProvider;
            }
            return null;
        }
    }
}
